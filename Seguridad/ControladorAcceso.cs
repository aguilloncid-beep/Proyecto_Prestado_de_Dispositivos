// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using MySql.Data.MySqlClient;
using SistemaPresatamos.Data;
using SistemaPresatamos.Models;

namespace SistemaPresatamos.Seguridad
{
    public static class ControladorAcceso
    {
        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A20 - Autenticación. Recibe las credenciales de la vista y las busca en la base de datos.
        //  Orden de validación: 1) existencia del usuario, 2) ESTADO (si está Inactivo se niega el acceso con un
        //  mensaje específico), 3) contraseña. Si todo es correcto carga el objeto de sesión global
        //  (SesionSistema) con el Id del usuario y su rol; no solo devuelve un valor booleano.
        // =====================================================================
        public static bool Validar(string? usuario, string? contrasena, out string mensaje)
        {
            mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrEmpty(contrasena))
            {
                mensaje = "Escriba el usuario y la contraseña.";
                return false;
            }

            string nombre = usuario.Trim();
            CuentaAcceso? cuenta;

            try
            {
                CuentaAccesoDAO dao = new CuentaAccesoDAO();
                dao.AsegurarCuentasIniciales();
                cuenta = dao.ObtenerPorUsuario(nombre);
            }
            catch (MySqlException)
            {
                return ValidarSinConexion(nombre, contrasena, out mensaje);
            }

            if (cuenta == null)
            {
                mensaje = "Usuario o contraseña incorrectos.";
                return false;
            }

            if (!cuenta.EstaActiva)
            {
                mensaje = "La cuenta «" + cuenta.NombreUsuario + "» está INACTIVA. Contacte al administrador para reactivarla.";
                return false;
            }

            if (!PasswordHasher.Verificar(contrasena, cuenta.Contrasena))
            {
                mensaje = "Usuario o contraseña incorrectos.";
                return false;
            }

            SesionSistema.Iniciar(cuenta.IdCuenta, cuenta.NombreUsuario, cuenta.NombreCompleto, cuenta.Rol);
            return true;
        }

        // Respaldo cuando MariaDB no está disponible o aún no se ejecutó la migración de CuentaAcceso.
        private static bool ValidarSinConexion(string nombre, string contrasena, out string mensaje)
        {
            mensaje = string.Empty;

            if (UsuariosSistema.Validar(nombre, contrasena, out UsuarioEstatico? encontrado) && encontrado != null)
            {
                SesionSistema.Iniciar(0, encontrado.NombreUsuario, encontrado.NombreCompleto, encontrado.Rol);
                return true;
            }

            mensaje = "Usuario o contraseña incorrectos.";
            return false;
        }
    }
}
