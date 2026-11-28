using System;
using System.Data;
using MySql.Data.MySqlClient;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos.Data
{
    public class PrestamoDAO
    {

        public int RegistrarPrestamo(int idUsuario, int idMaterial, DateTime fechaPrestamo, DateTime fechaLimite)
        {
            Sesion.ExigirPersonal();

            fechaPrestamo = fechaPrestamo.Date;
            fechaLimite = fechaLimite.Date;

            if (fechaLimite < fechaPrestamo)
                throw new ArgumentException("La fecha límite no puede ser anterior a la fecha de préstamo.");

            int dias = (fechaLimite - fechaPrestamo).Days;
            if (!Material.DiasPrestamoValidos(dias))
                throw new ArgumentException("El préstamo debe durar entre 1 y " + Material.DiasMaximosPrestamo + " días.");

            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        bool usuarioActivo;
                        using (MySqlCommand cmd = Comando(conexion, transaccion,
                            "SELECT Activo FROM Usuario WHERE IdUsuario = @id FOR UPDATE;", Db.P("@id", idUsuario)))
                        {
                            object? r = cmd.ExecuteScalar();
                            if (r == null || r == DBNull.Value)
                                throw new InvalidOperationException("El usuario no existe.");
                            usuarioActivo = Convert.ToBoolean(r);
                        }

                        if (!usuarioActivo)
                            throw new InvalidOperationException("El usuario está desactivado.");

                        long abiertos;
                        using (MySqlCommand cmd = Comando(conexion, transaccion,
                            "SELECT COUNT(*) FROM Prestamo WHERE IdUsuario = @id AND Estado IN ('Activo', 'Mora');",
                            Db.P("@id", idUsuario)))
                        {
                            abiertos = Convert.ToInt64(cmd.ExecuteScalar());
                        }

                        if (abiertos >= Usuario.LimitePrestamosActivos)
                            throw new InvalidOperationException("El usuario ya tiene " + Usuario.LimitePrestamosActivos + " préstamos abiertos.");

                        string? estadoMaterial;
                        using (MySqlCommand cmd = Comando(conexion, transaccion,
                            "SELECT Estado FROM Material WHERE IdMaterial = @id FOR UPDATE;", Db.P("@id", idMaterial)))
                        {
                            object? r = cmd.ExecuteScalar();
                            estadoMaterial = (r == null || r == DBNull.Value) ? null : Convert.ToString(r);
                        }

                        if (estadoMaterial == null)
                            throw new InvalidOperationException("El material no existe.");
                        if (estadoMaterial != EstadosMaterial.Disponible)
                            throw new InvalidOperationException("El material no está disponible (estado actual: " + estadoMaterial + ").");

                        int idPrestamo;
                        using (MySqlCommand cmd = Comando(conexion, transaccion, @"
                            INSERT INTO Prestamo (IdUsuario, IdMaterial, FechaPrestamo, FechaLimite, Estado)
                            VALUES (@u, @m, @fp, @fl, 'Activo');",
                            Db.P("@u", idUsuario), Db.P("@m", idMaterial),
                            Db.P("@fp", fechaPrestamo), Db.P("@fl", fechaLimite)))
                        {
                            cmd.ExecuteNonQuery();
                            idPrestamo = (int)cmd.LastInsertedId;
                        }

                        int filas;
                        using (MySqlCommand cmd = Comando(conexion, transaccion, @"
                            UPDATE Material SET Estado = 'Prestado'
                            WHERE IdMaterial = @m AND Estado = 'Disponible';", Db.P("@m", idMaterial)))
                        {
                            filas = cmd.ExecuteNonQuery();
                        }

                        if (filas != 1)
                            throw new InvalidOperationException("No se pudo reservar el material; intente de nuevo.");

                        transaccion.Commit();
                        return idPrestamo;
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        public int ActualizarMoras()
        {
            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            using (MySqlCommand comando = new MySqlCommand(
                "UPDATE Prestamo SET Estado = 'Mora' WHERE Estado = 'Activo' AND FechaLimite < @hoy;", conexion))
            {
                comando.Parameters.Add(Db.P("@hoy", DateTime.Today));
                conexion.Open();
                return comando.ExecuteNonQuery();
            }
        }

        public int ContarEnMora()
        {
            return (int)Db.Escalar("SELECT COUNT(*) FROM Prestamo WHERE Estado = 'Mora';");
        }

        public Prestamo? ObtenerPorId(int idPrestamo)
        {
            const string sql = @"
                SELECT IdPrestamo, IdUsuario, IdMaterial, FechaPrestamo, FechaLimite, FechaDevolucionReal, Estado, FechaRegistro
                FROM Prestamo WHERE IdPrestamo = @id LIMIT 1;";

            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            using (MySqlCommand comando = new MySqlCommand(sql, conexion))
            {
                comando.Parameters.Add(Db.P("@id", idPrestamo));
                conexion.Open();
                using (MySqlDataReader r = comando.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return new Prestamo(
                        Db.Entero(r, "IdPrestamo"),
                        Db.Entero(r, "IdUsuario"),
                        Db.Entero(r, "IdMaterial"),
                        Db.Fecha(r, "FechaPrestamo"),
                        Db.Fecha(r, "FechaLimite"),
                        Db.FechaONula(r, "FechaDevolucionReal"),
                        Db.TextoONulo(r, "Estado") ?? EstadosPrestamo.Activo,
                        Db.Fecha(r, "FechaRegistro"));
                }
            }
        }

        public DataTable Consultar(string? texto, string? estado, DateTime? desde, DateTime? hasta,
                                   bool soloAbiertos, int? idUsuario)
        {
            Sesion.ExigirSesion();

            if (!Sesion.EsPersonal && Sesion.UsuarioActual != null)
                idUsuario = Sesion.UsuarioActual.Id;

            const string sql = @"
                SELECT p.IdPrestamo AS Id,
                       u.Nombre AS Alumno,
                       u.NumeroEstudiante AS 'No. estudiante',
                       m.Nombre AS Material,
                       m.NumeroSerie AS Serie,
                       p.FechaPrestamo AS Prestamo,
                       p.FechaLimite AS Limite,
                       p.FechaDevolucionReal AS 'Entrega real',
                       p.Estado,
                       GREATEST(DATEDIFF(COALESCE(p.FechaDevolucionReal, @hoy), p.FechaLimite), 0) AS 'Dias de retraso'
                FROM Prestamo p
                INNER JOIN Usuario u ON u.IdUsuario = p.IdUsuario
                INNER JOIN Material m ON m.IdMaterial = p.IdMaterial
                WHERE (u.Nombre LIKE @p OR u.NumeroEstudiante LIKE @p OR u.Correo LIKE @p
                       OR m.Nombre LIKE @p OR m.NumeroSerie LIKE @p)
                  AND (@estado = '' OR p.Estado = @estado)
                  AND (@abiertos = 0 OR p.Estado IN ('Activo', 'Mora'))
                  AND (@usuario = 0 OR p.IdUsuario = @usuario)
                  AND (@usaDesde = 0 OR p.FechaPrestamo >= @desde)
                  AND (@usaHasta = 0 OR p.FechaPrestamo <= @hasta)
                ORDER BY p.FechaPrestamo DESC, p.IdPrestamo DESC;";

            return Db.Tabla(sql,
                Db.P("@hoy", DateTime.Today),
                Db.P("@p", "%" + (texto ?? string.Empty).Trim() + "%"),
                Db.P("@estado", estado ?? string.Empty),
                Db.P("@abiertos", soloAbiertos ? 1 : 0),
                Db.P("@usuario", idUsuario ?? 0),
                Db.P("@usaDesde", desde.HasValue ? 1 : 0),
                Db.P("@desde", desde.HasValue ? desde.Value.Date : DateTime.Today),
                Db.P("@usaHasta", hasta.HasValue ? 1 : 0),
                Db.P("@hasta", hasta.HasValue ? hasta.Value.Date : DateTime.Today));
        }

        private static MySqlCommand Comando(MySqlConnection conexion, MySqlTransaction transaccion,
                                            string sql, params MySqlParameter[] parametros)
        {
            MySqlCommand comando = new MySqlCommand(sql, conexion, transaccion);
            comando.Parameters.AddRange(parametros);
            return comando;
        }
    }
}
