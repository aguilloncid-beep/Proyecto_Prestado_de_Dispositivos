using System;
using System.Collections.Generic;

namespace SistemaPresatamos.Models
{
    public class Prestamo : EntidadBase
    {
        private readonly Stack<Prestamo> _historialCambios = new Stack<Prestamo>();

        private DateTime _fechaPrestamo = DateTime.Today;
        private DateTime _fechaLimite = DateTime.Today.AddDays(3);
        private DateTime? _fechaDevolucionReal;
        private string _estado = EstadosPrestamo.Activo;

        public Prestamo() : base() { }

        public Prestamo(int id, int idUsuario, int idMaterial, DateTime fechaPrestamo, DateTime fechaLimite,
                        DateTime? fechaDevolucionReal, string estado, DateTime fechaRegistro)
            : base(id, fechaRegistro, true)
        {
            IdUsuario = idUsuario;
            IdMaterial = idMaterial;
            _fechaPrestamo = fechaPrestamo.Date;
            FechaLimite = fechaLimite;
            _fechaDevolucionReal = fechaDevolucionReal?.Date;
            Estado = estado;
        }

        public int IdUsuario { get; set; }

        public int IdMaterial { get; set; }

        public DateTime FechaPrestamo
        {
            get { return _fechaPrestamo; }
            set { _fechaPrestamo = value.Date; }
        }

        public DateTime FechaLimite
        {
            get { return _fechaLimite; }
            set
            {
                if (value.Date < _fechaPrestamo)
                    throw new ArgumentException("La fecha límite no puede ser anterior a la fecha de préstamo.");
                _fechaLimite = value.Date;
            }
        }

        public DateTime? FechaDevolucionReal
        {
            get { return _fechaDevolucionReal; }
            set { _fechaDevolucionReal = value?.Date; }
        }

        public string Estado
        {
            get { return _estado; }
            set
            {
                if (!EstadosPrestamo.EsValido(value))
                    throw new ArgumentException("Estado de préstamo inválido: " + value);
                _estado = value;
                EsActivo = value != EstadosPrestamo.Finalizado;
            }
        }

        public bool EstaAbierto { get { return _estado != EstadosPrestamo.Finalizado; } }

        public override string ToString()
        {
            string real = _fechaDevolucionReal.HasValue ? _fechaDevolucionReal.Value.ToShortDateString() : "pendiente";
            return "Préstamo #" + Id +
                   " | Usuario: " + IdUsuario +
                   " | Material: " + IdMaterial +
                   " | Préstamo: " + _fechaPrestamo.ToShortDateString() +
                   " | Límite: " + _fechaLimite.ToShortDateString() +
                   " | Entrega real: " + real +
                   " | Estado: " + _estado;
        }

        public bool VerificarVencimiento()
        {
            return VerificarVencimiento(DateTime.Today);
        }

        public bool VerificarVencimiento(DateTime fechaCorteExterna)
        {
            if (!EstaAbierto) return false;
            return fechaCorteExterna.Date > _fechaLimite;
        }

        public int CalcularDiasRetraso()
        {
            DateTime referencia = _fechaDevolucionReal ?? DateTime.Today;
            int dias = (referencia.Date - _fechaLimite).Days;
            return dias > 0 ? dias : 0;
        }

        public bool SeEntregoConRetraso()
        {
            return _fechaDevolucionReal.HasValue && _fechaDevolucionReal.Value > _fechaLimite;
        }

        public void MarcarDevuelto(DateTime fechaReal)
        {
            if (!EstaAbierto) throw new InvalidOperationException("El préstamo ya fue finalizado.");
            if (fechaReal.Date < _fechaPrestamo)
                throw new ArgumentException("La fecha real no puede ser anterior a la fecha de préstamo.");
            _fechaDevolucionReal = fechaReal.Date;
            Estado = EstadosPrestamo.Finalizado;
        }

        public Stack<Prestamo> HistorialCambios { get { return _historialCambios; } }

        public void ApilarAccion(Prestamo elemento)
        {
            if (elemento == null) throw new ArgumentNullException(nameof(elemento));
            _historialCambios.Push(CrearCopia(elemento));
        }

        public Prestamo? DesapilarYRevertir()
        {
            if (_historialCambios.Count == 0) return null;
            return _historialCambios.Pop();
        }

        public Prestamo? InspeccionarCima()
        {
            if (_historialCambios.Count == 0) return null;
            return _historialCambios.Peek();
        }

        public int ContarHistorial()
        {
            return _historialCambios.Count;
        }

        public bool ValidarExistenciaEstado(int id)
        {
            foreach (Prestamo elemento in _historialCambios)
            {
                if (elemento.Id == id) return true;
            }
            return false;
        }

        public void VaciarHistorial()
        {
            _historialCambios.Clear();
        }

        public Prestamo[] VolcadoAArregloLineal()
        {
            Prestamo[] resultado = new Prestamo[_historialCambios.Count];
            int posicion = 0;
            foreach (Prestamo elemento in _historialCambios)
            {
                resultado[posicion] = CrearCopia(elemento);
                posicion++;
            }
            return resultado;
        }

        private static Prestamo CrearCopia(Prestamo o)
        {
            return new Prestamo(o.Id, o.IdUsuario, o.IdMaterial, o.FechaPrestamo, o.FechaLimite,
                                o.FechaDevolucionReal, o.Estado, o.FechaRegistro);
        }
    }
}
