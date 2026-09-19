using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaPresatamos.Models
{
    public class Usuario : EntidadBase, IAlmacenamientoCRUD
    {
        private static List<Usuario> _tabla = new List<Usuario>();

        private string _numeroEstudiante = string.Empty;
        private string _nombre = string.Empty;
        private string _carrera = string.Empty;
        private string _rutaImagen = "default_user.png";

        public Usuario() : base() { }

        public Usuario(int id, string numeroEstudiante, string nombre, string carrera, string rutaImagen, DateTime fechaRegistro, bool esActivo)
            : base(id, fechaRegistro, esActivo)
        {
            NumeroEstudiante = numeroEstudiante; Nombre = nombre; Carrera = carrera; RutaImagen = rutaImagen;
        }

        public string NumeroEstudiante { get { return _numeroEstudiante; } set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Vacío."); _numeroEstudiante = value.Trim(); } }
        public string Nombre { get { return _nombre; } set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Vacío."); _nombre = value.Trim(); } }
        public string Carrera { get { return _carrera; } set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Vacío."); _carrera = value.Trim(); } }
        public string RutaImagen { get { return _rutaImagen; } set { _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_user.png" : value.Trim(); } }

        public override string ToString() { return $"Usuario: {_nombre} ({_numeroEstudiante}) - Carrera: {_carrera}"; }

        public bool ValidarElegibilidadPrestamo() { return this.EsActivo && !string.IsNullOrEmpty(this._numeroEstudiante); }
        public bool ValidarElegibilidadPrestamo(int prestamosActivosActuales) { if (!this.EsActivo) return false; return prestamosActivosActuales < 3; }

        public void InsertarRegistro(object objeto) { _tabla.Add((Usuario)objeto); }
        public object ConsultarRegistro(string id) { int idB = int.Parse(id); return _tabla.FirstOrDefault(x => x.Id == idB); }
        public void ActualizarRegistro(object objeto)
        {
            var act = (Usuario)objeto; var ext = (Usuario)ConsultarRegistro(act.Id.ToString());
            if (ext != null) { ext.NumeroEstudiante = act.NumeroEstudiante; ext.Nombre = act.Nombre; ext.Carrera = act.Carrera; ext.RutaImagen = act.RutaImagen; ext.EsActivo = act.EsActivo; }
        }
        public void EliminarRegistro(string id) { var ext = (Usuario)ConsultarRegistro(id); if (ext != null) _tabla.Remove(ext); }
    }
}


