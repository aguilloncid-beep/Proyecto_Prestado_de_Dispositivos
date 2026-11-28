using System;

namespace SistemaPresatamos.Models
{
    public abstract class EntidadBase
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool EsActivo { get; set; }

        protected EntidadBase()
        {
            Id = 0;
            FechaRegistro = DateTime.Now;
            EsActivo = true;
        }

        protected EntidadBase(int id, DateTime fechaRegistro, bool esActivo)
        {
            Id = id;
            FechaRegistro = fechaRegistro;
            EsActivo = esActivo;
        }
    }
}

