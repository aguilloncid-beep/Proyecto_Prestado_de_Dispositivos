using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPresatamos.Models
{
    public class Administrador
    {
        // ==========================================
        // 3. CAMPOS PRIVADOS (Ocultamiento de información)
        // ==========================================
        private int _idAdmin = 0;
        private string _correo = string.Empty;
        private string _nombre = string.Empty;
        private string _contrasena = string.Empty;
        private string _rutaImagen = "default_admin.png";
        private bool _estado = true;

        // ==========================================
        // 4. CONSTRUCTORES (Por defecto y parametrizado)
        // ==========================================

        // Constructor por defecto
        public Administrador()
        {
            _idAdmin = 0;
            _correo = string.Empty;
            _nombre = string.Empty;
            _contrasena = string.Empty;
            _rutaImagen = "default_admin.png";
            _estado = true; // Activo por defecto
        }


        // Constructor parametrizado
        public Administrador(int idAdmin, string correo, string nombre, string contrasena, string rutaImagen, bool estado)
        {
            // Asignación a través de las propiedades para aplicar filtros de validación
            IdAdmin = idAdmin;
            Correo = correo;
            Nombre = nombre;
            Contrasena = contrasena;
            RutaImagen = rutaImagen;
            Estado = estado;
        }
        public override string ToString()
        {
            return $"Administrador: {_nombre} (Correo: {_correo}) - Activo: {_estado}";
        }

        // ==========================================
        // 3. PROPIEDADES PÚBLICAS CON FILTROS DE VALIDACIÓN (Encapsulamiento)
        // ==========================================
        public int IdAdmin
        {
            get { return _idAdmin; }
            set
            {
                if (value < 0) throw new ArgumentException("El ID del administrador no puede ser negativo.");
                _idAdmin = value;
            }
        }

        public string Correo
        {
            get { return _correo; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                    throw new ArgumentException("El correo electrónico no es válido.");
                _correo = value.Trim();
            }
        }

        public string Nombre
        {
            get { return _nombre; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El nombre no puede estar vacío.");
                _nombre = value.Trim();
            }
        }

        public string Contrasena
        {
            get { return _contrasena; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 6)
                    throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");
                _contrasena = value;
            }
        }

        public string RutaImagen
        {
            get { return _rutaImagen; }
            set
            {
                _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_admin.png" : value.Trim();
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

        // Versión A (Sin parámetros externos): Valida las credenciales usando los datos internos del objeto
        public bool ValidarCredenciales()
        {
            return !string.IsNullOrEmpty(this._correo) && !string.IsNullOrEmpty(this._contrasena) && this._estado;
        }

        // Versión B (Con parámetro externo): Valida ingresando credenciales externas para verificar acceso temporal
        public bool ValidarCredenciales(string correoIngresado, string contrasenaIngresada)
        {
            if (!this._estado) return false; // Si está inactivo, no puede acceder
            return this._correo.Equals(correoIngresado.Trim(), StringComparison.OrdinalIgnoreCase) &&
                   this._contrasena.Equals(contrasenaIngresada);
        }
    }
}
    

