// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    // A4.8 - Historial de pedidos (comprobantes): encabezados arriba, renglones del pedido seleccionado abajo.
    public class FrmHistorialPedidos : Form
    {
        private const string MotorMariaDb = "MariaDB";
        private const string MotorMongo = "MongoDB";

        private readonly PedidoDAO _dao = new PedidoDAO();
        private readonly PedidoMongoDAO _mongo = new PedidoMongoDAO();

        private readonly ComboBox cmbMotor;
        private readonly DataGridView gridPedidos;
        private readonly DataGridView gridDetalles;
        private readonly Label lblDetalles;

        public FrmHistorialPedidos()
        {
            Sesion.ExigirSesion();

            Text = "Historial de pedidos";
            ClientSize = new Size(900, 520 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;

            Ui.Etiqueta(this, "Motor:", 20, 18);
            cmbMotor = Ui.Combo(this, 70, 15, 110, new[] { MotorMariaDb, MotorMongo });
            cmbMotor.SelectedIndex = 0;
            Ui.Boton(this, "Cargar historial", 200, 13, 140, (s, e) => CargarEncabezados());

            Ui.Etiqueta(this, "Pedidos registrados (ordenados cronológicamente)", 20, 55);
            gridPedidos = Ui.Tabla(this, 20, 75, 860, 190);
            gridPedidos.CellClick += GridPedidos_CellClick;

            lblDetalles = Ui.Etiqueta(this, "Detalle del pedido: haga clic en un pedido de la tabla superior", 20, 280);
            gridDetalles = Ui.Tabla(this, 20, 300, 860, 170);

            Ui.Boton(this, "Cerrar", 770, 480, 110, (s, e) => Close());

            Autoria.Agregar(this);
            Load += (s, e) => CargarEncabezados();
        }

        private async void CargarEncabezados()
        {
            try
            {
                DataTable encabezados = cmbMotor.Text == MotorMongo
                    ? await _mongo.ListarEncabezadosAsync()
                    : _dao.ListarEncabezados();

                Ui.Enlazar(gridPedidos, encabezados);
                if (gridPedidos.Columns.Contains("Total"))
                    gridPedidos.Columns["Total"].DefaultCellStyle.Format = "C2";
                if (gridPedidos.Columns.Contains("Fecha"))
                    gridPedidos.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";

                gridDetalles.DataSource = null;
                lblDetalles.Text = "Detalle del pedido: haga clic en un pedido de la tabla superior";
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.8 - Carga bajo demanda: al seleccionar un pedido se consultan SOLO sus renglones
        //  (INNER JOIN filtrado por IdPedido en MariaDB, o arreglo embebido del documento en MongoDB).
        // =====================================================================
        private async void GridPedidos_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (!int.TryParse(Ui.Texto(gridPedidos.Rows[e.RowIndex], "Id"), out int idPedido)) return;

            try
            {
                DataTable detalles = cmbMotor.Text == MotorMongo
                    ? await _mongo.ListarDetallesAsync(idPedido)
                    : _dao.ListarDetalles(idPedido);

                Ui.Enlazar(gridDetalles, detalles);
                foreach (string columna in new[] { "PrecioUnitario", "Subtotal" })
                {
                    if (gridDetalles.Columns.Contains(columna))
                        gridDetalles.Columns[columna].DefaultCellStyle.Format = "C2";
                }

                lblDetalles.Text = "Detalle del pedido " + idPedido + " (" + detalles.Rows.Count + " renglón(es))";
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }
    }
}
