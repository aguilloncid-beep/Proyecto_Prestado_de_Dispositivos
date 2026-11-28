// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using System.Data;
using MySql.Data.MySqlClient;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos.Data
{
    public class PedidoDAO
    {
        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.8 - Persistencia transaccional (ACID) de la relación 1:N en MariaDB. Todo ocurre dentro de una
        //  MySqlTransaction: encabezado (Pedidos), renglones (Detalles_Pedidos) y descuento de Stock. Si UNA sola
        //  instrucción falla se ejecuta Rollback y la base queda exactamente como estaba; si todas tienen éxito, Commit.
        // =====================================================================
        public int Guardar(Pedido pedido)
        {
            Sesion.ExigirPersonal();

            if (pedido == null) throw new ArgumentNullException(nameof(pedido));
            if (string.IsNullOrWhiteSpace(pedido.Cliente))
                throw new InvalidOperationException("El nombre del cliente es obligatorio.");
            if (pedido.Detalles.Count == 0)
                throw new InvalidOperationException("El pedido no tiene renglones.");

            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();

                // Inicio de la transacción explícita sobre este canal de datos
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        // 1) Encabezado (el "uno")
                        const string sqlEncabezado =
                            "INSERT INTO Pedidos (Fecha, Cliente, Total) VALUES (@fecha, @cliente, @total);";

                        using (MySqlCommand insertarEncabezado = new MySqlCommand(sqlEncabezado, conexion, transaccion))
                        {
                            insertarEncabezado.Parameters.Add(Db.P("@fecha", pedido.Fecha));
                            insertarEncabezado.Parameters.Add(Db.P("@cliente", pedido.Cliente.Trim()));
                            insertarEncabezado.Parameters.Add(Db.P("@total", pedido.Total));
                            insertarEncabezado.ExecuteNonQuery();
                        }

                        // 2) Llave primaria autogenerada en el servidor, dentro de la misma transacción
                        int idPedido;
                        using (MySqlCommand obtenerId = new MySqlCommand("SELECT LAST_INSERT_ID();", conexion, transaccion))
                        {
                            idPedido = Convert.ToInt32(obtenerId.ExecuteScalar());
                        }

                        if (idPedido <= 0)
                            throw new InvalidOperationException("No se pudo recuperar el Id del encabezado del pedido.");

                        // 3) Renglones (los "muchos") y descuento de stock, uno por uno
                        const string sqlDetalle =
                            "INSERT INTO Detalles_Pedidos (IdPedido, IdMaterial, Cantidad, Precio) VALUES (@idPedido, @idMaterial, @cantidad, @precio);";

                        // La condición Stock >= @cantidad evita stock negativo aunque otro usuario haya comprado mientras tanto.
                        const string sqlStock =
                            "UPDATE Material SET Stock = Stock - @cantidad WHERE IdMaterial = @idMaterial AND Stock >= @cantidad AND Activo = 1;";

                        foreach (DetallePedido linea in pedido.Detalles)
                        {
                            using (MySqlCommand insertarDetalle = new MySqlCommand(sqlDetalle, conexion, transaccion))
                            {
                                insertarDetalle.Parameters.Add(Db.P("@idPedido", idPedido));
                                insertarDetalle.Parameters.Add(Db.P("@idMaterial", linea.IdMaterial));
                                insertarDetalle.Parameters.Add(Db.P("@cantidad", linea.Cantidad));
                                insertarDetalle.Parameters.Add(Db.P("@precio", linea.PrecioUnitario));
                                insertarDetalle.ExecuteNonQuery();
                            }

                            using (MySqlCommand descontar = new MySqlCommand(sqlStock, conexion, transaccion))
                            {
                                descontar.Parameters.Add(Db.P("@cantidad", linea.Cantidad));
                                descontar.Parameters.Add(Db.P("@idMaterial", linea.IdMaterial));

                                if (descontar.ExecuteNonQuery() == 0)
                                    throw new InvalidOperationException(
                                        "Stock insuficiente para «" + linea.Nombre + "». No se guardó ninguna parte del pedido.");
                            }
                        }

                        // 4) Todo salió bien: se confirman encabezado, renglones y stock a la vez
                        transaccion.Commit();
                        pedido.IdPedido = idPedido;
                        return idPedido;
                    }
                    catch
                    {
                        // Una sola falla deshace todo: sin encabezados huérfanos ni stock descontado a medias
                        try { transaccion.Rollback(); }
                        catch (Exception) { }
                        throw;
                    }
                }
            }
        }

        // Historial: encabezados de todos los pedidos ordenados cronológicamente.
        public DataTable ListarEncabezados()
        {
            Sesion.ExigirSesion();

            const string sql = @"
                SELECT p.IdPedido AS Id, p.Fecha, p.Cliente, p.Total, COUNT(d.IdDetalle) AS Renglones
                FROM Pedidos p
                LEFT JOIN Detalles_Pedidos d ON d.IdPedido = p.IdPedido
                GROUP BY p.IdPedido, p.Fecha, p.Cliente, p.Total
                ORDER BY p.Fecha, p.IdPedido;";

            return Db.Tabla(sql);
        }

        // Detalle bajo demanda: INNER JOIN con Material filtrado por el Id del pedido seleccionado.
        public DataTable ListarDetalles(int idPedido)
        {
            Sesion.ExigirSesion();

            const string sql = @"
                SELECT d.IdDetalle AS Renglon, m.IdMaterial AS Id, m.Nombre, d.Cantidad,
                       d.Precio AS PrecioUnitario, d.Cantidad * d.Precio AS Subtotal
                FROM Detalles_Pedidos d
                INNER JOIN Material m ON m.IdMaterial = d.IdMaterial
                WHERE d.IdPedido = @idPedido
                ORDER BY d.IdDetalle;";

            return Db.Tabla(sql, Db.P("@idPedido", idPedido));
        }
    }
}
