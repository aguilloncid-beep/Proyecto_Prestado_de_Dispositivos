using System;
using System.Data;
using MySql.Data.MySqlClient;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos.Data
{
    public class UsuarioDAO
    {
        private const string Columnas =
            "IdUsuario, Nombre, Correo, NumeroEstudiante, Carrera, PasswordHash, Rol, Activo, FechaRegistro";

        private static readonly Lazy<string> HashFicticio = new Lazy<string>(() => PasswordHasher.Generar("relleno-sin-uso-1"));

        public Usuario? Autenticar(string correo, string password)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrEmpty(password))
                return null;

            Usuario? usuario = ObtenerPorCorreo(correo);

            if (usuario == null)
            {
                PasswordHasher.Verificar(password, HashFicticio.Value);
                return null;
            }

            if (!usuario.EsActivo)
                return null;

            return PasswordHasher.Verificar(password, usuario.PasswordHash) ? usuario : null;
        }

        public int RegistrarAlumno(string nombre, string correo, string numeroEstudiante, string? carrera, string password)
        {
            if (string.IsNullOrWhiteSpace(numeroEstudiante))
                throw new ArgumentException("El número de estudiante es obligatorio.");

            ValidarPassword(password);

            Usuario nuevo = new Usuario();
            nuevo.Nombre = nombre;
            nuevo.Correo = correo;
            nuevo.NumeroEstudiante = numeroEstudiante;
            nuevo.Carrera = carrera;
            nuevo.Rol = Roles.Alumno;
            nuevo.PasswordHash = PasswordHasher.Generar(password);
            nuevo.EsActivo = true;

            return InsertarInterno(nuevo);
        }

        public int Insertar(Usuario usuario, string password)
        {
            Sesion.ExigirAdministrador();
            ValidarDatosUsuario(usuario);
            ValidarPassword(password);

            usuario.PasswordHash = PasswordHasher.Generar(password);
            return InsertarInterno(usuario);
        }

        private int InsertarInterno(Usuario u)
        {
            const string sql = @"
                INSERT INTO Usuario (Nombre, Correo, NumeroEstudiante, Carrera, PasswordHash, Rol, Activo)
                VALUES (@nombre, @correo, @numero, @carrera, @hash, @rol, @activo);";

            try
            {
                using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.Add(Db.P("@nombre", u.Nombre));
                    comando.Parameters.Add(Db.P("@correo", u.Correo));
                    comando.Parameters.Add(Db.P("@numero", u.NumeroEstudiante));
                    comando.Parameters.Add(Db.P("@carrera", u.Carrera));
                    comando.Parameters.Add(Db.P("@hash", u.PasswordHash));
                    comando.Parameters.Add(Db.P("@rol", u.Rol));
                    comando.Parameters.Add(Db.P("@activo", u.EsActivo ? 1 : 0));

                    conexion.Open();
                    comando.ExecuteNonQuery();
                    u.Id = (int)comando.LastInsertedId;
                    return u.Id;
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new InvalidOperationException(MensajeDuplicado(ex.Message));
            }
        }

        public Usuario? ObtenerPorId(int id)
        {
            return ConsultarUno("SELECT " + Columnas + " FROM Usuario WHERE IdUsuario = @id LIMIT 1;",
                                Db.P("@id", id));
        }

        public Usuario? ObtenerPorCorreo(string correo)
        {
            return ConsultarUno("SELECT " + Columnas + " FROM Usuario WHERE Correo = @correo LIMIT 1;",
                                Db.P("@correo", correo.Trim().ToLowerInvariant()));
        }

        public Usuario? Buscar(string texto, bool incluirInactivos)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return null;

            string limpio = texto.Trim();
            int id = int.TryParse(limpio, out int n) ? n : -1;

            const string sql = "SELECT " + Columnas + @"
                FROM Usuario
                WHERE (Correo = @correo OR NumeroEstudiante = @texto OR IdUsuario = @id)
                  AND (@incluir = 1 OR Activo = 1)
                ORDER BY CASE WHEN Correo = @correo OR NumeroEstudiante = @texto THEN 0 ELSE 1 END
                LIMIT 1;";

            return ConsultarUno(sql,
                Db.P("@correo", limpio.ToLowerInvariant()),
                Db.P("@texto", limpio),
                Db.P("@id", id),
                Db.P("@incluir", incluirInactivos ? 1 : 0));
        }

        public DataTable Listar(string? filtro)
        {
            Sesion.ExigirAdministrador();

            string patron = "%" + (filtro ?? string.Empty).Trim() + "%";
            const string sql = @"
                SELECT IdUsuario AS Id, Nombre, Correo, NumeroEstudiante AS 'No. estudiante',
                       Carrera, Rol, IF(Activo = 1, 'Sí', 'No') AS Activo, FechaRegistro AS Registro
                FROM Usuario
                WHERE Nombre LIKE @p OR Correo LIKE @p OR NumeroEstudiante LIKE @p
                ORDER BY Nombre;";
            return Db.Tabla(sql, Db.P("@p", patron));
        }

        public int ContarAdministradoresActivos()
        {
            return (int)Db.Escalar("SELECT COUNT(*) FROM Usuario WHERE Rol = 'Administrador' AND Activo = 1;");
        }

        public int ContarPrestamosAbiertos(int idUsuario)
        {
            return (int)Db.Escalar(
                "SELECT COUNT(*) FROM Prestamo WHERE IdUsuario = @id AND Estado IN ('Activo', 'Mora');",
                Db.P("@id", idUsuario));
        }

        public void Actualizar(Usuario usuario, string? nuevaPassword)
        {
            Sesion.ExigirAdministrador();
            ValidarDatosUsuario(usuario);

            Usuario? actual = ObtenerPorId(usuario.Id);
            if (actual == null)
                throw new InvalidOperationException("El usuario no existe.");

            bool dejaDeSerAdminActivo = actual.EsAdministrador && actual.EsActivo &&
                                        (!usuario.EsAdministrador || !usuario.EsActivo);
            if (dejaDeSerAdminActivo && ContarAdministradoresActivos() <= 1)
                throw new InvalidOperationException("No se puede quitar o desactivar al único administrador activo.");

            if (!usuario.EsActivo && ContarPrestamosAbiertos(usuario.Id) > 0)
                throw new InvalidOperationException("El usuario tiene préstamos abiertos; registre primero su devolución.");

            bool cambiaPassword = !string.IsNullOrEmpty(nuevaPassword);
            if (cambiaPassword)
            {
                ValidarPassword(nuevaPassword!);
                usuario.PasswordHash = PasswordHasher.Generar(nuevaPassword!);
            }

            string sql = @"
                UPDATE Usuario
                SET Nombre = @nombre, Correo = @correo, NumeroEstudiante = @numero, Carrera = @carrera,
                    Rol = @rol, Activo = @activo" + (cambiaPassword ? ", PasswordHash = @hash" : "") + @"
                WHERE IdUsuario = @id;";

            try
            {
                using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.Add(Db.P("@nombre", usuario.Nombre));
                    comando.Parameters.Add(Db.P("@correo", usuario.Correo));
                    comando.Parameters.Add(Db.P("@numero", usuario.NumeroEstudiante));
                    comando.Parameters.Add(Db.P("@carrera", usuario.Carrera));
                    comando.Parameters.Add(Db.P("@rol", usuario.Rol));
                    comando.Parameters.Add(Db.P("@activo", usuario.EsActivo ? 1 : 0));
                    comando.Parameters.Add(Db.P("@id", usuario.Id));
                    if (cambiaPassword)
                        comando.Parameters.Add(Db.P("@hash", usuario.PasswordHash));

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new InvalidOperationException(MensajeDuplicado(ex.Message));
            }
        }

        public void Desactivar(int idUsuario)
        {
            Sesion.ExigirAdministrador();

            Usuario? actual = ObtenerPorId(idUsuario);
            if (actual == null)
                throw new InvalidOperationException("El usuario no existe.");

            actual.EsActivo = false;
            Actualizar(actual, null);
        }

        private Usuario? ConsultarUno(string sql, params MySqlParameter[] parametros)
        {
            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            using (MySqlCommand comando = new MySqlCommand(sql, conexion))
            {
                comando.Parameters.AddRange(parametros);
                conexion.Open();
                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    return lector.Read() ? Mapear(lector) : null;
                }
            }
        }

        private static Usuario Mapear(IDataRecord r)
        {
            return new Usuario(
                Db.Entero(r, "IdUsuario"),
                Db.TextoONulo(r, "Nombre") ?? string.Empty,
                Db.TextoONulo(r, "Correo") ?? string.Empty,
                Db.TextoONulo(r, "NumeroEstudiante"),
                Db.TextoONulo(r, "Carrera"),
                Db.TextoONulo(r, "PasswordHash") ?? string.Empty,
                Db.TextoONulo(r, "Rol") ?? Roles.Alumno,
                Db.Fecha(r, "FechaRegistro"),
                Db.Booleano(r, "Activo"));
        }

        private static void ValidarDatosUsuario(Usuario u)
        {
            if (u == null) throw new ArgumentNullException(nameof(u));
            if (u.Rol == Roles.Alumno && string.IsNullOrWhiteSpace(u.NumeroEstudiante))
                throw new ArgumentException("El número de estudiante es obligatorio para los alumnos.");
        }

        private static void ValidarPassword(string password)
        {
            if (!PasswordHasher.CumpleRequisitos(password, out string mensaje))
                throw new ArgumentException(mensaje);
        }

        private static string MensajeDuplicado(string mensajeMotor)
        {
            if (mensajeMotor.IndexOf("Correo", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Ya existe un usuario registrado con ese correo.";
            if (mensajeMotor.IndexOf("NumeroEstudiante", StringComparison.OrdinalIgnoreCase) >= 0)
                return "Ya existe un usuario con ese número de estudiante.";
            return "Ya existe un registro con esos datos.";
        }
    }
}
