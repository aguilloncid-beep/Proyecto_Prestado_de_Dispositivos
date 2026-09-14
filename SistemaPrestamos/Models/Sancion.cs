using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPresatamos.Models
{
    internal class Sancion
    {
        // ==========================================
        // 3. CAMPOS PRIVADOS (Ocultamiento de información)
        // ==========================================
        private int _idSancion = 0;
        private string _motivo = string.Empty;
        private decimal _monto = 0.0m;
        private string _reporte = string.Empty;
        private string _rutaImagen = "default_sancion.png";
        private bool _activaPagada = true;
        private bool _estado = true;       // Estado general del registro requerido por la rúbrica

        // ==========================================
        // 4. CONSTRUCTORES (Por defecto y parametrizado)
        // ==========================================

        // Constructor por defecto
        public Sancion()
        {
            _idSancion = 0;
            _motivo = string.Empty;
            _monto = 0.0m;
            _reporte = string.Empty;
            _rutaImagen = "default_sancion.png";
            _activaPagada = true; // Activa por defecto
            _estado = true;
        }

        // Constructor parametrizado
        public Sancion(int idSancion, string motivo, decimal monto, string reporte, string rutaImagen, bool activaPagada, bool estado)
        {
            IdSancion = idSancion;
            Motivo = motivo;
            Monto = monto;
            Reporte = reporte;
            RutaImagen = rutaImagen;
            ActivaPagada = activaPagada;
            Estado = estado;
        }

        public override string ToString()
        {
            return $"Sanción #{_idSancion} - Motivo: {_motivo} | Monto: {_monto:C} - Activa/Pendiente: {_activaPagada}";
        }

        // ==========================================
        // 3. PROPIEDADES PÚBLICAS CON FILTROS DE VALIDACIÓN (Encapsulamiento)
        // ==========================================
        public int IdSancion
        {
            get { return _idSancion; }
            set
            {
                if (value < 0) throw new ArgumentException("El ID de la sanción no puede ser negativo.");
                _idSancion = value;
            }
        }

        public string Motivo
        {
            get { return _motivo; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El motivo de la sanción no puede estar vacío.");
                _motivo = value.Trim();
            }
        }

        public decimal Monto
        {
            get { return _monto; }
            set
            {
                if (value < 0) throw new ArgumentException("El monto de la sanción no puede ser negativo.");
                _monto = value;
            }
        }

        public string Reporte
        {
            get { return _reporte; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El reporte no puede estar vacío.");
                _reporte = value.Trim();
            }
        }

        public string RutaImagen
        {
            get { return _rutaImagen; }
            set
            {
                _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_sancion.png" : value.Trim();
            }
        }

        public bool ActivaPagada
        {
            get { return _activaPagada; }
            set { _activaPagada = value; }
        }

        public bool Estado
        {
            get { return _estado; }
            set { _estado = value; }
        }

        // ==========================================
        // 5. FUNCIONES DE NEGOCIO Y SOBRECARGA DE MÉTODOS
        // ==========================================

        // Versión A (Sin parámetros externos): Verifica si la sanción sigue activa requiriendo atención o pago
        public bool VerificarEstatusSancion()
        {
            return this._activaPagada && this._estado;
        }

        // Versión B (Con parámetro externo): Compara el monto de la sanción contra un límite económico máximo establecido por la escuela
        public bool VerificarEstatusSancion(decimal montoMaximoPermitido)
        {
            if (!this._activaPagada) return false;
            return this._monto <= montoMaximoPermitido;
        }
    }
}
    

