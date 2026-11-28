// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using System.Collections.Generic;

namespace SistemaPresatamos.Models
{
    // A4.8 - Encabezado del pedido (el "uno") con su lista de renglones (los "muchos").
    public class Pedido
    {
        public int IdPedido { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public string Cliente { get; set; } = string.Empty;

        public List<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();

        public decimal Total
        {
            get
            {
                decimal suma = 0m;
                foreach (DetallePedido linea in Detalles)
                    suma += linea.Subtotal;
                return suma;
            }
        }
    }
}
