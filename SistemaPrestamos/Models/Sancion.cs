using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaPresatamos.Models
{
    internal class Sancion : EntidadBase, IAlmacenamientoCRUD
    {
        private static List<Sancion> _tabla = new List<Sancion>();

        private string _motivo = string.Empty;
        private decimal _monto = 0.0m;
        private string _reporte = string.Empty;
        private string _rutaImagen = "default_sancion.png";
        private bool _activaPagada = true;

        public Sancion() : base() { }

        public Sancion(int id, string motivo, decimal monto, string reporte, string rutaImagen, bool activaPagada, DateTime fechaRegistro, bool esActivo)
            : base(id, fechaRegistro, esActivo)
        {
            Motivo = motivo; Monto = monto; Reporte = reporte; RutaImagen = rutaImagen; ActivaPagada = activaPagada;
        }

        public string Motivo { get { return _motivo; } set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Vacío."); _motivo = value.Trim(); } }
        public decimal Monto { get { return _monto; } set { if (value < 0) throw new ArgumentException("Negativo."); _monto = value; } }
        public string Reporte { get { return _reporte; } set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Vacío."); _reporte = value.Trim(); } }
        public string RutaImagen { get { return _rutaImagen; } set { _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_sancion.png" : value.Trim(); } }
        public bool ActivaPagada { get { return _activaPagada; } set { _activaPagada = value; } }

        public override string ToString() { return $"Sanción #{Id} - Motivo: {_motivo} | Monto: {_monto:C} - Activa: {_activaPagada}"; }

        public bool VerificarEstatusSancion() { return this._activaPagada && this.EsActivo; }
        public bool VerificarEstatusSancion(decimal montoMaximoPermitido) { if (!this._activaPagada) return false; return this._monto <= montoMaximoPermitido; }

        public void InsertarRegistro(object objeto) { _tabla.Add((Sancion)objeto); }
        public object ConsultarRegistro(string id) { int idB = int.Parse(id); return _tabla.FirstOrDefault(x => x.Id == idB); }
        public void ActualizarRegistro(object objeto)
        {
            var act = (Sancion)objeto; var ext = (Sancion)ConsultarRegistro(act.Id.ToString());
            if (ext != null) { ext.Motivo = act.Motivo; ext.Monto = act.Monto; ext.Reporte = act.Reporte; ext.RutaImagen = act.RutaImagen; ext.ActivaPagada = act.ActivaPagada; ext.EsActivo = act.EsActivo; }
        }
        public void EliminarRegistro(string id) { var ext = (Sancion)ConsultarRegistro(id); if (ext != null) _tabla.Remove(ext); }
    }
}


