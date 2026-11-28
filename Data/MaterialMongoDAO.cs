// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos.Data
{
    public class MaterialMongoDAO
    {
        private const string NombreColeccion = "materiales";

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  Recibe el material ya validado, con RutaImagen = ruta relativa final (copiada por GestorImagenes) e
        //  idRelacional = Id generado en MariaDB, para que ambos motores queden sincronizados.
        // =====================================================================
        public async Task<ObjectId> InsertarAsync(Material material, int idRelacional)
        {
            Sesion.ExigirPersonal();
            if (material == null) throw new ArgumentNullException(nameof(material));
            if (material.IdCategoria == null)
                throw new InvalidOperationException("El material no tiene una categoría seleccionada.");

            ObjectId idDocumento = ObjectId.GenerateNewId();
            BsonDocument documento = ConstruirDocumento(material, idRelacional, idDocumento, DateTime.UtcNow);

            await ColeccionMateriales().InsertOneAsync(documento);
            return idDocumento;
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.5 - Actualización en MongoDB: se localiza el documento por idMaterial para obtener su _id (ObjectId),
        //  y ReplaceOneAsync reemplaza el documento filtrando por ese _id. Se conserva fechaRegistro.
        //  Si el material aún no tenía documento (creado antes de usar MongoDB) se inserta.
        // =====================================================================
        public async Task<ObjectId> ActualizarAsync(Material material)
        {
            Sesion.ExigirPersonal();
            if (material == null) throw new ArgumentNullException(nameof(material));
            if (material.IdCategoria == null)
                throw new InvalidOperationException("El material no tiene una categoría seleccionada.");

            BsonDocument? existente = await BuscarPorIdMaterialAsync(material.Id);
            if (existente == null)
                return await InsertarAsync(material, material.Id);

            ObjectId idDocumento = existente["_id"].AsObjectId;
            DateTime fecha = existente.Contains("fechaRegistro") && existente["fechaRegistro"].IsValidDateTime
                ? existente["fechaRegistro"].ToUniversalTime()
                : DateTime.UtcNow;

            BsonDocument reemplazo = ConstruirDocumento(material, material.Id, idDocumento, fecha);
            FilterDefinition<BsonDocument> filtro = Builders<BsonDocument>.Filter.Eq("_id", idDocumento);

            ReplaceOneResult resultado = await ColeccionMateriales().ReplaceOneAsync(filtro, reemplazo);
            if (resultado.MatchedCount == 0)
                throw new InvalidOperationException("MongoDB no encontró el documento a actualizar (_id " + idDocumento + ").");

            return idDocumento;
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.6 - Borrado LÓGICO en MongoDB: UpdateOneAsync con $set activo = false (el documento se conserva).
        // =====================================================================
        public async Task<bool> BajaLogicaAsync(int idMaterial)
        {
            Sesion.ExigirAdministrador();

            ObjectId? idDocumento = await ObtenerIdDocumentoAsync(idMaterial);
            if (idDocumento == null) return false;

            FilterDefinition<BsonDocument> filtro = Builders<BsonDocument>.Filter.Eq("_id", idDocumento.Value);
            UpdateDefinition<BsonDocument> cambio = Builders<BsonDocument>.Update
                .Set("activo", false)
                .Set("estado", EstadosMaterial.Baja);

            UpdateResult resultado = await ColeccionMateriales().UpdateOneAsync(filtro, cambio);
            return resultado.MatchedCount > 0;
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.6 - Borrado FÍSICO en MongoDB: DeleteOneAsync con filtro por _id (ObjectId); nunca un filtro vacío.
        // =====================================================================
        public async Task<bool> EliminarAsync(int idMaterial)
        {
            Sesion.ExigirAdministrador();

            ObjectId? idDocumento = await ObtenerIdDocumentoAsync(idMaterial);
            if (idDocumento == null) return false;

            FilterDefinition<BsonDocument> filtro = Builders<BsonDocument>.Filter.Eq("_id", idDocumento.Value);
            DeleteResult resultado = await ColeccionMateriales().DeleteOneAsync(filtro);
            return resultado.DeletedCount > 0;
        }

        // Listado de MongoDB que respeta el borrado lógico: solo documentos con activo = true.
        public async Task<List<BsonDocument>> ListarActivosAsync()
        {
            Sesion.ExigirSesion();

            FilterDefinition<BsonDocument> filtro = Builders<BsonDocument>.Filter.Eq("activo", true);
            return await ColeccionMateriales().Find(filtro).ToListAsync();
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.7 - Búsqueda en MongoDB: idMaterial exacto (igualdad) y nombre con expresión regular sin distinguir
        //  mayúsculas (equivalente a LIKE '%texto%'). El texto se escapa con Regex.Escape y solo se devuelven
        //  documentos con activo = true.
        // =====================================================================
        public async Task<DataTable> BuscarProductosAsync(int? id, string? nombre)
        {
            Sesion.ExigirSesion();

            FilterDefinitionBuilder<BsonDocument> f = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filtro = f.Eq("activo", true) & f.Eq("estado", EstadosMaterial.Disponible);

            if (id != null)
                filtro &= f.Eq("idMaterial", id.Value);

            string texto = (nombre ?? string.Empty).Trim();
            if (texto.Length > 0)
                filtro &= f.Regex("nombre", new BsonRegularExpression(Regex.Escape(texto), "i"));

            List<BsonDocument> documentos = await ColeccionMateriales().Find(filtro).Sort(Builders<BsonDocument>.Sort.Ascending("nombre")).ToListAsync();

            DataTable tabla = new DataTable();
            tabla.Columns.Add("Id", typeof(int));
            tabla.Columns.Add("Nombre", typeof(string));
            tabla.Columns.Add("Categoria", typeof(string));
            tabla.Columns.Add("Precio", typeof(decimal));
            tabla.Columns.Add("Stock", typeof(int));

            foreach (BsonDocument d in documentos)
            {
                tabla.Rows.Add(
                    d["idMaterial"].ToInt32(),
                    d["nombre"].AsString,
                    d.Contains("categoria") && d["categoria"].IsBsonDocument ? d["categoria"]["nombre"].AsString : string.Empty,
                    d.Contains("precio") && !d["precio"].IsBsonNull ? d["precio"].ToDecimal() : 0m,
                    d.Contains("stock") && !d["stock"].IsBsonNull ? d["stock"].ToInt32() : 0);
            }

            return tabla;
        }

        public async Task<ObjectId?> ObtenerIdDocumentoAsync(int idMaterial)
        {
            BsonDocument? documento = await BuscarPorIdMaterialAsync(idMaterial);
            return documento == null ? (ObjectId?)null : documento["_id"].AsObjectId;
        }

        private async Task<BsonDocument?> BuscarPorIdMaterialAsync(int idMaterial)
        {
            FilterDefinition<BsonDocument> filtro = Builders<BsonDocument>.Filter.Eq("idMaterial", idMaterial);
            return await ColeccionMateriales().Find(filtro).FirstOrDefaultAsync();
        }

        private static IMongoCollection<BsonDocument> ColeccionMateriales()
        {
            return ConexionMongoDB.ObtenerColeccion<BsonDocument>(NombreColeccion);
        }

        private static BsonDocument ConstruirDocumento(Material material, int idRelacional, ObjectId idDocumento, DateTime fechaRegistro)
        {
            return new BsonDocument
            {
                { "_id", idDocumento },
                { "idMaterial", idRelacional },
                { "numeroSerie", Texto(material.NumeroSerie) },
                { "nombre", material.Nombre },
                { "categoria", new BsonDocument { { "idCategoria", material.IdCategoria!.Value }, { "nombre", material.Categoria } } },
                { "estado", material.Estado },
                { "descripcion", Texto(material.Descripcion) },
                { "precio", new BsonDecimal128(material.Precio) },
                { "stock", material.Stock },
                { "activo", material.Estado != EstadosMaterial.Baja },
                { "rutaImagen", Texto(material.RutaImagen) },
                { "fechaRegistro", fechaRegistro }
            };
        }

        private static BsonValue Texto(string? valor)
        {
            return valor == null ? BsonNull.Value : new BsonString(valor);
        }
    }
}
