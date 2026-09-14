using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPresatamos.Models
{
    internal class Devolucion
    {
        // ==========================================
        // 3. CAMPOS PRIVADOS (Ocultamiento de información)
        // ==========================================
        private int _idDevolucion;
        private DateTime _devolucionReal;
        private string _rutaImagen = "default_devolucion.png";
        // Campo obligatorio por rúbrica de escritorio (ej. foto del estado del equipo al regresar)
        private bool _estado;     // True = Procesada/Correcta, False = Con daños o pendientes

        // ==========================================
        // 4. CONSTRUCTORES (Por defecto y parametrizado)
        // ==========================================

        // Constructor por defecto
        public Devolucion()
        {
            _idDevolucion = 0;
            _devolucionReal = DateTime.Now;
            _rutaImagen = "default_devolucion.png";
            _estado = true; // Correcta por defecto
        }

        // Constructor parametrizado
        public Devolucion(int idDevolucion, DateTime devolucionReal, string rutaImagen, bool estado)
        {
            IdDevolucion = idDevolucion;
            DevolucionReal = devolucionReal;
            RutaImagen = rutaImagen;
            Estado = estado;
        }

        public override string ToString()
        {
            return $"Devolución #{_idDevolucion} - Realizada el: {_devolucionReal.ToShortDateString()} - Estado: {_estado}";
        }

        // ==========================================
        // 3. PROPIEDADES PÚBLICAS CON FILTROS DE VALIDACIÓN (Encapsulamiento)
        // ==========================================
        public int IdDevolucion
        {
            get { return _idDevolucion; }
            set
            {
                if (value < 0) throw new ArgumentException("El ID de devolución no puede ser negativo.");
                _idDevolucion = value;
            }
        }

        public DateTime DevolucionReal
        {
            get { return _devolucionReal; }
            set
            {
                _devolucionReal = value;
            }
        }

        public string RutaImagen
        {
            get { return _rutaImagen; }
            set
            {
                _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_devolucion.png" : value.Trim();
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

        // Versión A (Sin parámetros externos): Verifica si la devolución real se registró el día de hoy
        public bool ValidarDevolucionHoy()
        {
            return this._devolucionReal.Date == DateTime.Now.Date;
        }

        // Versión B (Con parámetro externo): Comprueba si el objeto se devolvió a tiempo comparándolo con una fecha límite externa
        public bool ValidarDevolucionHoy(DateTime fechaLimiteEsperada)
        {
            return this._devolucionReal <= fechaLimiteEsperada;
        }
    }
}
    

