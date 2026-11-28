using System;

namespace SistemaPresatamos.Models
{
    public class Usuario : EntidadBase
    {
        public const int LimitePrestamosActivos = 3;

        private string _nombre = string.Empty;
        private string _correo = string.Empty;
        private string? _numeroEstudiante;
        private string? _carrera;
        private string _passwordHash = string.Empty;
        private string _rol = Roles.Alumno;

        public Usuario() : base() { }

        public Usuario(int id, string nombre, string correo, string? numeroEstudiante, string? carrera,
                       string passwordHash, string rol, DateTime fechaRegistro, bool esActivo)
            : base(id, fechaRegistro, esActivo)
        {
            Nombre = nombre;
            Correo = correo;
            NumeroEstudiante = numeroEstudiante;
            Carrera = carrera;
            PasswordHash = passwordHash;
            Rol = rol;
        }

        public string Nombre
        {
            get { return _nombre; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("El nombre no puede estar vacío.");
                _nombre = value.Trim();
            }
        }

        public string Correo
        {
            get { return _correo; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("El correo no puede estar vacío.");
                string limpio = value.Trim().ToLowerInvariant();
                int arroba = limpio.IndexOf('@');
                if (arroba <= 0 || arroba != limpio.LastIndexOf('@') || limpio.IndexOf('.', arroba) < arroba + 2 || limpio.EndsWith("."))
                    throw new ArgumentException("El correo no tiene un formato válido.");
                _correo = limpio;
            }
        }

        public string? NumeroEstudiante
        {
            get { return _numeroEstudiante; }
            set { _numeroEstudiante = string.IsNullOrWhiteSpace(value) ? null : value.Trim(); }
        }

        public string? Carrera
        {
            get { return _carrera; }
            set { _carrera = string.IsNullOrWhiteSpace(value) ? null : value.Trim(); }
        }

        public string PasswordHash
        {
            get { return _passwordHash; }
            set { _passwordHash = value ?? string.Empty; }
        }

        public string Rol
        {
            get { return _rol; }
            set
            {
                if (!Roles.EsValido(value)) throw new ArgumentException("Rol inválido. Use Alumno o Administrador.");
                _rol = value;
            }
        }

        public bool EsAdministrador { get { return _rol == Roles.Administrador; } }

        public override string ToString()
        {
            string num = _numeroEstudiante == null ? "s/n" : _numeroEstudiante;
            return $"Usuario: {_nombre} ({num}) - Rol: {_rol} - Activo: {EsActivo}";
        }

        public bool ValidarElegibilidadPrestamo()
        {
            return this.EsActivo && !string.IsNullOrEmpty(this._numeroEstudiante);
        }

        public bool ValidarElegibilidadPrestamo(int prestamosActivosActuales)
        {
            if (!this.EsActivo) return false;
            return prestamosActivosActuales < LimitePrestamosActivos;
        }
    }
}
