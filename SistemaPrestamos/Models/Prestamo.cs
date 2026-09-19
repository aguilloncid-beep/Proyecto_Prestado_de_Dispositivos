using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaPresatamos.Models
{
    internal class Prestamo : EntidadBase, IAlmacenamientoCRUD
    {
        private static List<Prestamo> _tabla = new List<Prestamo>();

        private DateTime _fechaPrestamo = DateTime.Now;
        private DateTime _fechaDevolucion = DateTime.Now.AddDays(3);
        private string _rutaImagen = "default_prestamo.png";

        public Prestamo() : base() { }

        public Prestamo(int id, DateTime fechaPrestamo, DateTime fechaDevolucion, string rutaImagen, DateTime fechaRegistro, bool esActivo)
            : base(id, fechaRegistro, esActivo)
        {
            FechaPrestamo = fechaPrestamo;
            FechaDevolucion = fechaDevolucion;
            RutaImagen = rutaImagen;
        }

        public int IdUsuario { get; set; }
        public int IdElemento { get; set; }
        public DateTime FechaPrestamo { get { return _fechaPrestamo; } set { _fechaPrestamo = value; } }
        public DateTime FechaDevolucion { get { return _fechaDevolucion; } set { if (value < _fechaPrestamo) throw new ArgumentException("Error fecha."); _fechaDevolucion = value; } }
        public string RutaImagen { get { return _rutaImagen; } set { _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_prestamo.png" : value.Trim(); } }

        public override string ToString() { return $"Préstamo #{Id} - Fecha: {_fechaPrestamo.ToShortDateString()} | Entrega: {_fechaDevolucion.ToShortDateString()}"; }

        public bool VerificarVencimiento() { if (!this.EsActivo) return false; return DateTime.Now > this._fechaDevolucion; }
        public bool VerificarVencimiento(DateTime fechaCorteExterna) { if (!this.EsActivo) return false; return fechaCorteExterna > this._fechaDevolucion; }

        public void InsertarRegistro(object objeto) { _tabla.Add((Prestamo)objeto); }
        public object ConsultarRegistro(string id) { int idB = int.Parse(id); return _tabla.FirstOrDefault(x => x.Id == idB); }
        public void ActualizarRegistro(object objeto)
        {
            var act = (Prestamo)objeto; var ext = (Prestamo)ConsultarRegistro(act.Id.ToString());
            if (ext != null) { ext.IdUsuario = act.IdUsuario; ext.IdElemento = act.IdElemento; ext.FechaPrestamo = act.FechaPrestamo; ext.FechaDevolucion = act.FechaDevolucion; ext.RutaImagen = act.RutaImagen; ext.EsActivo = act.EsActivo; }
        }
        public void EliminarRegistro(string id) { var ext = (Prestamo)ConsultarRegistro(id); if (ext != null) _tabla.Remove(ext); }
    }
}


