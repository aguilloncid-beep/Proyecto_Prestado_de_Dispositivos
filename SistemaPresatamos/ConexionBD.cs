using System;
using MySql.Data.MySqlClient;

namespace SistemaPresatamos.Data
{
    public class ConexionBD
    {
        // Define tu cadena de conexión (reemplaza con los datos de tu servidor local o XAMPP)
        private readonly string cadenaConexion = "Server=localhost;Database=sistema_prestamos;Uid=root;Pwd=;";

        // Método para abrir y retornar la conexión
        public MySqlConnection ObtenerConexion()
        {
            try
            {
                var conexion = new MySqlConnection(cadenaConexion);
                return conexion;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al intentar conectar con la base de datos: " + ex.Message);
            }
        }
    }
}

	

