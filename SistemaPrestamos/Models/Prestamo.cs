using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPresatamos.Models
{
    internal class Prestamo
    {
        // ==========================================
        // 3. CAMPOS PRIVADOS (Ocultamiento de información)
        // ==========================================
        private int _idPrestamo = 0;
        private DateTime _fechaPrestamo = DateTime.Now;
        private DateTime _fechaDevolucion = DateTime.Now.AddDays(3);
        private string _rutaImagen = "default_prestamo.png";
        private bool _estado = true;     // True = Vigente/Activo, False = Concluido/Cancelado

        // ==========================================
        // 4. CONSTRUCTORES (Por defecto y parametrizado)
        // ==========================================

        // Constructor por defecto
        public Prestamo()
        {
            _idPrestamo = 0;
            _fechaPrestamo = DateTime.Now;
            _fechaDevolucion = DateTime.Now.AddDays(3); // 3 días por defecto
            _rutaImagen = "default_prestamo.png";
            _estado = true; // Activo por defecto
        }

        // Constructor parametrizado
        public Prestamo(int idPrestamo, DateTime fechaPrestamo, DateTime fechaDevolucion, string rutaImagen, bool estado)
        {
            IdPrestamo = idPrestamo;
            FechaPrestamo = fechaPrestamo;
            FechaDevolucion = fechaDevolucion;
            RutaImagen = rutaImagen;
            Estado = estado;
        }

        public override string ToString()
        {
            return $"Préstamo #{_idPrestamo} - Fecha: {_fechaPrestamo.ToShortDateString()} | Entrega estimada: {_fechaDevolucion.ToShortDateString()}";
        }

        // ==========================================
        // 3. PROPIEDADES PÚBLICAS CON FILTROS DE VALIDACIÓN (Encapsulamiento)
        // ========
        // ==================================
        public int IdUsuario { get; set; }
        public int IdElemento { get; set; }
        public int IdPrestamo
        {
            get { return _idPrestamo; }
            set
            {
                if (value < 0) throw new ArgumentException("El ID del préstamo no puede ser negativo.");
                _idPrestamo = value;
            }
        }

        public DateTime FechaPrestamo
        {
            get { return _fechaPrestamo; }
            set { _fechaPrestamo = value; }
        }

        public DateTime FechaDevolucion
        {
            get { return _fechaDevolucion; }
            set
            {
                if (value < _fechaPrestamo)
                    throw new ArgumentException("La fecha de devolución esperada no puede ser anterior a la fecha del préstamo.");
                _fechaDevolucion = value;
            }
        }

        public string RutaImagen
        {
            get { return _rutaImagen; }
            set
            {
                _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_prestamo.png" : value.Trim();
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

        // Versión A (Sin parámetros externos): Verifica si el préstamo está vencido comparándolo con la fecha actual del sistema
        public bool VerificarVencimiento()
        {
            if (!this._estado) return false; // Si ya no está activo, no cuenta como vencido pendiente
            return DateTime.Now > this._fechaDevolucion;
        }

        // Versión B (Con parámetro externo): Verifica si está vencido comparándolo con una fecha de corte o auditoría externa
        public bool VerificarVencimiento(DateTime fechaCorteExterna)
        {
            if (!this._estado) return false;
            return fechaCorteExterna > this._fechaDevolucion;
        }
    }
}
    

