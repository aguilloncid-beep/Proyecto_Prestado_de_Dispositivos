// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;

namespace SistemaPresatamos.Models
{
    public class DetallePedido
    {
        public int IdMaterial { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public decimal PrecioUnitario { get; set; }

        public int Cantidad { get; set; }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.7 - Subtotal = Cantidad * Precio unitario. Es una propiedad calculada: siempre refleja la cantidad
        //  vigente, por lo que al editar la cantidad en la rejilla basta con refrescarla.
        // =====================================================================
        public decimal Subtotal
        {
            get { return Cantidad * PrecioUnitario; }
        }
    }
}
