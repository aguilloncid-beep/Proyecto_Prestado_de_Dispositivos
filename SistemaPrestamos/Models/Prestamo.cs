using System;
using System.Collections.Generic;

namespace SistemaPresatamos.Models
{
    internal class Prestamo : EntidadBase, IAlmacenamientoCRUD
    {
        private static List<Prestamo> _tabla = new List<Prestamo>();

        private Stack<Prestamo> _historialCambios;

        public Stack<Prestamo> HistorialCambios
        {
            get { return _historialCambios; }
        }

        private DateTime _fechaPrestamo = DateTime.Now;
        private DateTime _fechaDevolucion = DateTime.Now.AddDays(3);
        private string _rutaImagen = "default_prestamo.png";

        public Prestamo()
            : base()
        {
            _historialCambios = new Stack<Prestamo>();
        }

        public Prestamo(int id, DateTime fechaPrestamo, DateTime fechaDevolucion,
                        string rutaImagen, DateTime fechaRegistro, bool esActivo)
            : base(id, fechaRegistro, esActivo)
        {
            _historialCambios = new Stack<Prestamo>();
            FechaPrestamo = fechaPrestamo;
            FechaDevolucion = fechaDevolucion;
            RutaImagen = rutaImagen;
        }

        public int IdUsuario { get; set; }
        public int IdElemento { get; set; }

        public DateTime FechaPrestamo
        {
            get { return _fechaPrestamo; }
            set { _fechaPrestamo = value; }
        }

        public DateTime FechaDevolucion
        {
            get { return _fechaDevolucion; }
            set
            {
                if (value < _fechaPrestamo)
                    throw new ArgumentException("La fecha de devolución no puede ser anterior a la fecha de préstamo.");

                _fechaDevolucion = value;
            }
        }

        public string RutaImagen
        {
            get { return _rutaImagen; }
            set
            {
                _rutaImagen = string.IsNullOrWhiteSpace(value)
                    ? "default_prestamo.png"
                    : value.Trim();
            }
        }

        public override string ToString()
        {
            return "Préstamo #" + Id +
                   " | Usuario: " + IdUsuario +
                   " | Elemento: " + IdElemento +
                   " | Fecha: " + _fechaPrestamo.ToShortDateString() +
                   " | Entrega: " + _fechaDevolucion.ToShortDateString();
        }

        public bool VerificarVencimiento()
        {
            if (!this.EsActivo)
                return false;

            return DateTime.Now > this._fechaDevolucion;
        }

        public bool VerificarVencimiento(DateTime fechaCorteExterna)
        {
            if (!this.EsActivo)
                return false;

            return fechaCorteExterna > this._fechaDevolucion;
        }

        // 1 Push
        public void ApilarAccion(Prestamo elemento)
        {
            if (elemento == null)
                throw new ArgumentNullException("elemento");

            _historialCambios.Push(CrearCopia(elemento));
        }

        // 2 Pop
        public Prestamo DesapilarYRevertir()
        {
            if (_historialCambios.Count == 0)
                return null;

            Prestamo elemento = _historialCambios.Pop();

            EliminarRegistro(elemento.Id.ToString()); 

            return elemento;
        }

        // 3 Peek
        public Prestamo InspeccionarCima()
        {
            if (_historialCambios.Count == 0)
                return null;

            return _historialCambios.Peek();
        }

        // 4 Count
        public int ContarHistorial()
        {
            return _historialCambios.Count;
        }

        // 5 foreach
        public bool ValidarExistenciaEstado(int id)
        {
            foreach (Prestamo elemento in _historialCambios)
            {
                if (elemento.Id == id)
                    return true;
            }

            return false;
        }

        // 6 Clear
        public void VaciarHistorial()
        {
            _historialCambios.Clear();
        }

        // 7 VOLCADO 
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

        // Crea una copia independiente del prestamo para guardar el istorial
        private Prestamo CrearCopia(Prestamo original)
        {
            Prestamo copia = new Prestamo(
                original.Id,
                original.FechaPrestamo,
                original.FechaDevolucion,
                original.RutaImagen,
                original.FechaRegistro,
                original.EsActivo
            );

            copia.IdUsuario = original.IdUsuario;
            copia.IdElemento = original.IdElemento;

            return copia;
        }

        public void InsertarRegistro(object objeto)
        {
            Prestamo prestamo = (Prestamo)objeto;
            _tabla.Add(prestamo);
        }

        public object ConsultarRegistro(string id)
        {
            int idBuscado = int.Parse(id);

            foreach (Prestamo prestamo in _tabla)
            {
                if (prestamo.Id == idBuscado)
                    return prestamo;
            }

            return null;
        }

        public void ActualizarRegistro(object objeto)
        {
            Prestamo actualizacion = (Prestamo)objeto;
            Prestamo existente = (Prestamo)ConsultarRegistro(actualizacion.Id.ToString());

            if (existente != null)
            {
                existente.IdUsuario = actualizacion.IdUsuario;
                existente.IdElemento = actualizacion.IdElemento;
                existente.FechaPrestamo = actualizacion.FechaPrestamo;
                existente.FechaDevolucion = actualizacion.FechaDevolucion;
                existente.RutaImagen = actualizacion.RutaImagen;
                existente.EsActivo = actualizacion.EsActivo;
            }
        }

        public void EliminarRegistro(string id)
        {
            Prestamo existente = (Prestamo)ConsultarRegistro(id);

            if (existente != null)
                _tabla.Remove(existente);
        }
    }
}




