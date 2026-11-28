// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos.Data
{
    public class PedidoMongoDAO
    {
        private const string NombreColeccion = "pedidos";

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.8 - Persistencia documental: el pedido se guarda como UN solo documento que incluye sus renglones
        //  como arreglo de subdocumentos embebidos. InsertOneAsync inserta la relación 1:N en una sola operación
        //  de red (sin colección separada de detalles). idPedido es el Id generado en MariaDB.
        // =====================================================================
        public async Task<ObjectId> InsertarAsync(Pedido pedido, int idPedidoRelacional)
        {
            Sesion.ExigirPersonal();
            if (pedido == null) throw new ArgumentNullException(nameof(pedido));

            ObjectId idDocumento = ObjectId.GenerateNewId();

            BsonArray renglones = new BsonArray();
            foreach (DetallePedido linea in pedido.Detalles)
            {
                renglones.Add(new BsonDocument
                {
                    { "idMaterial", linea.IdMaterial },
                    { "nombre", linea.Nombre },
                    { "cantidad", linea.Cantidad },
                    { "precioUnitario", new BsonDecimal128(linea.PrecioUnitario) },
                    { "subtotal", new BsonDecimal128(linea.Subtotal) }
                });
            }

            BsonDocument documento = new BsonDocument
            {
                { "_id", idDocumento },
                { "idPedido", idPedidoRelacional },
                { "fecha", pedido.Fecha.ToUniversalTime() },
                { "cliente", pedido.Cliente.Trim() },
                { "total", new BsonDecimal128(pedido.Total) },
                { "detalles", renglones }
            };

            await Coleccion().InsertOneAsync(documento);
            return idDocumento;
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.8 - Descuento de stock en la colección de materiales: $inc con valor negativo por cada renglón
        //  (si el material no tiene documento en MongoDB, simplemente no se modifica nada).
        // =====================================================================
        public async Task<int> DescontarStockAsync(Pedido pedido)
        {
            Sesion.ExigirPersonal();
            if (pedido == null) throw new ArgumentNullException(nameof(pedido));

            IMongoCollection<BsonDocument> materiales = ConexionMongoDB.ObtenerColeccion<BsonDocument>("materiales");
            int actualizados = 0;

            foreach (DetallePedido linea in pedido.Detalles)
            {
                FilterDefinition<BsonDocument> filtro = Builders<BsonDocument>.Filter.Eq("idMaterial", linea.IdMaterial);
                UpdateDefinition<BsonDocument> cambio = Builders<BsonDocument>.Update.Inc("stock", -linea.Cantidad);

                UpdateResult resultado = await materiales.UpdateOneAsync(filtro, cambio);
                if (resultado.ModifiedCount > 0) actualizados++;
            }

            return actualizados;
        }

        public async Task<DataTable> ListarEncabezadosAsync()
        {
            Sesion.ExigirSesion();

            List<BsonDocument> documentos = await Coleccion().Find(FilterDefinition<BsonDocument>.Empty)
                .Sort(Builders<BsonDocument>.Sort.Combine(Builders<BsonDocument>.Sort.Ascending("fecha"), Builders<BsonDocument>.Sort.Ascending("idPedido")))
                .ToListAsync();

            DataTable tabla = new DataTable();
            tabla.Columns.Add("Id", typeof(int));
            tabla.Columns.Add("Fecha", typeof(DateTime));
            tabla.Columns.Add("Cliente", typeof(string));
            tabla.Columns.Add("Total", typeof(decimal));
            tabla.Columns.Add("Renglones", typeof(int));

            foreach (BsonDocument d in documentos)
            {
                int renglones = d.Contains("detalles") && d["detalles"].IsBsonArray ? d["detalles"].AsBsonArray.Count : 0;
                tabla.Rows.Add(
                    d["idPedido"].ToInt32(),
                    d["fecha"].ToLocalTime(),
                    d["cliente"].AsString,
                    d["total"].ToDecimal(),
                    renglones);
            }

            return tabla;
        }

        // Detalle bajo demanda: se extrae el arreglo embebido del documento del pedido seleccionado.
        public async Task<DataTable> ListarDetallesAsync(int idPedido)
        {
            Sesion.ExigirSesion();

            FilterDefinition<BsonDocument> filtro = Builders<BsonDocument>.Filter.Eq("idPedido", idPedido);
            BsonDocument? documento = await Coleccion().Find(filtro).FirstOrDefaultAsync();

            DataTable tabla = new DataTable();
            tabla.Columns.Add("Id", typeof(int));
            tabla.Columns.Add("Nombre", typeof(string));
            tabla.Columns.Add("Cantidad", typeof(int));
            tabla.Columns.Add("PrecioUnitario", typeof(decimal));
            tabla.Columns.Add("Subtotal", typeof(decimal));

            if (documento == null || !documento.Contains("detalles") || !documento["detalles"].IsBsonArray)
                return tabla;

            foreach (BsonValue valor in documento["detalles"].AsBsonArray)
            {
                BsonDocument renglon = valor.AsBsonDocument;
                tabla.Rows.Add(
                    renglon["idMaterial"].ToInt32(),
                    renglon["nombre"].AsString,
                    renglon["cantidad"].ToInt32(),
                    renglon["precioUnitario"].ToDecimal(),
                    renglon["subtotal"].ToDecimal());
            }

            return tabla;
        }

        private static IMongoCollection<BsonDocument> Coleccion()
        {
            return ConexionMongoDB.ObtenerColeccion<BsonDocument>(NombreColeccion);
        }
    }
}
