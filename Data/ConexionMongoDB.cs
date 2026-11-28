// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SistemaPresatamos.Data
{
    public static class ConexionMongoDB
    {
        private const string CadenaPorDefecto = "mongodb://localhost:27017";
        private const string NombreBaseDatos = "SistemaPrestamos";

        private static readonly object _candado = new object();
        private static MongoClient? _cliente;

        public static string Cadena
        {
            get
            {
                string? externa = Environment.GetEnvironmentVariable("SISTEMA_PRESTAMOS_MONGO");
                return string.IsNullOrWhiteSpace(externa) ? CadenaPorDefecto : externa;
            }
        }

        private static MongoClient ObtenerCliente()
        {
            if (_cliente == null)
            {
                lock (_candado)
                {
                    if (_cliente == null)
                    {
                        MongoClientSettings opciones = MongoClientSettings.FromConnectionString(Cadena);
                        opciones.ServerSelectionTimeout = TimeSpan.FromSeconds(3);
                        _cliente = new MongoClient(opciones);
                    }
                }
            }
            return _cliente;
        }

        public static IMongoDatabase ObtenerBaseDatos()
        {
            return ObtenerCliente().GetDatabase(NombreBaseDatos);
        }

        public static IMongoCollection<T> ObtenerColeccion<T>(string nombre)
        {
            return ObtenerBaseDatos().GetCollection<T>(nombre);
        }

        public static bool ProbarConexion(out string mensaje)
        {
            try
            {
                ObtenerBaseDatos().RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                mensaje = "Conexión exitosa con MongoDB.";
                return true;
            }
            catch (MongoAuthenticationException ex)
            {
                mensaje = "MongoDB rechazó las credenciales: " + ex.Message;
            }
            catch (TimeoutException)
            {
                mensaje = "No se encontró el servidor MongoDB (tiempo agotado). Verifique que esté encendido y la dirección/puerto.";
            }
            catch (MongoConnectionException ex)
            {
                mensaje = "Error de red al conectar con MongoDB: " + ex.Message;
            }
            catch (MongoException ex)
            {
                mensaje = "Error de MongoDB: " + ex.Message;
            }
            return false;
        }
    }
}
