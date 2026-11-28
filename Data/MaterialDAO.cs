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
    public class MaterialDAO
    {
        private const string Columnas =
            "IdMaterial, NumeroSerie, Nombre, Categoria, IdCategoria, Estado, Descripcion, Precio, Stock, FechaRegistro, RutaImagen";

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.4 - Inserción relacional (MariaDB): INSERT parametrizado (@parámetros), sin concatenar texto.
        //  La llave de la categoría llega desde el SelectedValue del ComboBox (IdCategoria).
        // =====================================================================
        public int Insertar(Material material)
        {
            Sesion.ExigirPersonal();
            if (material == null) throw new ArgumentNullException(nameof(material));

            if (material.Estado == EstadosMaterial.Prestado)
                throw new InvalidOperationException("Un material nuevo no puede darse de alta como Prestado; ese estado lo asigna el registro de un préstamo.");

            // A20 - Autorización: solo el Administrador fija precios; el Operador (vendedor) no puede.
            if (!Sesion.EsAdministrador && material.Precio != 0)
                throw new UnauthorizedAccessException("Solo el Administrador puede asignar o modificar precios.");

            const string sql = @"
                INSERT INTO Material (NumeroSerie, Nombre, Categoria, IdCategoria, Estado, Descripcion, Precio, Stock, RutaImagen)
                VALUES (@serie, @nombre, @categoria, @idCategoria, @estado, @descripcion, @precio, @stock, @ruta);";

            try
            {
                using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    AgregarParametros(comando, material);
                    conexion.Open();
                    comando.ExecuteNonQuery();
                    material.Id = (int)comando.LastInsertedId;
                    return material.Id;
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new InvalidOperationException("Ya existe un material con ese número de serie.");
            }
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.5 - Actualización relacional (MariaDB): UPDATE parametrizado con WHERE sobre la llave primaria.
        //  Si ninguna fila coincide con el Id, se informa en lugar de dar por buena la operación.
        // =====================================================================
        public void Actualizar(Material material)
        {
            Sesion.ExigirPersonal();
            if (material == null) throw new ArgumentNullException(nameof(material));

            Material? actual = ObtenerPorId(material.Id);
            if (actual == null)
                throw new InvalidOperationException("El material no existe.");

            // A20 - Autorización: el rol se evalúa ANTES de ejecutar el UPDATE; el Operador no puede cambiar el precio.
            if (!Sesion.EsAdministrador && actual.Precio != material.Precio)
                throw new UnauthorizedAccessException("Solo el Administrador puede modificar precios.");

            bool prestamoAbierto = TienePrestamoAbierto(material.Id);

            if (prestamoAbierto && material.Estado != EstadosMaterial.Prestado)
                throw new InvalidOperationException("El material tiene un préstamo abierto. Registre primero su devolución.");

            if (!prestamoAbierto && material.Estado == EstadosMaterial.Prestado)
                throw new InvalidOperationException("El estado Prestado solo se asigna al registrar un préstamo.");

            const string sql = @"
                UPDATE Material
                SET NumeroSerie = @serie, Nombre = @nombre, Categoria = @categoria, IdCategoria = @idCategoria,
                    Estado = @estado, Descripcion = @descripcion, Precio = @precio, Stock = @stock, RutaImagen = @ruta
                WHERE IdMaterial = @id;";

            try
            {
                using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    AgregarParametros(comando, material);
                    comando.Parameters.Add(Db.P("@id", material.Id));
                    conexion.Open();

                    int filas = comando.ExecuteNonQuery();
                    if (filas == 0)
                        throw new InvalidOperationException("No se actualizó ningún registro: el material con Id " + material.Id + " ya no existe.");
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new InvalidOperationException("Ya existe un material con ese número de serie.");
            }
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.6 - Borrado LÓGICO: no hay DELETE; el UPDATE deja el estado en Baja (columna Activo = 0).
        // =====================================================================
        public void Desactivar(int idMaterial)
        {
            Sesion.ExigirAdministrador();

            Material? material = ObtenerPorId(idMaterial);
            if (material == null)
                throw new InvalidOperationException("El material no existe.");

            material.Estado = EstadosMaterial.Baja;
            Actualizar(material);
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.6 - Borrado FÍSICO: DELETE parametrizado por llave primaria (nunca masivo). Si el material tiene
        //  préstamos asociados (llave foránea, error 1451) se explica al usuario y no se borra nada.
        // =====================================================================
        public void Eliminar(int idMaterial)
        {
            Sesion.ExigirAdministrador();

            const string sql = "DELETE FROM Material WHERE IdMaterial = @id;";

            try
            {
                using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.Add(Db.P("@id", idMaterial));
                    conexion.Open();

                    int filas = comando.ExecuteNonQuery();
                    if (filas == 0)
                        throw new InvalidOperationException("El material no existe o ya fue eliminado.");
                }
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar definitivamente: el material tiene préstamos o pedidos registrados (llave foránea activa).\n" +
                    "Use la baja lógica («Desactivar») para ocultarlo conservando su historial.");
            }
            catch (MySqlException ex) when (ex.Number == 1142)
            {
                throw new InvalidOperationException(
                    "El usuario de la base de datos no tiene permiso DELETE sobre Material.\n" +
                    "Ejecute el script Database/Migracion_A4_6_A4_7.sql como root.");
            }
        }

        public Material? ObtenerPorId(int id)
        {
            const string sql = "SELECT " + Columnas + " FROM Material WHERE IdMaterial = @id LIMIT 1;";

            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            using (MySqlCommand comando = new MySqlCommand(sql, conexion))
            {
                comando.Parameters.Add(Db.P("@id", id));
                conexion.Open();
                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read()) return null;
                    return new Material(
                        Db.Entero(lector, "IdMaterial"),
                        Db.TextoONulo(lector, "NumeroSerie"),
                        Db.TextoONulo(lector, "Nombre") ?? string.Empty,
                        Db.TextoONulo(lector, "Categoria") ?? CategoriasMaterial.Otro,
                        Db.TextoONulo(lector, "Estado") ?? EstadosMaterial.Disponible,
                        Db.TextoONulo(lector, "Descripcion"),
                        Db.Fecha(lector, "FechaRegistro"),
                        Db.TextoONulo(lector, "RutaImagen"))
                    {
                        IdCategoria = lector.IsDBNull(lector.GetOrdinal("IdCategoria")) ? (int?)null : Db.Entero(lector, "IdCategoria"),
                        Precio = Db.Decimal(lector, "Precio"),
                        Stock = Db.Entero(lector, "Stock")
                    };
                }
            }
        }

        // Filtro del borrado lógico: Activo = 1 oculta los materiales en Baja, salvo que se pida ver las bajas.
        public DataTable Listar(string? texto, string? categoria, string? estado, bool incluirBajas = false)
        {
            Sesion.ExigirSesion();

            const string sql = @"
                SELECT IdMaterial AS Id, NumeroSerie AS Serie, Nombre, Categoria, Estado, Descripcion,
                       Precio, Stock, IdCategoria, RutaImagen
                FROM Material
                WHERE (Nombre LIKE @p OR NumeroSerie LIKE @p OR Descripcion LIKE @p)
                  AND (@categoria = '' OR Categoria = @categoria)
                  AND (@estado = '' OR Estado = @estado)
                  AND (@todos = 1 OR Activo = 1 OR @estado = 'Baja')
                ORDER BY Nombre;";

            return Db.Tabla(sql,
                Db.P("@p", "%" + (texto ?? string.Empty).Trim() + "%"),
                Db.P("@categoria", categoria ?? string.Empty),
                Db.P("@estado", estado ?? string.Empty),
                Db.P("@todos", incluirBajas ? 1 : 0));
        }

        public DataTable ListarDisponibles()
        {
            Sesion.ExigirSesion();

            const string sql = @"
                SELECT IdMaterial, Nombre, Categoria, NumeroSerie AS Serie,
                       CONCAT(Nombre, ' [', COALESCE(NumeroSerie, 's/n'), '] - ', Categoria) AS Texto
                FROM Material
                WHERE Estado = 'Disponible'
                ORDER BY Nombre;";

            return Db.Tabla(sql);
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.7 - Búsqueda de productos: por llave primaria exacta (id) o por coincidencia parcial con LIKE (nombre).
        // =====================================================================
        public DataTable BuscarProductos(int? id, string? nombre)
        {
            Sesion.ExigirSesion();

            const string sql = @"
                SELECT IdMaterial AS Id, Nombre, Categoria, Precio, Stock
                FROM Material
                WHERE Activo = 1 AND Estado = 'Disponible'
                  AND (@id IS NULL OR IdMaterial = @id)
                  AND Nombre LIKE @nombre
                ORDER BY Nombre;";

            return Db.Tabla(sql,
                Db.P("@id", id),
                Db.P("@nombre", "%" + (nombre ?? string.Empty).Trim() + "%"));
        }

        public bool TienePrestamoAbierto(int idMaterial)
        {
            return Db.Escalar(
                "SELECT COUNT(*) FROM Prestamo WHERE IdMaterial = @id AND Estado IN ('Activo', 'Mora');",
                Db.P("@id", idMaterial)) > 0;
        }

        private static void AgregarParametros(MySqlCommand comando, Material m)
        {
            comando.Parameters.Add(Db.P("@serie", m.NumeroSerie));
            comando.Parameters.Add(Db.P("@nombre", m.Nombre));
            comando.Parameters.Add(Db.P("@categoria", m.Categoria));
            comando.Parameters.Add(Db.P("@idCategoria", m.IdCategoria));
            comando.Parameters.Add(Db.P("@estado", m.Estado));
            comando.Parameters.Add(Db.P("@descripcion", m.Descripcion));
            comando.Parameters.Add(Db.P("@precio", m.Precio));
            comando.Parameters.Add(Db.P("@stock", m.Stock));
            comando.Parameters.Add(Db.P("@ruta", m.RutaImagen));
        }
    }
}
