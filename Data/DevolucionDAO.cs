using System;
using MySql.Data.MySqlClient;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos.Data
{
    public class ResultadoDevolucion
    {
        public int IdDevolucion { get; set; }
        public int DiasRetraso { get; set; }
        public bool ConRetraso { get { return DiasRetraso > 0; } }
        public string EstadoMaterial { get; set; } = EstadosMaterial.Disponible;
    }

    public class DevolucionDAO
    {

        public ResultadoDevolucion RegistrarDevolucion(int idPrestamo, DateTime fechaReal,
                                                       string estadoMaterialRecibido, string? observaciones)
        {
            Sesion.ExigirPersonal();

            fechaReal = fechaReal.Date;

            if (Array.IndexOf(EstadosMaterial.AlDevolver, estadoMaterialRecibido) < 0)
                throw new ArgumentException("El material solo puede recibirse como Disponible, Dañado o Mantenimiento.");

            if (fechaReal > DateTime.Today)
                throw new ArgumentException("La fecha real de devolución no puede ser futura.");

            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int idMaterial;
                        DateTime fechaPrestamo;
                        DateTime fechaLimite;
                        string estadoPrestamo;

                        using (MySqlCommand cmd = new MySqlCommand(@"
                            SELECT IdMaterial, FechaPrestamo, FechaLimite, Estado
                            FROM Prestamo WHERE IdPrestamo = @id FOR UPDATE;", conexion, transaccion))
                        {
                            cmd.Parameters.Add(Db.P("@id", idPrestamo));
                            using (MySqlDataReader r = cmd.ExecuteReader())
                            {
                                if (!r.Read())
                                    throw new InvalidOperationException("El préstamo no existe.");

                                idMaterial = Db.Entero(r, "IdMaterial");
                                fechaPrestamo = Db.Fecha(r, "FechaPrestamo");
                                fechaLimite = Db.Fecha(r, "FechaLimite");
                                estadoPrestamo = Db.TextoONulo(r, "Estado") ?? string.Empty;
                            }
                        }

                        if (estadoPrestamo != EstadosPrestamo.Activo && estadoPrestamo != EstadosPrestamo.Mora)
                            throw new InvalidOperationException("El préstamo ya está finalizado.");

                        if (fechaReal < fechaPrestamo.Date)
                            throw new ArgumentException("La fecha real no puede ser anterior a la fecha del préstamo (" +
                                                        fechaPrestamo.ToShortDateString() + ").");

                        long previas;
                        using (MySqlCommand cmd = new MySqlCommand(
                            "SELECT COUNT(*) FROM Devolucion WHERE IdPrestamo = @id;", conexion, transaccion))
                        {
                            cmd.Parameters.Add(Db.P("@id", idPrestamo));
                            previas = Convert.ToInt64(cmd.ExecuteScalar());
                        }

                        if (previas > 0)
                            throw new InvalidOperationException("Este préstamo ya tiene una devolución registrada.");

                        int idDevolucion;
                        using (MySqlCommand cmd = new MySqlCommand(@"
                            INSERT INTO Devolucion (IdPrestamo, FechaReal, Observaciones)
                            VALUES (@p, @f, @o);", conexion, transaccion))
                        {
                            cmd.Parameters.Add(Db.P("@p", idPrestamo));
                            cmd.Parameters.Add(Db.P("@f", fechaReal));
                            cmd.Parameters.Add(Db.P("@o", string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()));
                            cmd.ExecuteNonQuery();
                            idDevolucion = (int)cmd.LastInsertedId;
                        }

                        int filasPrestamo;
                        using (MySqlCommand cmd = new MySqlCommand(@"
                            UPDATE Prestamo
                            SET Estado = 'Finalizado', FechaDevolucionReal = @f
                            WHERE IdPrestamo = @id AND Estado IN ('Activo', 'Mora');", conexion, transaccion))
                        {
                            cmd.Parameters.Add(Db.P("@f", fechaReal));
                            cmd.Parameters.Add(Db.P("@id", idPrestamo));
                            filasPrestamo = cmd.ExecuteNonQuery();
                        }

                        if (filasPrestamo != 1)
                            throw new InvalidOperationException("No se pudo finalizar el préstamo.");

                        int filasMaterial;
                        using (MySqlCommand cmd = new MySqlCommand(@"
                            UPDATE Material SET Estado = @estado
                            WHERE IdMaterial = @id AND Estado = 'Prestado';", conexion, transaccion))
                        {
                            cmd.Parameters.Add(Db.P("@estado", estadoMaterialRecibido));
                            cmd.Parameters.Add(Db.P("@id", idMaterial));
                            filasMaterial = cmd.ExecuteNonQuery();
                        }

                        if (filasMaterial != 1)
                            throw new InvalidOperationException("El material asociado no estaba en estado Prestado; revise el inventario.");

                        transaccion.Commit();

                        int retraso = (fechaReal - fechaLimite.Date).Days;
                        return new ResultadoDevolucion
                        {
                            IdDevolucion = idDevolucion,
                            DiasRetraso = retraso > 0 ? retraso : 0,
                            EstadoMaterial = estadoMaterialRecibido
                        };
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
