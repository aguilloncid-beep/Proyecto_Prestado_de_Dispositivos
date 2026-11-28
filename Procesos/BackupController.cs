// =====================================================================
//  Autoría - Equipo 4
//  Integrantes en orden alfabético por primer apellido
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Xml.Serialization;
using SistemaPresatamos.Data;
using SistemaPresatamos.Models;

namespace SistemaPresatamos.Procesos
{
    public static class BackupController
    {
        public const string NombreCarpeta = "Backups";

        private static readonly JsonSerializerOptions OpcionesJson = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public static string CarpetaRespaldos
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NombreCarpeta); }
        }

        public static List<MaterialRespaldo> ObtenerMateriales()
        {
            DataTable tabla = new MaterialDAO().Listar(string.Empty, string.Empty, string.Empty, true);
            List<MaterialRespaldo> lista = new List<MaterialRespaldo>();

            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new MaterialRespaldo
                {
                    IdMaterial = Convert.ToInt32(fila["Id"]),
                    NumeroSerie = Texto(fila, "Serie"),
                    Nombre = Texto(fila, "Nombre") ?? string.Empty,
                    Categoria = Texto(fila, "Categoria") ?? string.Empty,
                    Estado = Texto(fila, "Estado") ?? string.Empty,
                    Descripcion = Texto(fila, "Descripcion"),
                    RutaImagen = Texto(fila, "RutaImagen")
                });
            }
            return lista;
        }

        public static List<PrestamoRespaldo> ObtenerPrestamos()
        {
            DataTable tabla = new PrestamoDAO().Consultar(null, null, null, null, false, null);
            List<PrestamoRespaldo> lista = new List<PrestamoRespaldo>();

            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new PrestamoRespaldo
                {
                    IdPrestamo = Convert.ToInt32(fila["Id"]),
                    Alumno = Texto(fila, "Alumno") ?? string.Empty,
                    NumeroEstudiante = Texto(fila, "No. estudiante"),
                    Material = Texto(fila, "Material") ?? string.Empty,
                    Serie = Texto(fila, "Serie"),
                    FechaPrestamo = Convert.ToDateTime(fila["Prestamo"]),
                    FechaLimite = Convert.ToDateTime(fila["Limite"]),
                    FechaDevolucionReal = fila["Entrega real"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(fila["Entrega real"]),
                    Estado = Texto(fila, "Estado") ?? string.Empty,
                    DiasRetraso = Convert.ToInt32(fila["Dias de retraso"])
                });
            }
            return lista;
        }

        public static bool TryExportarJson<T>(List<T> datos, string nombreBase, out string? rutaArchivo, out string error)
        {
            string json = JsonSerializer.Serialize(datos, OpcionesJson);
            return TryEscribir(NombreArchivo(nombreBase, ".json"), flujo => flujo.Write(json), out rutaArchivo, out error);
        }

        public static bool TryExportarXml<T>(List<T> datos, string nombreBase, out string? rutaArchivo, out string error)
        {
            XmlSerializer serializador = new XmlSerializer(typeof(List<T>));
            return TryEscribir(NombreArchivo(nombreBase, ".xml"), flujo => serializador.Serialize(flujo, datos), out rutaArchivo, out error);
        }

        public static bool TryImportarJson<T>(string ruta, out List<T>? lista, out string error) where T : IRegistroRespaldo
        {
            lista = null;
            if (!TryLeer(ruta, out string texto, out error))
                return false;

            try
            {
                lista = JsonSerializer.Deserialize<List<T>>(texto, OpcionesJson);
            }
            catch (JsonException ex)
            {
                error = "El archivo no es un JSON válido o no cumple con el esquema de " + Descripcion<T>() +
                        ".\n\nDetalle: " + ex.Message;
                return false;
            }

            return Validar(ref lista, out error);
        }

        public static bool TryImportarXml<T>(string ruta, out List<T>? lista, out string error) where T : IRegistroRespaldo
        {
            lista = null;
            if (!TryLeer(ruta, out string texto, out error))
                return false;

            try
            {
                XmlSerializer serializador = new XmlSerializer(typeof(List<T>));
                using (StringReader lector = new StringReader(texto))
                {
                    lista = serializador.Deserialize(lector) as List<T>;
                }
            }
            catch (InvalidOperationException ex)
            {
                string detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                error = "El archivo no es un XML válido o no cumple con el esquema de " + Descripcion<T>() +
                        ".\n\nDetalle: " + detalle;
                return false;
            }

            return Validar(ref lista, out error);
        }

        public static bool AsegurarCarpeta(out string error)
        {
            error = string.Empty;

            try
            {
                if (!Directory.Exists(CarpetaRespaldos))
                    Directory.CreateDirectory(CarpetaRespaldos);

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                error = "No hay permisos para crear la carpeta de respaldos:\n" + CarpetaRespaldos;
                return false;
            }
            catch (IOException ex)
            {
                error = "No se pudo crear la carpeta de respaldos: " + ex.Message;
                return false;
            }
        }

        private static bool TryEscribir(string nombreArchivo, Action<StreamWriter> escritura, out string? rutaArchivo, out string error)
        {
            rutaArchivo = null;
            if (!AsegurarCarpeta(out error))
                return false;

            string ruta = Path.Combine(CarpetaRespaldos, nombreArchivo);

            try
            {
                using (StreamWriter flujo = new StreamWriter(ruta, false, new UTF8Encoding(false)))
                {
                    escritura(flujo);
                }
                rutaArchivo = ruta;
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                error = "El sistema operativo negó el acceso: no hay permisos de escritura en\n" + CarpetaRespaldos;
            }
            catch (IOException ex)
            {
                error = "No se pudo escribir el respaldo; el archivo puede estar bloqueado o el disco no responde.\n\n" + ex.Message;
            }
            return false;
        }

        private static bool TryLeer(string ruta, out string texto, out string error)
        {
            texto = string.Empty;
            error = string.Empty;

            try
            {
                texto = File.ReadAllText(ruta, Encoding.UTF8);
                return true;
            }
            catch (FileNotFoundException)
            {
                error = "El archivo seleccionado no existe:\n" + ruta;
            }
            catch (UnauthorizedAccessException)
            {
                error = "No hay permisos para leer el archivo seleccionado.";
            }
            catch (IOException ex)
            {
                error = "No se pudo leer el archivo; puede estar abierto por otro programa.\n\n" + ex.Message;
            }
            return false;
        }

        private static bool Validar<T>(ref List<T>? lista, out string error) where T : IRegistroRespaldo
        {
            error = string.Empty;

            if (lista == null)
            {
                error = "El archivo no contiene una lista de " + Descripcion<T>() + ".";
                return false;
            }

            for (int i = 0; i < lista.Count; i++)
            {
                if (lista[i] == null || !lista[i].EsValido())
                {
                    error = "El registro " + (i + 1) + " del archivo no corresponde a " + Descripcion<T>() +
                            ". Verifique que eligió el tipo de datos correcto.";
                    lista = null;
                    return false;
                }
            }
            return true;
        }

        private static string Descripcion<T>()
        {
            return typeof(T) == typeof(MaterialRespaldo) ? "materiales" : "préstamos";
        }

        private static string NombreArchivo(string nombreBase, string extension)
        {
            return nombreBase + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + extension;
        }

        private static string? Texto(DataRow fila, string columna)
        {
            return fila[columna] == DBNull.Value ? null : Convert.ToString(fila[columna]);
        }
    }
}
