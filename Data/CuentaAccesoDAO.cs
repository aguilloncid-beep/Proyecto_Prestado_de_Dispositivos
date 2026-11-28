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
    public class CuentaAccesoDAO
    {
        // Búsqueda exacta por nombre de usuario (consulta parametrizada).
        public CuentaAcceso? ObtenerPorUsuario(string nombreUsuario)
        {
            const string sql =
                "SELECT IdCuenta, NombreUsuario, Contrasena, NombreCompleto, Rol, Estado FROM CuentaAcceso WHERE NombreUsuario = @usuario LIMIT 1;";

            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            using (MySqlCommand comando = new MySqlCommand(sql, conexion))
            {
                comando.Parameters.Add(Db.P("@usuario", nombreUsuario));
                conexion.Open();
                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read()) return null;

                    return new CuentaAcceso
                    {
                        IdCuenta = Db.Entero(lector, "IdCuenta"),
                        NombreUsuario = Db.TextoONulo(lector, "NombreUsuario") ?? string.Empty,
                        Contrasena = Db.TextoONulo(lector, "Contrasena") ?? string.Empty,
                        NombreCompleto = Db.TextoONulo(lector, "NombreCompleto") ?? string.Empty,
                        Rol = Db.TextoONulo(lector, "Rol") ?? string.Empty,
                        Estado = Db.TextoONulo(lector, "Estado") ?? CuentaAcceso.EstadoInactivo
                    };
                }
            }
        }

        // Cuentas iniciales: solo si la tabla está vacía. Se ejecuta antes del login, por eso no exige sesión.
        public void AsegurarCuentasIniciales()
        {
            if (Db.Escalar("SELECT COUNT(*) FROM CuentaAcceso;") > 0)
                return;

            const string sql = @"
                INSERT INTO CuentaAcceso (NombreUsuario, Contrasena, NombreCompleto, Rol, Estado)
                VALUES (@usuario, @hash, @nombre, @rol, 'Activo');";

            string[][] iniciales =
            {
                new[] { "admin", "admin123", "Administrador del Sistema", RolesSistema.Administrador },
                new[] { "operador", "Oper123", "Operador de Préstamos", RolesSistema.Operador },
                new[] { "invitado", "Invit123", "Invitado de Consulta", RolesSistema.Invitado }
            };

            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();
                foreach (string[] cuenta in iniciales)
                {
                    using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                    {
                        comando.Parameters.Add(Db.P("@usuario", cuenta[0]));
                        comando.Parameters.Add(Db.P("@hash", PasswordHasher.Generar(cuenta[1])));
                        comando.Parameters.Add(Db.P("@nombre", cuenta[2]));
                        comando.Parameters.Add(Db.P("@rol", cuenta[3]));
                        comando.ExecuteNonQuery();
                    }
                }
            }
        }

        public DataTable Listar()
        {
            Sesion.ExigirAdministrador();

            return Db.Tabla(
                "SELECT IdCuenta AS Id, NombreUsuario AS Usuario, NombreCompleto AS Nombre, Rol, Estado FROM CuentaAcceso ORDER BY IdCuenta;");
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A20 - Alta de cuentas (solo Administrador): INSERT parametrizado con la contraseña ya convertida en hash.
        // =====================================================================
        public int Crear(string nombreUsuario, string nombreCompleto, string rol, string password)
        {
            Sesion.ExigirAdministrador();

            ValidarDatos(nombreUsuario, nombreCompleto, rol);
            if (!PasswordHasher.CumpleRequisitos(password, out string mensaje))
                throw new ArgumentException(mensaje);

            const string sql = @"
                INSERT INTO CuentaAcceso (NombreUsuario, Contrasena, NombreCompleto, Rol, Estado)
                VALUES (@usuario, @hash, @nombre, @rol, 'Activo');";

            try
            {
                using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.Add(Db.P("@usuario", nombreUsuario.Trim()));
                    comando.Parameters.Add(Db.P("@hash", PasswordHasher.Generar(password)));
                    comando.Parameters.Add(Db.P("@nombre", nombreCompleto.Trim()));
                    comando.Parameters.Add(Db.P("@rol", rol));
                    conexion.Open();
                    comando.ExecuteNonQuery();
                    return (int)comando.LastInsertedId;
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new InvalidOperationException("Ya existe una cuenta con ese nombre de usuario.");
            }
        }

        // A20 - Modificación de cuentas (solo Administrador). Contraseña vacía = conservar la actual.
        public void Actualizar(int idCuenta, string nombreCompleto, string rol, string estado, string? password)
        {
            Sesion.ExigirAdministrador();

            ValidarDatos("x", nombreCompleto, rol);
            if (estado != CuentaAcceso.EstadoActivo && estado != CuentaAcceso.EstadoInactivo)
                throw new ArgumentException("Estado no válido.");

            bool cambiaPassword = !string.IsNullOrEmpty(password);
            if (cambiaPassword && !PasswordHasher.CumpleRequisitos(password, out string mensaje))
                throw new ArgumentException(mensaje);

            if (idCuenta == SesionSistema.IdUsuario &&
                (estado == CuentaAcceso.EstadoInactivo || rol != RolesSistema.Administrador))
                throw new InvalidOperationException("No puede desactivar ni quitarle el rol de Administrador a la cuenta con la que inició sesión.");

            string sql = "UPDATE CuentaAcceso SET NombreCompleto = @nombre, Rol = @rol, Estado = @estado" +
                         (cambiaPassword ? ", Contrasena = @hash" : "") + " WHERE IdCuenta = @id;";

            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            using (MySqlCommand comando = new MySqlCommand(sql, conexion))
            {
                comando.Parameters.Add(Db.P("@nombre", nombreCompleto.Trim()));
                comando.Parameters.Add(Db.P("@rol", rol));
                comando.Parameters.Add(Db.P("@estado", estado));
                comando.Parameters.Add(Db.P("@id", idCuenta));
                if (cambiaPassword)
                    comando.Parameters.Add(Db.P("@hash", PasswordHasher.Generar(password!)));

                conexion.Open();
                if (comando.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("La cuenta no existe o no hubo cambios.");
            }
        }

        private static void ValidarDatos(string nombreUsuario, string nombreCompleto, string rol)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || nombreUsuario.Trim().Length > 50 || nombreUsuario.Trim().Contains(' '))
                throw new ArgumentException("El usuario es obligatorio, sin espacios y de máximo 50 caracteres.");
            if (string.IsNullOrWhiteSpace(nombreCompleto) || nombreCompleto.Trim().Length > 120)
                throw new ArgumentException("El nombre es obligatorio y de máximo 120 caracteres.");
            if (rol != RolesSistema.Administrador && rol != RolesSistema.Operador && rol != RolesSistema.Invitado)
                throw new ArgumentException("Seleccione un rol válido.");
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A20 - Gestión de usuarios (solo Administrador): UPDATE del Estado. Una cuenta no puede
        //  desactivarse a sí misma para evitar dejar el sistema sin acceso.
        // =====================================================================
        public void CambiarEstado(int idCuenta, string estado)
        {
            Sesion.ExigirAdministrador();

            if (estado != CuentaAcceso.EstadoActivo && estado != CuentaAcceso.EstadoInactivo)
                throw new ArgumentException("Estado no válido.");

            if (estado == CuentaAcceso.EstadoInactivo && idCuenta == SesionSistema.IdUsuario)
                throw new InvalidOperationException("No puede desactivar la cuenta con la que inició sesión.");

            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            using (MySqlCommand comando = new MySqlCommand("UPDATE CuentaAcceso SET Estado = @estado WHERE IdCuenta = @id;", conexion))
            {
                comando.Parameters.Add(Db.P("@estado", estado));
                comando.Parameters.Add(Db.P("@id", idCuenta));
                conexion.Open();

                if (comando.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("La cuenta no existe.");
            }
        }
    }
}
