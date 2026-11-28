using System;

namespace SistemaPresatamos.Models
{
    public static class Roles
    {
        public const string Alumno = "Alumno";
        public const string Administrador = "Administrador";
        public static readonly string[] Todos = { Alumno, Administrador };

        public static bool EsValido(string? valor)
        {
            return valor != null && Array.IndexOf(Todos, valor) >= 0;
        }
    }

    public static class EstadosMaterial
    {
        public const string Disponible = "Disponible";
        public const string Prestado = "Prestado";
        public const string Mantenimiento = "Mantenimiento";
        public const string Danado = "Dañado";
        public const string Baja = "Baja";

        public static readonly string[] Todos = { Disponible, Prestado, Mantenimiento, Danado, Baja };

        public static readonly string[] AlDevolver = { Disponible, Danado, Mantenimiento };

        public static bool EsValido(string? valor)
        {
            return valor != null && Array.IndexOf(Todos, valor) >= 0;
        }
    }

    public static class EstadosPrestamo
    {
        public const string Activo = "Activo";
        public const string Finalizado = "Finalizado";
        public const string Mora = "Mora";

        public static readonly string[] Todos = { Activo, Finalizado, Mora };

        public static bool EsValido(string? valor)
        {
            return valor != null && Array.IndexOf(Todos, valor) >= 0;
        }
    }

    public static class CategoriasMaterial
    {
        public const string Computo = "Equipo de cómputo";
        public const string Didactico = "Material didáctico";
        public const string Recreativo = "Recreativo";
        public const string Otro = "Otro";

        public static readonly string[] Todas = { Computo, Didactico, Recreativo, Otro };
    }
}
