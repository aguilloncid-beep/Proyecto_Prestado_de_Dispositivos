using System;
using SistemaPresatamos.Models;

namespace SistemaPresatamos.Seguridad
{
    public static class Sesion
    {
        public static Usuario? UsuarioActual { get; private set; }

        public static bool CerrarSesionSolicitada { get; set; }

        public static bool HayUsuario { get { return UsuarioActual != null || SesionSistema.Autenticada; } }

        public static bool EsAdministrador
        {
            get
            {
                if (UsuarioActual != null && UsuarioActual.EsActivo && UsuarioActual.Rol == Roles.Administrador)
                    return true;

                return SesionSistema.Autenticada && SesionSistema.RolActivo == RolesSistema.Administrador;
            }
        }

        public static bool EsOperador
        {
            get { return SesionSistema.Autenticada && SesionSistema.RolActivo == RolesSistema.Operador; }
        }

        public static bool EsPersonal { get { return EsAdministrador || EsOperador; } }

        public static void Iniciar(Usuario usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            UsuarioActual = usuario;
            CerrarSesionSolicitada = false;
        }

        public static void Cerrar()
        {
            UsuarioActual = null;
            SesionSistema.Cerrar();
        }

        public static void ExigirAdministrador()
        {
            if (!EsAdministrador)
                throw new UnauthorizedAccessException("Esta función es exclusiva del administrador.");
        }

        public static void ExigirPersonal()
        {
            if (!EsPersonal)
                throw new UnauthorizedAccessException("Esta función requiere el perfil Administrador u Operador.");
        }

        public static void ExigirSesion()
        {
            if (!HayUsuario)
                throw new UnauthorizedAccessException("Debe iniciar sesión.");
        }
    }
}
