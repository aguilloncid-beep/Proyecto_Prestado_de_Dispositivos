using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPresatamos.Models
{
    internal class Apartado
    {
        // ==========================================
        // 3. CAMPOS PRIVADOS (Ocultamiento de información)
        // ==========================================
        private int _idApartado;
        private DateTime _fechaSolicitud;
        private DateTime _fechaReserva;
        private string _rutaImagen = "default_apartado.png"; // Campo obligatorio por rúbrica de escritorio
        private bool _estado;     // True = Confirmado/Activo, False = Cancelado/Expirado

        // ==========================================
        // 4. CONSTRUCTORES (Por defecto y parametrizado)
        // ==========================================

        // Constructor por defecto
        public Apartado()
        {
            _idApartado = 0;
            _fechaSolicitud = DateTime.Now;
            _fechaReserva = DateTime.Now.AddDays(3); // 3 días antes por defecto
            _rutaImagen = "default_apartado.png";
            _estado = true; // Activo por defecto
        }

        // Constructor parametrizado
        public Apartado(int idApartado, DateTime fechaSolicitud, DateTime fechaReserva, string rutaImagen, bool estado)
        {
            IdApartado = idApartado;
            FechaSolicitud = fechaSolicitud;
            FechaReserva = fechaReserva;
            RutaImagen = rutaImagen;
            Estado = estado;
        }
        public override string ToString()
        {
            return $"Apartado #{_idApartado} - Solicitud: {_fechaSolicitud.ToShortDateString()} | Reserva: {_fechaReserva.ToShortDateString()}";
        }

        // ==========================================
        // 3. PROPIEDADES PÚBLICAS CON FILTROS DE VALIDACIÓN (Encapsulamiento)
        // ==========================================
        public int IdApartado
        {
            get { return _idApartado; }
            set
            {
                if (value < 0) throw new ArgumentException("El ID del apartado no puede ser negativo.");
                _idApartado = value;
            }
        }

        public DateTime FechaSolicitud
        {
            get { return _fechaSolicitud; }
            set { _fechaSolicitud = value; }
        }

        public DateTime FechaReserva
        {
            get { return _fechaReserva; }
            set
            {
                if (value < _fechaSolicitud)
                    throw new ArgumentException("La fecha de reserva no puede ser anterior a la fecha de solicitud.");
                _fechaReserva = value;
            }
        }

        public string RutaImagen
        {
            get { return _rutaImagen; }
            set
            {
                _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_apartado.png" : value.Trim();
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

        // Versión A (Sin parámetros externos): Verifica si el apartado cumple con la regla de los 3 días de anticipación
        public bool ValidarAnticipacionApartado()
        {
            TimeSpan diferencia = this._fechaReserva - this._fechaSolicitud;
            return diferencia.Days >= 3;
        }

        // Versión B (Con parámetro externo): Valida la vigencia del apartado comparándolo con una fecha y hora de entrega límite externa
        public bool ValidarAnticipacionApartado(DateTime fechaHoraLimiteEntrega)
        {
            if (!this._estado) return false;
            return DateTime.Now <= fechaHoraLimiteEntrega;
        }
    }
}
    

