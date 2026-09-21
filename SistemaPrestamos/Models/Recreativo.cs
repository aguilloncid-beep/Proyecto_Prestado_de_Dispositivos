using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaPresatamos.Models
{
    internal class Recreativo : EntidadBase, IAlmacenamientoCRUD
    {
        private static List<Recreativo> _tabla = new List<Recreativo>();

        private string _numero = string.Empty;
        private string _nombre = string.Empty;
        private string _modelo = string.Empty;
        private string _rutaImagen = "default_recreativo.png";

        public Recreativo() : base() { }

        public Recreativo(int id, string numero, string nombre, string modelo, string rutaImagen, DateTime fechaRegistro, bool esActivo)
            : base(id, fechaRegistro, esActivo)
        {
            Numero = numero; Nombre = nombre; Modelo = modelo; RutaImagen = rutaImagen;
        }

        public string Numero { get { return _numero; } set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Vacío."); _numero = value.Trim(); } }
        public string Nombre { get { return _nombre; } set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Vacío."); _nombre = value.Trim(); } }
        public string Modelo { get { return _modelo; } set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Vacío."); _modelo = value.Trim(); } }
        public string RutaImagen { get { return _rutaImagen; } set { _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_recreativo.png" : value.Trim(); } }

        public override string ToString() { return $"Recreativo: {_nombre} - Modelo: {_modelo} (# {_numero})"; }

        public bool ValidarDisponibilidad() { return this.EsActivo; }
        public bool ValidarDisponibilidad(int horasPrestamoSolicitadas) { if (!this.EsActivo) return false; return horasPrestamoSolicitadas > 0 && horasPrestamoSolicitadas <= 4; }

        public void InsertarRegistro(object objeto) { _tabla.Add((Recreativo)objeto); }
        public object ConsultarRegistro(string id) { int idB = int.Parse(id); return _tabla.FirstOrDefault(x => x.Id == idB); }
        public void ActualizarRegistro(object objeto)
        {
            var act = (Recreativo)objeto; var ext = (Recreativo)ConsultarRegistro(act.Id.ToString());
            if (ext != null) { ext.Numero = act.Numero; ext.Nombre = act.Nombre; ext.Modelo = act.Modelo; ext.RutaImagen = act.RutaImagen; ext.EsActivo = act.EsActivo; }
        }
        public void EliminarRegistro(string id) { var ext = (Recreativo)ConsultarRegistro(id); if (ext != null) _tabla.Remove(ext); }
    }
}


