// =====================================================================
//  Autoría - Equipo 4
//  Integrantes en orden alfabético por primer apellido
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos.Data
{
    public class ConsultaPrestamoDAO
    {
        // =================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid | FLORES Becerra Cid Hector | GOVEA Mijares Jorge Luis
        // =================================================================
        public List<PrestamoVista> ListarDetalle(string? texto)
        {
            Sesion.ExigirSesion();

            int idUsuario = (!Sesion.EsPersonal && Sesion.UsuarioActual != null) ? Sesion.UsuarioActual.Id : 0;

            const string sql = @"
                SELECT p.IdPrestamo,
                       u.Nombre           AS Alumno,
                       u.NumeroEstudiante AS NumeroEstudiante,
                       u.Carrera          AS Carrera,
                       m.Nombre           AS Material,
                       m.Categoria        AS Categoria,
                       m.NumeroSerie      AS NumeroSerie,
                       m.RutaImagen       AS RutaImagen,
                       p.FechaPrestamo,
                       p.FechaLimite,
                       d.FechaReal        AS FechaDevolucion,
                       d.Observaciones    AS Observaciones,
                       p.Estado
                FROM Prestamo p
                INNER JOIN Usuario    u ON u.IdUsuario  = p.IdUsuario
                INNER JOIN Material   m ON m.IdMaterial = p.IdMaterial
                LEFT  JOIN Devolucion d ON d.IdPrestamo = p.IdPrestamo
                WHERE (u.Nombre LIKE @p OR u.NumeroEstudiante LIKE @p OR m.Nombre LIKE @p
                       OR m.NumeroSerie LIKE @p OR m.Categoria LIKE @p)
                  AND (@usuario = 0 OR p.IdUsuario = @usuario)
                ORDER BY p.FechaPrestamo DESC, p.IdPrestamo DESC;";

            List<PrestamoVista> lista = new List<PrestamoVista>();

            using (MySqlConnection conexion = ConexionBD.ObtenerConexion())
            using (MySqlCommand comando = new MySqlCommand(sql, conexion))
            {
                comando.Parameters.Add(Db.P("@p", "%" + (texto ?? string.Empty).Trim() + "%"));
                comando.Parameters.Add(Db.P("@usuario", idUsuario));
                conexion.Open();

                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(new PrestamoVista
                        {
                            IdPrestamo = Db.Entero(lector, "IdPrestamo"),
                            Alumno = Db.TextoONulo(lector, "Alumno") ?? string.Empty,
                            NumeroEstudiante = Db.TextoONulo(lector, "NumeroEstudiante"),
                            Carrera = Db.TextoONulo(lector, "Carrera"),
                            Material = Db.TextoONulo(lector, "Material") ?? string.Empty,
                            Categoria = Db.TextoONulo(lector, "Categoria") ?? string.Empty,
                            NumeroSerie = Db.TextoONulo(lector, "NumeroSerie"),
                            RutaImagen = Db.TextoONulo(lector, "RutaImagen"),
                            FechaPrestamo = Db.Fecha(lector, "FechaPrestamo"),
                            FechaLimite = Db.Fecha(lector, "FechaLimite"),
                            FechaDevolucion = Db.FechaONula(lector, "FechaDevolucion"),
                            Observaciones = Db.TextoONulo(lector, "Observaciones"),
                            Estado = Db.TextoONulo(lector, "Estado") ?? string.Empty
                        });
                    }
                }
            }

            return lista;
        }
    }
}
