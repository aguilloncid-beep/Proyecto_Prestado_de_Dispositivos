using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaPresatamos.Models
{
    internal class Ubicacion : EntidadBase, IAlmacenamientoCRUD
    {
        private static List<Ubicacion> _tabla = new List<Ubicacion>();

        private string _edificio = string.Empty;
        private string _almacen = string.Empty;
        private string _rutaImagen = "default_ubicacion.png";

        public Ubicacion() : base() { }

        public Ubicacion(int id, string edificio, string almacen, string rutaImagen, DateTime fechaRegistro, bool esActivo)
            : base(id, fechaRegistro, esActivo)
        {
            Edificio = edificio; Almacen = almacen; RutaImagen = rutaImagen;
        }

        public string Edificio { get { return _edificio; } set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Vacío."); _edificio = value.Trim(); } }
        public string Almacen { get { return _almacen; } set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Vacío."); _almacen = value.Trim(); } }
        public string RutaImagen { get { return _rutaImagen; } set { _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_ubicacion.png" : value.Trim(); } }

        public override string ToString() { return $"Ubicación: {_edificio} - Almacén: {_almacen} (Activo: {EsActivo})"; }

        public bool ValidarDisponibilidadEspacio() { return this.EsActivo; }
        public bool ValidarDisponibilidadEspacio(int cantidadArticulosActuales, int capacidadMaxima) { if (!this.EsActivo) return false; return cantidadArticulosActuales < capacidadMaxima; }

        public void InsertarRegistro(object objeto) { _tabla.Add((Ubicacion)objeto); }
        public object ConsultarRegistro(string id) { int idB = int.Parse(id); return _tabla.FirstOrDefault(x => x.Id == idB); }
        public void ActualizarRegistro(object objeto)
        {
            var act = (Ubicacion)objeto; var ext = (Ubicacion)ConsultarRegistro(act.Id.ToString());
            if (ext != null) { ext.Edificio = act.Edificio; ext.Almacen = act.Almacen; ext.RutaImagen = act.RutaImagen; ext.EsActivo = act.EsActivo; }
        }
        public void EliminarRegistro(string id) { var ext = (Ubicacion)ConsultarRegistro(id); if (ext != null) _tabla.Remove(ext); }
    }
}


