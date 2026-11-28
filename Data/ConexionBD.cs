using System;
using MySql.Data.MySqlClient;

namespace SistemaPresatamos.Data
{
    public static class ConexionBD
    {
        private const string CadenaPorDefecto =
            "Server=localhost;" +
            "Port=3306;" +
            "Database=SistemaPrestamos;" +
            "Uid=app_prestamos;" +
            "Pwd=Prestamos2026!;" +
            "CharSet=utf8mb4;";

        public static string Cadena
        {
            get
            {
                string? externa = Environment.GetEnvironmentVariable("SISTEMA_PRESTAMOS_CONN");
                return string.IsNullOrWhiteSpace(externa) ? CadenaPorDefecto : externa;
            }
        }

        public static MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(Cadena);
        }

        public static bool ProbarConexion(out string mensaje)
        {
            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    conexion.Open();
                }
                mensaje = "Conexión exitosa con la base de datos.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "No se pudo conectar con la base de datos: " + ex.Message;
                return false;
            }
        }
    }
}
