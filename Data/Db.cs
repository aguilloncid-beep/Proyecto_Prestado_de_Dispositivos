using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace SistemaPresatamos.Data
{
    internal static class Db
    {
        public static MySqlParameter P(string nombre, object? valor)
        {
            return new MySqlParameter(nombre, valor ?? DBNull.Value);
        }

        public static DataTable Tabla(string sql, params MySqlParameter[] parametros)
        {
            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            using (MySqlCommand comando = new MySqlCommand(sql, conexion))
            {
                comando.Parameters.AddRange(parametros);
                using (MySqlDataAdapter adaptador = new MySqlDataAdapter(comando))
                {
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);
                    return tabla;
                }
            }
        }

        public static long Escalar(string sql, params MySqlParameter[] parametros)
        {
            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            using (MySqlCommand comando = new MySqlCommand(sql, conexion))
            {
                comando.Parameters.AddRange(parametros);
                conexion.Open();
                object? resultado = comando.ExecuteScalar();
                return (resultado == null || resultado == DBNull.Value) ? 0 : Convert.ToInt64(resultado);
            }
        }

        public static string? TextoONulo(IDataRecord r, string columna)
        {
            int i = r.GetOrdinal(columna);
            return r.IsDBNull(i) ? null : Convert.ToString(r.GetValue(i));
        }

        public static int Entero(IDataRecord r, string columna)
        {
            return Convert.ToInt32(r.GetValue(r.GetOrdinal(columna)));
        }

        public static decimal Decimal(IDataRecord r, string columna)
        {
            return Convert.ToDecimal(r.GetValue(r.GetOrdinal(columna)));
        }

        public static DateTime Fecha(IDataRecord r, string columna)
        {
            return Convert.ToDateTime(r.GetValue(r.GetOrdinal(columna)));
        }

        public static DateTime? FechaONula(IDataRecord r, string columna)
        {
            int i = r.GetOrdinal(columna);
            return r.IsDBNull(i) ? (DateTime?)null : Convert.ToDateTime(r.GetValue(i));
        }

        public static bool Booleano(IDataRecord r, string columna)
        {
            return Convert.ToBoolean(r.GetValue(r.GetOrdinal(columna)));
        }
    }
}
