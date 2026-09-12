using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPresatamos.Models
{
    internal class Ubicacion
    {
        // ==========================================
        // 3. CAMPOS PRIVADOS (Ocultamiento de información)
        // ==========================================
        private int _idUbicacion = 0;
        private string _edificio = string.Empty;
        private string _almacen = string.Empty;
        private string _rutaImagen = "default_ubicacion.png";
        private bool _estado = true;     // True = Disponible/Activo, False = Saturado/Inhabilitado

        // ==========================================
        // 4. CONSTRUCTORES (Por defecto y parametrizado)
        // ==========================================

        // Constructor por defecto
        public Ubicacion()
        {
            _idUbicacion = 0;
            _edificio = string.Empty;
            _almacen = string.Empty;
            _rutaImagen = "default_ubicacion.png";
            _estado = true; // Activo por defecto
        }

        // Constructor parametrizado
        public Ubicacion(int idUbicacion, string edificio, string almacen, string rutaImagen, bool estado)
        {
            IdUbicacion = idUbicacion;
            Edificio = edificio;
            Almacen = almacen;
            RutaImagen = rutaImagen;
            Estado = estado;
        }

        public override string ToString()
        {
            return $"Ubicación: {_edificio} - Almacén: {_almacen} (Activo: {_estado})";
        }

        // ==========================================
        // 3. PROPIEDADES PÚBLICAS CON FILTROS DE VALIDACIÓN (Encapsulamiento)
        // ==========================================
        public int IdUbicacion
        {
            get { return _idUbicacion; }
            set
            {
                if (value < 0) throw new ArgumentException("El ID de ubicación no puede ser negativo.");
                _idUbicacion = value;
            }
        }

        public string Edificio
        {
            get { return _edificio; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El edificio no puede estar vacío.");
                _edificio = value.Trim();
            }
        }

        public string Almacen
        {
            get { return _almacen; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El almacén no puede estar vacío.");
                _almacen = value.Trim();
            }
        }

        public string RutaImagen
        {
            get { return _rutaImagen; }
            set
            {
                _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_ubicacion.png" : value.Trim();
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

        // Versión A (Sin parámetros externos): Verifica si la ubicación está activa para recibir equipo
        public bool ValidarDisponibilidadEspacio()
        {
            return this._estado;
        }

        // Versión B (Con parámetro externo): Compara los artículos actuales con la capacidad máxima del almacén
        public bool ValidarDisponibilidadEspacio(int cantidadArticulosActuales, int capacidadMaxima)
        {
            if (!this._estado) return false;
            return cantidadArticulosActuales < capacidadMaxima;
        }
    }
}
    

