using System;
using System.Collections.Generic;

namespace SistemaPresatamos.Seguridad
{
    public static class RolesSistema
    {
        public const string Administrador = "Administrador";
        public const string Operador = "Operador";
        public const string Invitado = "Invitado";
    }

    public class UsuarioEstatico
    {
        public UsuarioEstatico(string nombreUsuario, string contrasena, string nombreCompleto, string rol)
        {
            NombreUsuario = nombreUsuario;
            Contrasena = contrasena;
            NombreCompleto = nombreCompleto;
            Rol = rol;
        }

        public string NombreUsuario { get; private set; }
        public string Contrasena { get; private set; }
        public string NombreCompleto { get; private set; }
        public string Rol { get; private set; }
    }

    public static class UsuariosSistema
    {
        private static readonly List<UsuarioEstatico> _usuarios = new List<UsuarioEstatico>
        {
            new UsuarioEstatico("admin",    "admin123", "Administrador del Sistema", RolesSistema.Administrador),
            new UsuarioEstatico("operador", "Oper123",  "Operador de Préstamos",     RolesSistema.Operador),
            new UsuarioEstatico("invitado", "Invit123", "Invitado de Consulta",      RolesSistema.Invitado)
        };

        public static IReadOnlyList<UsuarioEstatico> Usuarios
        {
            get { return _usuarios; }
        }

        public static bool Validar(string? usuario, string? contrasena, out UsuarioEstatico? encontrado)
        {
            encontrado = null;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrEmpty(contrasena))
                return false;

            string nombre = usuario.Trim();

            foreach (UsuarioEstatico candidato in _usuarios)
            {
                if (string.Equals(candidato.NombreUsuario, nombre, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(candidato.Contrasena, contrasena, StringComparison.Ordinal))
                {
                    encontrado = candidato;
                    return true;
                }
            }

            return false;
        }
    }
}
