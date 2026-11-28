// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;

namespace SistemaPresatamos.Models
{
    public class Material : EntidadBase
    {
        public const int DiasMaximosPrestamo = 7;

        private string? _numeroSerie;
        private string _nombre = string.Empty;
        private string _categoria = CategoriasMaterial.Otro;
        private string _estado = EstadosMaterial.Disponible;
        private string? _descripcion;
        private string? _rutaImagen;
        private decimal _precio;
        private int _stock = 1;

        public Material() : base() { }

        public Material(int id, string? numeroSerie, string nombre, string categoria, string estado,
                        string? descripcion, DateTime fechaRegistro, string? rutaImagen = null)
            : base(id, fechaRegistro, true)
        {
            NumeroSerie = numeroSerie;
            Nombre = nombre;
            Categoria = categoria;
            Estado = estado;
            Descripcion = descripcion;
            RutaImagen = rutaImagen;
        }

        public string? NumeroSerie
        {
            get { return _numeroSerie; }
            set { _numeroSerie = string.IsNullOrWhiteSpace(value) ? null : value.Trim(); }
        }

        public string Nombre
        {
            get { return _nombre; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("El nombre del material no puede estar vacío.");
                _nombre = value.Trim();
            }
        }

        public string Categoria
        {
            get { return _categoria; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("La categoría no puede estar vacía.");
                _categoria = value.Trim();
            }
        }

        public int? IdCategoria { get; set; }

        public decimal Precio
        {
            get { return _precio; }
            set
            {
                if (value < 0) throw new ArgumentException("El precio no puede ser negativo.");
                _precio = value;
            }
        }

        public int Stock
        {
            get { return _stock; }
            set
            {
                if (value < 0) throw new ArgumentException("El stock no puede ser negativo.");
                _stock = value;
            }
        }

        public string Estado
        {
            get { return _estado; }
            set
            {
                if (!EstadosMaterial.EsValido(value))
                    throw new ArgumentException("Estado de material inválido: " + value);
                _estado = value;
                EsActivo = value != EstadosMaterial.Baja;
            }
        }

        public string? Descripcion
        {
            get { return _descripcion; }
            set { _descripcion = string.IsNullOrWhiteSpace(value) ? null : value.Trim(); }
        }

        public string? RutaImagen
        {
            get { return _rutaImagen; }
            set { _rutaImagen = string.IsNullOrWhiteSpace(value) ? null : value.Trim(); }
        }

        public override string ToString()
        {
            string serie = _numeroSerie == null ? "s/n" : _numeroSerie;
            return $"Material: {_nombre} [Serie: {serie}] - {_categoria} - Estado: {_estado}";
        }

        public static bool DiasPrestamoValidos(int dias)
        {
            return dias > 0 && dias <= DiasMaximosPrestamo;
        }

        public bool VerificarDisponibilidad()
        {
            return this._estado == EstadosMaterial.Disponible;
        }

        public bool VerificarDisponibilidad(int diasPrestamoSolicitados)
        {
            return VerificarDisponibilidad() && DiasPrestamoValidos(diasPrestamoSolicitados);
        }
    }
}
