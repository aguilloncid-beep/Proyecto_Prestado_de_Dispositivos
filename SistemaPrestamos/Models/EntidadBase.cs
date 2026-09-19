using System;

namespace SistemaPresatamos.Models
{
    public abstract class EntidadBase
    {
        // Atributos heredables comunes para todos los modelos
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool EsActivo { get; set; }

        // Constructor por defecto
        protected EntidadBase()
        {
            Id = 0;
            FechaRegistro = DateTime.Now;
            EsActivo = true;
        }

        // Constructor parametrizado
        protected EntidadBase(int id, DateTime fechaRegistro, bool esActivo)
        {
            Id = id;
            FechaRegistro = fechaRegistro;
            EsActivo = esActivo;
        }
    }
}


