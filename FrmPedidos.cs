// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    // A4.7 - Carrito de préstamo (adaptado al proyecto): el alumno reúne varios materiales con su cantidad. El precio
    // unitario es el valor de reposición del material y el total es el valor en garantía de la solicitud (sin guardar aún).
    public class FrmPedidos : Form
    {
        private const string MotorMariaDb = "MariaDB";
        private const string MotorMongo = "MongoDB";

        private readonly MaterialDAO _dao = new MaterialDAO();
        private readonly MaterialMongoDAO _mongo = new MaterialMongoDAO();
        private readonly PedidoDAO _pedidos = new PedidoDAO();
        private readonly PedidoMongoDAO _pedidosMongo = new PedidoMongoDAO();
        private readonly BindingList<DetallePedido> _carrito = new BindingList<DetallePedido>();

        private readonly TextBox txtId;
        private readonly TextBox txtNombre;
        private readonly ComboBox cmbMotor;
        private readonly DataGridView gridProductos;
        private readonly TextBox txtCantidad;
        private readonly DataGridView gridCarrito;
        private readonly Label lblTotalVenta;
        private readonly TextBox txtCliente;
        private readonly Button btnConfirmar;

        public FrmPedidos()
        {
            Sesion.ExigirSesion();

            Text = "Solicitud de préstamo - carrito de materiales";
            ClientSize = new Size(900, 600 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;

            Ui.Etiqueta(this, "Id (exacto):", 20, 18);
            txtId = Ui.Caja(this, 100, 15, 70);

            Ui.Etiqueta(this, "Nombre:", 190, 18);
            txtNombre = Ui.Caja(this, 245, 15, 220);

            Ui.Etiqueta(this, "Motor:", 485, 18);
            cmbMotor = Ui.Combo(this, 530, 15, 100, new[] { MotorMariaDb, MotorMongo });
            cmbMotor.SelectedIndex = 0;

            Ui.Boton(this, "Buscar", 650, 13, 90, BtnBuscar_Click);
            Ui.Boton(this, "Limpiar", 750, 13, 90, (s, e) =>
            {
                txtId.Clear();
                txtNombre.Clear();
                gridProductos.DataSource = null;
            });

            Ui.Etiqueta(this, "Materiales disponibles (resultados de la búsqueda)", 20, 48);
            gridProductos = Ui.Tabla(this, 20, 68, 860, 150);

            Ui.Etiqueta(this, "Cantidad (unidades):", 20, 233);
            txtCantidad = Ui.Caja(this, 135, 230, 50);
            txtCantidad.Text = "1";
            Button btnAnadir = Ui.Boton(this, "Añadir al Carrito", 200, 228, 140, BtnAnadir_Click);
            Ui.Etiqueta(this, "Precio unitario = valor de reposición del material. Subtotal = Cantidad x Precio unitario.", 360, 233);

            Ui.Etiqueta(this, "Carrito de préstamo (la columna Cantidad se puede editar)", 20, 270);
            gridCarrito = Ui.Tabla(this, 20, 290, 860, 200);
            gridCarrito.ReadOnly = false;
            gridCarrito.AutoGenerateColumns = true;
            gridCarrito.DataSource = _carrito;
            gridCarrito.DataError += (s, e) => { e.ThrowException = false; };
            gridCarrito.CellEndEdit += GridCarrito_CellEndEdit;
            ConfigurarColumnasCarrito();

            Button btnQuitar = Ui.Boton(this, "Quitar seleccionado", 20, 500, 150, BtnQuitar_Click);

            Label etiquetaTotal = Ui.Etiqueta(this, "TOTAL EN GARANTÍA:", 520, 505);
            etiquetaTotal.Font = new Font(Font, FontStyle.Bold);
            lblTotalVenta = Ui.Etiqueta(this, 0m.ToString("C2"), 680, 503);
            lblTotalVenta.Font = new Font(Font.FontFamily, 13f, FontStyle.Bold);

            Ui.Etiqueta(this, "Cliente:", 190, 508);
            txtCliente = Ui.Caja(this, 245, 505, 260);

            btnConfirmar = Ui.Boton(this, "Confirmar préstamo", 20, 555, 150, BtnConfirmar_Click);
            btnConfirmar.Enabled = Sesion.EsPersonal;

            // A20 - Invitado: solo lectura. Se bloquean los campos de captura y los botones que escriben.
            if (!Sesion.EsPersonal)
            {
                txtCantidad.Enabled = false;
                txtCliente.Enabled = false;
                btnAnadir.Enabled = false;
                btnQuitar.Enabled = false;
                gridCarrito.ReadOnly = true;
            }

            Ui.Boton(this, "Historial de pedidos", 190, 555, 170, (s, e) =>
            {
                using (FrmHistorialPedidos historial = new FrmHistorialPedidos())
                    historial.ShowDialog(this);
            });
            Ui.Boton(this, "Cerrar", 770, 555, 110, (s, e) => Close());

            Autoria.Agregar(this);
            Load += (s, e) => BuscarProductos();
        }

        private void ConfigurarColumnasCarrito()
        {
            foreach (DataGridViewColumn columna in gridCarrito.Columns)
                columna.ReadOnly = columna.DataPropertyName != nameof(DetallePedido.Cantidad);

            gridCarrito.Columns[nameof(DetallePedido.IdMaterial)].HeaderText = "Id";
            gridCarrito.Columns[nameof(DetallePedido.PrecioUnitario)].HeaderText = "Precio unitario";
            gridCarrito.Columns[nameof(DetallePedido.PrecioUnitario)].DefaultCellStyle.Format = "C2";
            gridCarrito.Columns[nameof(DetallePedido.Subtotal)].DefaultCellStyle.Format = "C2";
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.7 - Búsqueda de materiales disponibles: por Id exacto (llave primaria, vacío = sin filtro, así nunca hay
        //  referencias nulas) y por nombre con LIKE (MariaDB) o expresión regular (MongoDB). Todo parametrizado.
        // =====================================================================
        private void BtnBuscar_Click(object? sender, EventArgs e)
        {
            BuscarProductos();
        }

        private async void BuscarProductos()
        {
            try
            {
                int? id = null;
                string textoId = txtId.Text.Trim();
                if (textoId.Length > 0)
                {
                    if (!int.TryParse(textoId, NumberStyles.None, CultureInfo.InvariantCulture, out int valor) || valor <= 0)
                    {
                        Ui.Aviso(this, "El Id debe ser un número entero positivo.");
                        txtId.Focus();
                        return;
                    }
                    id = valor;
                }

                DataTable resultados = cmbMotor.Text == MotorMongo
                    ? await _mongo.BuscarProductosAsync(id, txtNombre.Text)
                    : _dao.BuscarProductos(id, txtNombre.Text);

                Ui.Enlazar(gridProductos, resultados);
                if (gridProductos.Columns.Contains("Precio"))
                    gridProductos.Columns["Precio"].DefaultCellStyle.Format = "C2";

                if (resultados.Rows.Count == 0)
                    Ui.Aviso(this, "No se encontraron materiales disponibles con esos criterios.");
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private int ObtenerStock(int idMaterial)
        {
            Material? material = _dao.ObtenerPorId(idMaterial);
            return material == null ? 0 : material.Stock;
        }

        private void AvisoStock(string nombre, int stock)
        {
            MessageBox.Show(this, "Stock insuficiente para «" + nombre + "». Unidades disponibles para préstamo: " + stock + ".",
                            "Sistema de Préstamos - Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void BtnAnadir_Click(object? sender, EventArgs e)
        {
            try
            {
                if (gridProductos.CurrentRow == null || gridProductos.CurrentRow.Index < 0 || gridProductos.SelectedRows.Count == 0)
                {
                    Ui.Aviso(this, "Seleccione un material de los resultados de la búsqueda.");
                    return;
                }

                if (!int.TryParse(txtCantidad.Text.Trim(), out int cantidad))
                {
                    Ui.Aviso(this, "La cantidad debe ser un número entero.");
                    txtCantidad.Focus();
                    return;
                }

                if (cantidad < 0)
                {
                    Ui.Aviso(this, "No se permiten cantidades negativas.");
                    txtCantidad.Focus();
                    return;
                }

                if (cantidad == 0)
                {
                    Ui.Aviso(this, "La cantidad debe ser al menos 1.");
                    txtCantidad.Focus();
                    return;
                }

                DataGridViewRow fila = gridProductos.SelectedRows[0];
                if (!int.TryParse(Ui.Texto(fila, "Id"), out int idMaterial)) return;

                string nombre = Ui.Texto(fila, "Nombre");
                decimal precio = decimal.TryParse(Ui.Texto(fila, "Precio"), out decimal p) ? p : 0m;

                // El stock se consulta en la base de datos en el momento de añadir, no del dato mostrado en pantalla.
                int stock = ObtenerStock(idMaterial);

                DetallePedido? existente = null;
                foreach (DetallePedido linea in _carrito)
                {
                    if (linea.IdMaterial == idMaterial) { existente = linea; break; }
                }

                int totalSolicitado = cantidad + (existente == null ? 0 : existente.Cantidad);
                if (totalSolicitado > stock)
                {
                    AvisoStock(nombre, stock);
                    return;
                }

                if (existente != null)
                {
                    existente.Cantidad = totalSolicitado;
                }
                else
                {
                    _carrito.Add(new DetallePedido
                    {
                        IdMaterial = idMaterial,
                        Nombre = nombre,
                        PrecioUnitario = precio,
                        Cantidad = cantidad
                    });
                }

                gridCarrito.Refresh();
                CalcularTotal();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void GridCarrito_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DetallePedido? linea = gridCarrito.Rows[e.RowIndex].DataBoundItem as DetallePedido;
                if (linea == null) return;

                int stock = ObtenerStock(linea.IdMaterial);

                if (linea.Cantidad < 0)
                {
                    Ui.Aviso(this, "No se permiten cantidades negativas.");
                    linea.Cantidad = 1;
                }
                else if (linea.Cantidad == 0)
                {
                    Ui.Aviso(this, "La cantidad debe ser al menos 1. Use «Quitar seleccionado» para eliminar la línea.");
                    linea.Cantidad = 1;
                }
                else if (linea.Cantidad > stock)
                {
                    AvisoStock(linea.Nombre, stock);
                    linea.Cantidad = Math.Max(1, stock);
                }

                gridCarrito.Refresh();
                CalcularTotal();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void BtnQuitar_Click(object? sender, EventArgs e)
        {
            if (gridCarrito.CurrentRow == null || gridCarrito.CurrentRow.DataBoundItem is not DetallePedido linea)
            {
                Ui.Aviso(this, "Seleccione una línea del carrito.");
                return;
            }

            _carrito.Remove(linea);
            CalcularTotal();
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.7 - Cálculo del total (valor en garantía del préstamo): se recorre la lista del carrito sumando Subtotal (Cantidad * Precio unitario)
        //  y se muestra con formato de moneda en lblTotalVenta. Se invoca al añadir, editar o quitar líneas.
        // =====================================================================
        private void CalcularTotal()
        {
            decimal total = 0m;
            foreach (DetallePedido linea in _carrito)
                total += linea.Subtotal;

            lblTotalVenta.Text = total.ToString("C2");
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.8 - Confirmación del pedido: valida el carrito y guarda de forma atómica en MariaDB (encabezado,
        //  renglones y descuento de stock en una sola transacción). Solo si esa transacción tuvo éxito se guarda
        //  el documento embebido en MongoDB y se descuenta el stock allí; si MariaDB falla no se toca MongoDB.
        // =====================================================================
        private async void BtnConfirmar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_carrito.Count == 0)
                {
                    Ui.Aviso(this, "El carrito está vacío. Añada al menos un material.");
                    return;
                }

                if (txtCliente.Text.Trim().Length == 0)
                {
                    Ui.Aviso(this, "Escriba el nombre del cliente.");
                    txtCliente.Focus();
                    return;
                }

                if (txtCliente.Text.Trim().Length > 120)
                {
                    Ui.Aviso(this, "El nombre del cliente no puede exceder 120 caracteres.");
                    txtCliente.Focus();
                    return;
                }

                Pedido pedido = new Pedido();
                pedido.Fecha = DateTime.Now;
                pedido.Cliente = txtCliente.Text.Trim();

                foreach (DetallePedido linea in _carrito)
                {
                    if (linea.Cantidad <= 0 || linea.PrecioUnitario < 0)
                    {
                        Ui.Aviso(this, "La línea de «" + linea.Nombre + "» tiene datos no válidos.");
                        return;
                    }

                    int stock = ObtenerStock(linea.IdMaterial);
                    if (linea.Cantidad > stock)
                    {
                        AvisoStock(linea.Nombre, stock);
                        return;
                    }

                    pedido.Detalles.Add(new DetallePedido
                    {
                        IdMaterial = linea.IdMaterial,
                        Nombre = linea.Nombre,
                        PrecioUnitario = linea.PrecioUnitario,
                        Cantidad = linea.Cantidad
                    });
                }

                btnConfirmar.Enabled = false;

                int idPedido = _pedidos.Guardar(pedido);

                string avisoMongo;
                try
                {
                    MongoDB.Bson.ObjectId idDocumento = await _pedidosMongo.InsertarAsync(pedido, idPedido);
                    int descontados = await _pedidosMongo.DescontarStockAsync(pedido);
                    avisoMongo = "\nTambién se guardó en MongoDB (ObjectId " + idDocumento + ") y se descontó el stock de " + descontados + " material(es).";
                }
                catch (Exception ex)
                {
                    avisoMongo = "\n\nAtención: el pedido quedó confirmado en MariaDB, pero NO se pudo guardar en MongoDB.\n" + ex.Message;
                }

                Ui.Aviso(this, "Pedido " + idPedido + " guardado en MariaDB (transacción confirmada, total " + pedido.Total.ToString("C2") + ")." + avisoMongo);

                _carrito.Clear();
                txtCliente.Clear();
                CalcularTotal();
                BuscarProductos();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
            finally
            {
                btnConfirmar.Enabled = Sesion.EsPersonal;
            }
        }
    }
}
