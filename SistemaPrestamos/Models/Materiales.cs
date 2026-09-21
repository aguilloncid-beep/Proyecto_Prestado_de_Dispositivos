using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaPresatamos.Models
{
    public class Materiales : EntidadBase, IAlmacenamientoCRUD
    {
        private static List<Materiales> _tabla = new List<Materiales>();

        private string _numeroSerie = string.Empty;
        private string _nombre = string.Empty;
        private string _rutaImagen = "default_material.png";

        public Materiales() : base() { }

        public Materiales(int id, string numeroSerie, string nombre, string rutaImagen, DateTime fechaRegistro, bool esActivo)
            : base(id, fechaRegistro, esActivo)
        {
            NumeroSerie = numeroSerie;
            Nombre = nombre;
            RutaImagen = rutaImagen;
        }

        public string NumeroSerie { get { return _numeroSerie; } set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Serie vacía."); _numeroSerie = value.Trim(); } }
        public string Nombre { get { return _nombre; } set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Nombre vacío."); _nombre = value.Trim(); } }
        public string RutaImagen { get { return _rutaImagen; } set { _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_material.png" : value.Trim(); } }

        public override string ToString() { return $"Materiales: {_nombre} [Serie: {_numeroSerie}] - Activo: {EsActivo}"; }

        public bool VerificarDisponibilidad() { return this.EsActivo; }
        public bool VerificarDisponibilidad(int diasPrestamoSolicitados) { if (!this.EsActivo) return false; return diasPrestamoSolicitados > 0 && diasPrestamoSolicitados <= 7; }

        public void InsertarRegistro(object objeto) { _tabla.Add((Materiales)objeto); }
        public object ConsultarRegistro(string id) { int idB = int.Parse(id); return _tabla.FirstOrDefault(x => x.Id == idB); }
        public void ActualizarRegistro(object objeto)
        {
            var act = (Materiales)objeto; var ext = (Materiales)ConsultarRegistro(act.Id.ToString());
            if (ext != null) { ext.NumeroSerie = act.NumeroSerie; ext.Nombre = act.Nombre; ext.RutaImagen = act.RutaImagen; ext.EsActivo = act.EsActivo; }
        }
        public void EliminarRegistro(string id) { var ext = (Materiales)ConsultarRegistro(id); if (ext != null) _tabla.Remove(ext); }
    }
}



