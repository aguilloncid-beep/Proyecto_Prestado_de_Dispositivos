using System;

namespace SistemaPresatamos.Models
{
    public class Devolucion : EntidadBase
    {
        private DateTime _fechaReal = DateTime.Today;
        private string? _observaciones;

        public Devolucion() : base() { }

        public Devolucion(int id, int idPrestamo, DateTime fechaReal, string? observaciones, DateTime fechaRegistro)
            : base(id, fechaRegistro, true)
        {
            IdPrestamo = idPrestamo;
            FechaReal = fechaReal;
            Observaciones = observaciones;
        }

        public int IdPrestamo { get; set; }

        public DateTime FechaReal
        {
            get { return _fechaReal; }
            set { _fechaReal = value.Date; }
        }

        public string? Observaciones
        {
            get { return _observaciones; }
            set { _observaciones = string.IsNullOrWhiteSpace(value) ? null : value.Trim(); }
        }

        public override string ToString()
        {
            return $"Devolución #{Id} - Préstamo {IdPrestamo} - Entregado el: {_fechaReal.ToShortDateString()}";
        }

        public bool ValidarDevolucionHoy()
        {
            return this._fechaReal.Date == DateTime.Today;
        }

        public bool ValidarDevolucionHoy(DateTime fechaLimiteEsperada)
        {
            return this._fechaReal.Date <= fechaLimiteEsperada.Date;
        }
    }
}
