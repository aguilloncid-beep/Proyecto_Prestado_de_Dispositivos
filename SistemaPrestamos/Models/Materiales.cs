using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaPresatamos.Models
{
        public class Materiales
        {
        // ==========================================
        // 3. CAMPOS PRIVADOS (Ocultamiento de información)
        // ==========================================
        private int _idMaterial = 0;
        private string _numeroSerie = string.Empty;
        private string _nombre = string.Empty;
        private string _rutaImagen = "default_material.png";
        private bool _estado = true; // True = Disponible/Activo, False = No disponible

        // ==========================================
        // 4. CONSTRUCTORES (Por defecto y parametrizado)
        // ==========================================

        // Constructor por defecto
        public Materiales()
            {
                _idMaterial = 0;
                _numeroSerie = string.Empty;
                _nombre = string.Empty;
                _rutaImagen = "default_material.png";
                _estado = true; // Disponible por defecto
            }

            // Constructor parametrizado
            public Materiales(int idMaterial, string numeroSerie, string nombre, string rutaImagen, bool estado)
            {
                IdMaterial = idMaterial;
                NumeroSerie = numeroSerie;
                Nombre = nombre;
                RutaImagen = rutaImagen;
                Estado = estado;
            }

        public override string ToString()
        {
            return $"Materiales: {_nombre} [Serie: {_numeroSerie}] - Estado: {_estado}";
        }

        // ==========================================
        // 3. PROPIEDADES PÚBLICAS CON FILTROS DE VALIDACIÓN (Encapsulamiento)
        // ==========================================
        public int IdMaterial
            {
                get { return _idMaterial; }
                set
                {
                    if (value < 0) throw new ArgumentException("El ID del material no puede ser negativo.");
                    _idMaterial = value;
                }
            }

            public string NumeroSerie
            {
                get { return _numeroSerie; }
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                        throw new ArgumentException("El número de serie no puede estar vacío.");
                    _numeroSerie = value.Trim();
                }
            }

            public string Nombre
            {
                get { return _nombre; }
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                        throw new ArgumentException("El nombre del material no puede estar vacío.");
                    _nombre = value.Trim();
                }
            }

            public string RutaImagen
            {
                get { return _rutaImagen; }
                set
                {
                    _rutaImagen = string.IsNullOrWhiteSpace(value) ? "default_material.png" : value.Trim();
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

            // Versión A (Sin parámetros externos): Verifica si el material está listo y disponible para préstamo
            public bool VerificarDisponibilidad()
            {
                return this._estado;
            }

            // Versión B (Con parámetro externo): Valida la disponibilidad considerando un límite de días de préstamo solicitados
            public bool VerificarDisponibilidad(int diasPrestamoSolicitados)
            {
                const int maxDiasPermitidos = 7; // Regla de negocio: máx. 7 días para material académico
                if (!this._estado) return false;

                return diasPrestamoSolicitados > 0 && diasPrestamoSolicitados <= maxDiasPermitidos;
            }
        }
    }



