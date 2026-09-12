using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPresatamos.Models
{
    internal class Reparacion
    {
        // ==========================================
        // 3. CAMPOS PRIVADOS (Ocultamiento de información)
        // ==========================================
        private int _idReparacion = 0;
        private string _material = string.Empty;
        private DateTime _fecha = DateTime.Now;
        private string _rutaImagen = "default_reparacion.png";
        private bool _estado = true;     // True = En proceso de reparación, False = Reparado/Finalizado

        // ==========================================
        // 4. CONSTRUCTORES (Por defecto y parametrizado)
        // ==========================================

        // Constructor por defecto
        public Reparacion()
        {
            _idReparacion = 0;
            _material = string.Empty;
            _fecha = DateTime.Now;
            _rutaImagen = "default_reparacion.png";
            _estado = true; // Activo/En proceso por defecto
        }

        // Constructor parametrizado
        public Reparacion(int idReparacion, string material, DateTime fecha, string rutaImagen, bool estado)
        {
            IdReparacion = idReparacion;
            Material = material;
            Fecha = fecha;
            RutaImagen = rutaImagen;
            Estado = estado;
        }

        public override string ToString()
        {
            return $"Reparación #{_idReparacion} - Material: {_material} - Fecha: {_fecha.ToShortDateString()}";
        }

        // ==========================================
        // 3. PROPIEDADES PÚBLICAS CON FILTROS DE VALIDACIÓN (Encapsulamiento)
        // ==========================================
        public int IdReparacion
        {
            get { return _idReparacion; }
            set
            {
                if (value < 0) throw new ArgumentException("El ID de reparación no puede ser negativo.");
                _idReparacion = value;
            }
        }

        public string Material
        {
            get { return _material; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El campo material no puede estar vacío.");
                _material = value.Trim();
            }
        }

        public DateTime Fecha
        {
            get { return _fecha; }
            set { _fecha = value; }
        }

        public string RutaImagen
        {
            get { return _rutaImagen; }
            set
            {
                _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_reparacion.png" : value.Trim();
            }
        }

        public bool Estado
        {
            get { return _estado; }
            set { _estado = value; }
        }

        // ==========================================
        // 5. FUNCIONES DE NEGOCIO Y SOBRECARGA DE MÉTODOS
        // ==========================================

        // Versión A (Sin parámetros externos): Verifica si la reparación lleva más de 10 días abierta
        public bool VerificarRetrasoReparacion()
        {
            if (!this._estado) return false; // Si ya está concluida, no hay retraso activo
            TimeSpan tiempoTranscurrido = DateTime.Now - this._fecha;
            return tiempoTranscurrido.Days > 10;
        }

        // Versión B (Con parámetro externo): Compara el tiempo transcurrido contra un límite de días de taller externo
        public bool VerificarRetrasoReparacion(int diasMaximosTaller)
        {
            if (!this._estado) return false;
            TimeSpan tiempoTranscurrido = DateTime.Now - this._fecha;
            return tiempoTranscurrido.Days > diasMaximosTaller;
        }
    }
}
    

