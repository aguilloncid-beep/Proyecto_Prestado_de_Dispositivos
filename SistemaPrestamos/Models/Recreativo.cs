using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPresatamos.Models
{
    internal class Recreativo
    {
        // ==========================================
        // 3. CAMPOS PRIVADOS (Ocultamiento de información)
        // ==========================================
        private int _idRecrea = 0;
        private string _numero = string.Empty;
        private string _nombre = string.Empty;
        private string _modelo = string.Empty;
        private string _rutaImagen = "default_recreativo.png";
        private bool _estado = true; // True = Disponible/Activo, False = En uso/Inactivo

        // ==========================================
        // 4. CONSTRUCTORES (Por defecto y parametrizado)
        // ==========================================

        // Constructor por defecto
        public Recreativo()
        {
            _idRecrea = 0;
            _numero = string.Empty;
            _nombre = string.Empty;
            _modelo = string.Empty;
            _rutaImagen = "default_recreativo.png";
            _estado = true; // Disponible por defecto
        }

        // Constructor parametrizado
        public Recreativo(int idRecrea, string numero, string nombre, string modelo, string rutaImagen, bool estado)
        {
            IdRecrea = idRecrea;
            Numero = numero;
            Nombre = nombre;
            Modelo = modelo;
            RutaImagen = rutaImagen;
            Estado = estado;
        }

        public override string ToString()
        {
            return $"Recreativo: {_nombre} - Modelo: {_modelo} (# {_numero})";
        }

        // ==========================================
        // 3. PROPIEDADES PÚBLICAS CON FILTROS DE VALIDACIÓN (Encapsulamiento)
        // ==========================================
        public int IdRecrea
        {
            get { return _idRecrea; }
            set
            {
                if (value < 0) throw new ArgumentException("El ID recreativo no puede ser negativo.");
                _idRecrea = value;
            }
        }

        public string Numero
        {
            get { return _numero; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El número de identificación no puede estar vacío.");
                _numero = value.Trim();
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

        public string Modelo
        {
            get { return _modelo; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El modelo no puede estar vacío.");
                _modelo = value.Trim();
            }
        }

        public string RutaImagen
        {
            get { return _rutaImagen; }
            set
            {
                _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_recreativo.png" : value.Trim();
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

        // Versión A (Sin parámetros externos): Verifica si el artículo recreativo está disponible
        public bool ValidarDisponibilidad()
        {
            return this._estado;
        }

        // Versión B (Con parámetro externo): Valida la disponibilidad según un límite de horas solicitadas (ej. uso temporal corto)
        public bool ValidarDisponibilidad(int horasPrestamoSolicitadas)
        {
            const int maxHorasPermitidas = 4; // Regla de negocio: máx. 4 horas para materiales recreativos
            if (!this._estado) return false;

            return horasPrestamoSolicitadas > 0 && horasPrestamoSolicitadas <= maxHorasPermitidas;
        }
    }
}
    

