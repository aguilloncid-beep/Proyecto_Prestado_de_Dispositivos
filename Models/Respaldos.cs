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
    public interface IRegistroRespaldo
    {
        bool EsValido();
    }

    public class MaterialRespaldo : IRegistroRespaldo
    {
        public MaterialRespaldo() { }

        public int IdMaterial { get; set; }
        public string? NumeroSerie { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? RutaImagen { get; set; }

        public bool EsValido()
        {
            return IdMaterial > 0 && !string.IsNullOrWhiteSpace(Nombre) && EstadosMaterial.EsValido(Estado);
        }
    }

    public class PrestamoRespaldo : IRegistroRespaldo
    {
        public PrestamoRespaldo() { }

        public int IdPrestamo { get; set; }
        public string Alumno { get; set; } = string.Empty;
        public string? NumeroEstudiante { get; set; }
        public string Material { get; set; } = string.Empty;
        public string? Serie { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaLimite { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int DiasRetraso { get; set; }

        public bool EsValido()
        {
            return IdPrestamo > 0 && !string.IsNullOrWhiteSpace(Alumno) && !string.IsNullOrWhiteSpace(Material);
        }
    }
}
