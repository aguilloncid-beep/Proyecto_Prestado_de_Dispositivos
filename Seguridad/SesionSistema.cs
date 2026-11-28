using System;

namespace SistemaPresatamos.Seguridad
{
    public static class SesionSistema
    {
        public static string UsuarioActivo { get; private set; } = string.Empty;

        public static string RolActivo { get; private set; } = string.Empty;

        public static DateTime HoraInicioSesion { get; private set; }

        public static bool Autenticada { get; private set; }

        public static int IdUsuario { get; private set; }

        public static string NombreUsuario { get; private set; } = string.Empty;

        // A20 - Objeto de sesión global: además del rol conserva el Id y el nombre de usuario de la cuenta.
        public static void Iniciar(int idUsuario, string nombreUsuario, string nombreCompleto, string rol)
        {
            Iniciar(nombreCompleto, rol);
            IdUsuario = idUsuario;
            NombreUsuario = nombreUsuario;
        }

        public static void Iniciar(string nombreCompleto, string rol)
        {
            UsuarioActivo = nombreCompleto;
            RolActivo = rol;
            HoraInicioSesion = DateTime.Now;
            Autenticada = true;
        }

        public static void Cerrar()
        {
            IdUsuario = 0;
            NombreUsuario = string.Empty;
            UsuarioActivo = string.Empty;
            RolActivo = string.Empty;
            HoraInicioSesion = default(DateTime);
            Autenticada = false;
        }
    }
}
