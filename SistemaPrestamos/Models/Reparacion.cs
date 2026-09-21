using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaPresatamos.Models
{
    internal class Reparacion : EntidadBase, IAlmacenamientoCRUD
    {
        private static List<Reparacion> _tabla = new List<Reparacion>();

        private string _material = string.Empty;
        private DateTime _fecha = DateTime.Now;
        private string _rutaImagen = "default_reparacion.png";

        public Reparacion() : base() { }

        public Reparacion(int id, string material, DateTime fecha, string rutaImagen, DateTime fechaRegistro, bool esActivo)
            : base(id, fechaRegistro, esActivo)
        {
            Material = material; Fecha = fecha; RutaImagen = rutaImagen;
        }

        public string Material { get { return _material; } set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Vacío."); _material = value.Trim(); } }
        public DateTime Fecha { get { return _fecha; } set { _fecha = value; } }
        public string RutaImagen { get { return _rutaImagen; } set { _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_reparacion.png" : value.Trim(); } }

        public override string ToString() { return $"Reparación #{Id} - Material: {_material} - Fecha: {_fecha.ToShortDateString()}"; }

        public bool VerificarRetrasoReparacion() { if (!this.EsActivo) return false; return (DateTime.Now - this._fecha).Days > 10; }
        public bool VerificarRetrasoReparacion(int diasMaximosTaller) { if (!this.EsActivo) return false; return (DateTime.Now - this._fecha).Days > diasMaximosTaller; }

        public void InsertarRegistro(object objeto) { _tabla.Add((Reparacion)objeto); }
        public object ConsultarRegistro(string id) { int idB = int.Parse(id); return _tabla.FirstOrDefault(x => x.Id == idB); }
        public void ActualizarRegistro(object objeto)
        {
            var act = (Reparacion)objeto; var ext = (Reparacion)ConsultarRegistro(act.Id.ToString());
            if (ext != null) { ext.Material = act.Material; ext.Fecha = act.Fecha; ext.RutaImagen = act.RutaImagen; ext.EsActivo = act.EsActivo; }
        }
        public void EliminarRegistro(string id) { var ext = (Reparacion)ConsultarRegistro(id); if (ext != null) _tabla.Remove(ext); }
    }
}


