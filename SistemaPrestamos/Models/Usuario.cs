using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPresatamos.Models
{
    public class Usuario
    {
        // ==========================================
        // 3. CAMPOS PRIVADOS (Ocultamiento de información)
        // ==========================================
        private int _idUsuario = 0;
        private string _numeroEstudiante = string.Empty;
        private string _nombre = string.Empty;
        private string _carrera = string.Empty;
        private string _rutaImagen = "default_user.png";
        private bool _estado = true;

        // ==========================================
        // 4. CONSTRUCTORES (Por defecto y parametrizado)
        // ==========================================

        // Constructor por defecto
        public Usuario()
        {
            _idUsuario = 0;
            _numeroEstudiante = string.Empty;
            _nombre = string.Empty;
            _carrera = string.Empty;
            _rutaImagen = "default_user.png";
            _estado = true; // Activo por defecto
        }

        // Constructor parametrizado
        public Usuario(int idUsuario, string numeroEstudiante, string nombre, string carrera, string rutaImagen, bool estado)
        {
            IdUsuario = idUsuario;
            NumeroEstudiante = numeroEstudiante;
            Nombre = nombre;
            Carrera = carrera;
            RutaImagen = rutaImagen;
            Estado = estado;
        }
        public override string ToString()
        {
            return $"Usuario: {_nombre} ({_numeroEstudiante}) - Carrera: {_carrera}";
        }

        // ==========================================
        // 3. PROPIEDADES PÚBLICAS CON FILTROS DE VALIDACIÓN (Encapsulamiento)
        // ==========================================
        public int IdUsuario
        {
            get { return _idUsuario; }
            set
            {
                if (value < 0) throw new ArgumentException("El ID del usuario no puede ser negativo.");
                _idUsuario = value;
            }
        }

        public string NumeroEstudiante
        {
            get { return _numeroEstudiante; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El número de estudiante no puede estar vacío.");
                _numeroEstudiante = value.Trim();
            }
        }

        public string Nombre
        {
            get { return _nombre; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El nombre del usuario no puede estar vacío.");
                _nombre = value.Trim();
            }
        }

        public string Carrera
        {
            get { return _carrera; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("La carrera no puede estar vacía.");
                _carrera = value.Trim();
            }
        }

        public string RutaImagen
        {
            get { return _rutaImagen; }
            set
            {
                _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_user.png" : value.Trim();
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

        // Versión A (Sin parámetros externos): Valida si el usuario se encuentra habilitado para solicitar préstamos
        public bool ValidarElegibilidadPrestamo()
        {
            return this._estado && !string.IsNullOrEmpty(this._numeroEstudiante);
        }

        // Versión B (Con parámetro externo): Recibe un límite de préstamos activos permitidos para evaluar si puede solicitar más
        public bool ValidarElegibilidadPrestamo(int prestamosActivosActuales)
        {
            const int limiteMaximoPrestamos = 3; // Regla de negocio de la escuela
            if (!this._estado) return false;

            return prestamosActivosActuales < limiteMaximoPrestamos;
        }
    }
}
    

