using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaPresatamos.Models
{
    internal class Devolucion : EntidadBase, IAlmacenamientoCRUD
    {
        private static List<Devolucion> _tabla = new List<Devolucion>();

        private DateTime _devolucionReal;
        private string _rutaImagen = "default_devolucion.png";

        public Devolucion() : base()
        {
            _devolucionReal = DateTime.Now;
            _rutaImagen = "default_devolucion.png";
        }

        public Devolucion(int id, DateTime devolucionReal, string rutaImagen, DateTime fechaRegistro, bool esActivo)
            : base(id, fechaRegistro, esActivo)
        {
            DevolucionReal = devolucionReal;
            RutaImagen = rutaImagen;
        }

        public DateTime DevolucionReal { get { return _devolucionReal; } set { _devolucionReal = value; } }
        public string RutaImagen { get { return _rutaImagen; } set { _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_devolucion.png" : value.Trim(); } }

        public override string ToString() { return $"Devolución #{Id} - Realizada el: {_devolucionReal.ToShortDateString()} - Activo: {EsActivo}"; }

        public bool ValidarDevolucionHoy() { return this._devolucionReal.Date == DateTime.Now.Date; }
        public bool ValidarDevolucionHoy(DateTime fechaLimiteEsperada) { return this._devolucionReal <= fechaLimiteEsperada; }

        public void InsertarRegistro(object objeto) { _tabla.Add((Devolucion)objeto); }
        public object ConsultarRegistro(string id) { int idB = int.Parse(id); return _tabla.FirstOrDefault(x => x.Id == idB); }
        public void ActualizarRegistro(object objeto)
        {
            var act = (Devolucion)objeto; var ext = (Devolucion)ConsultarRegistro(act.Id.ToString());
            if (ext != null) { ext.DevolucionReal = act.DevolucionReal; ext.RutaImagen = act.RutaImagen; ext.EsActivo = act.EsActivo; }
        }
        public void EliminarRegistro(string id) { var ext = (Devolucion)ConsultarRegistro(id); if (ext != null) _tabla.Remove(ext); }
    }
}


