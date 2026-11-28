// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using System.Drawing;
using System.IO;

namespace SistemaPresatamos.Archivos
{
    public static class GestorImagenes
    {
        public const string NombreCarpeta = "ImagenesDatos";
        public const long TamanoMaximoBytes = 5L * 1024 * 1024;
        public const string FiltroDialogo = "Archivos de imagen (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

        private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png" };

        public static string CarpetaDestino
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NombreCarpeta); }
        }

        public static bool AsegurarCarpeta(out string error)
        {
            error = string.Empty;

            try
            {
                if (!Directory.Exists(CarpetaDestino))
                    Directory.CreateDirectory(CarpetaDestino);

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                error = "No hay permisos para crear la carpeta de imágenes:\n" + CarpetaDestino +
                        "\n\nEjecute la aplicación desde una ubicación con permisos de escritura.";
                return false;
            }
            catch (IOException ex)
            {
                error = "No se pudo crear la carpeta de imágenes (disco o ruta no disponible):\n" + ex.Message;
                return false;
            }
        }

        public static bool ValidarArchivo(string? ruta, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(ruta))
            {
                error = "No se seleccionó ningún archivo.";
                return false;
            }

            string extension = Path.GetExtension(ruta).ToLowerInvariant();
            if (Array.IndexOf(ExtensionesPermitidas, extension) < 0)
            {
                error = "Solo se admiten imágenes .jpg, .jpeg o .png.";
                return false;
            }

            try
            {
                FileInfo info = new FileInfo(ruta);
                if (!info.Exists)
                {
                    error = "El archivo seleccionado ya no existe:\n" + ruta;
                    return false;
                }

                if (info.Length > TamanoMaximoBytes)
                {
                    error = "La imagen pesa más de " + (TamanoMaximoBytes / (1024 * 1024)) + " MB. Elija una más ligera.";
                    return false;
                }
            }
            catch (UnauthorizedAccessException)
            {
                error = "No hay permisos para leer el archivo seleccionado.";
                return false;
            }
            catch (IOException ex)
            {
                error = "No se pudo leer el archivo: " + ex.Message;
                return false;
            }

            return true;
        }

        public static bool TryGuardar(string rutaOrigen, out string? rutaRelativa, out string error)
        {
            rutaRelativa = null;

            if (!ValidarArchivo(rutaOrigen, out error))
                return false;

            if (!AsegurarCarpeta(out error))
                return false;

            string nombreUnico = Guid.NewGuid().ToString() + Path.GetExtension(rutaOrigen).ToLowerInvariant();
            string rutaDestino = Path.Combine(CarpetaDestino, nombreUnico);

            try
            {
                File.Copy(rutaOrigen, rutaDestino, true);
                rutaRelativa = Path.Combine(NombreCarpeta, nombreUnico);
                return true;
            }
            catch (FileNotFoundException)
            {
                error = "El archivo de origen no se encontró (¿se movió o se eliminó?):\n" + rutaOrigen;
            }
            catch (UnauthorizedAccessException)
            {
                error = "El sistema operativo negó el acceso: no hay permisos de escritura en\n" + CarpetaDestino;
            }
            catch (IOException ex)
            {
                error = "No se pudo copiar la imagen; el archivo puede estar bloqueado por otro programa o el disco no responde.\n\n" +
                        ex.Message;
            }

            return false;
        }

        public static bool TryCargarVistaPrevia(string rutaCompleta, out Image? imagen, out string error)
        {
            imagen = null;
            error = string.Empty;

            try
            {
                using (Image original = Image.FromFile(rutaCompleta))
                {
                    imagen = new Bitmap(original);
                }
                return true;
            }
            catch (FileNotFoundException)
            {
                error = "No se encontró la imagen:\n" + rutaCompleta;
            }
            catch (UnauthorizedAccessException)
            {
                error = "No hay permisos para leer la imagen.";
            }
            catch (IOException ex)
            {
                error = "No se pudo leer la imagen: " + ex.Message;
            }
            catch (OutOfMemoryException)
            {
                error = "El archivo no es una imagen válida o está dañado.";
            }

            return false;
        }

        public static string? ResolverRuta(string? rutaRelativa)
        {
            if (string.IsNullOrWhiteSpace(rutaRelativa))
                return null;

            string completa = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rutaRelativa));
            string carpeta = Path.GetFullPath(CarpetaDestino) + Path.DirectorySeparatorChar;

            return completa.StartsWith(carpeta, StringComparison.OrdinalIgnoreCase) ? completa : null;
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.6 - Destrucción física de la imagen: comprueba que el archivo no esté en uso (apertura exclusiva)
        //  y solo entonces invoca File.Delete.
        // =====================================================================
        public static bool TryEliminar(string? rutaRelativa, out string mensaje)
        {
            mensaje = string.Empty;

            string? completa = ResolverRuta(rutaRelativa);
            if (completa == null)
            {
                mensaje = "El registro no tenía imagen asociada.";
                return true;
            }

            if (!File.Exists(completa))
            {
                mensaje = "El archivo de imagen ya no existía en el disco.";
                return true;
            }

            try
            {
                using (new FileStream(completa, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }

                File.Delete(completa);
                mensaje = "La imagen se eliminó del disco.";
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                mensaje = "No hay permisos para eliminar la imagen del disco:\n" + completa;
            }
            catch (IOException ex)
            {
                mensaje = "La imagen está siendo usada por otro proceso y no se pudo eliminar: " + ex.Message;
            }

            return false;
        }

        public static void EliminarSiExiste(string? rutaRelativa)
        {
            string? completa = ResolverRuta(rutaRelativa);
            if (completa == null) return;

            try
            {
                if (File.Exists(completa))
                    File.Delete(completa);
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }
        }
    }
}
