// =====================================================================
//  Autoría - Equipo 4
//  Integrantes en orden alfabético por primer apellido
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;

namespace SistemaPresatamos.Models
{
    public class PrestamoVista
    {
        public int IdPrestamo { get; set; }
        public string? RutaImagen { get; set; }

        public string Alumno { get; set; } = string.Empty;
        public string? NumeroEstudiante { get; set; }
        public string? Carrera { get; set; }
        public string Material { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string? NumeroSerie { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaLimite { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string? Observaciones { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
