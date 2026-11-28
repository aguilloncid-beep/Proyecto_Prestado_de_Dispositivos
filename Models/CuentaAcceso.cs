// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;

namespace SistemaPresatamos.Models
{
    // A20 - Modelo de usuario de acceso: identificador, nombre de usuario, contraseña (hash PBKDF2) y Estado.
    public class CuentaAcceso
    {
        public const string EstadoActivo = "Activo";
        public const string EstadoInactivo = "Inactivo";

        public int IdCuenta { get; set; }

        public string NombreUsuario { get; set; } = string.Empty;

        public string Contrasena { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;

        public string Estado { get; set; } = EstadoActivo;

        public bool EstaActiva
        {
            get { return string.Equals(Estado, EstadoActivo, StringComparison.OrdinalIgnoreCase); }
        }
    }
}
