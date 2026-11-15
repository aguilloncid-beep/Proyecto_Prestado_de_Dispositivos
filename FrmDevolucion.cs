using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    /// <summary>
    /// Registro de la entrega real del material. Compara la fecha real con la fecha límite para identificar
    /// retraso y deja el material en el estado en que se recibió (Disponible, Dañado o Mantenimiento).
    /// </summary>
    public class FrmDevolucion : Form
    {
        private readonly PrestamoDAO _prestamos = new PrestamoDAO();
        private readonly DevolucionDAO _devoluciones = new DevolucionDAO();

        private readonly TextBox txtBuscar;
        private readonly DataGridView grid;
        private readonly Label lblResumen;
        private readonly DateTimePicker dtpReal;
        private readonly ComboBox cmbEstadoMaterial;
        private readonly TextBox txtObservaciones;

        private int _idSeleccionado;
        private DateTime? _limiteSeleccionado;

        public FrmDevolucion()
        {
            Sesion.ExigirPersonal();

            Text = "Registrar devolución";
            ClientSize = new Size(900, 580 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;

            Ui.Etiqueta(this, "Buscar alumno o material:", 20, 18);
            txtBuscar = Ui.Caja(this, 175, 15, 220);
            Ui.Boton(this, "Buscar", 405, 13, 90, (s, e) => Cargar());

            grid = Ui.Tabla(this, 20, 50, 860, 250);
            grid.CellClick += Grid_CellClick;

            lblResumen = Ui.Etiqueta(this, "Seleccione un préstamo de la tabla.", 20, 315);

            Ui.Etiqueta(this, "Fecha real de entrega:", 20, 355);
            dtpReal = new DateTimePicker();
            dtpReal.Format = DateTimePickerFormat.Short;
            dtpReal.Location = new Point(190, 352);
            dtpReal.Width = 130;
            dtpReal.MaxDate = DateTime.Today;
            dtpReal.Value = DateTime.Today;
            dtpReal.ValueChanged += (s, e) => ActualizarResumen();
            Controls.Add(dtpReal);

            Ui.Etiqueta(this, "Estado del material recibido:", 20, 395);
            cmbEstadoMaterial = Ui.Combo(this, 190, 392, 160, EstadosMaterial.AlDevolver);
            cmbEstadoMaterial.SelectedItem = EstadosMaterial.Disponible;

            Ui.Etiqueta(this, "Observaciones:", 420, 355);
            txtObservaciones = Ui.Caja(this, 420, 375, 460);
            txtObservaciones.Multiline = true;
            txtObservaciones.Height = 70;

            Ui.Boton(this, "Registrar devolución", 20, 470, 200, BtnRegistrar_Click);
            Ui.Boton(this, "Cerrar", 770, 535, 110, (s, e) => Close()).Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Autoria.Agregar(this);
            Load += (s, e) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                _prestamos.ActualizarMoras();
                Ui.Enlazar(grid, _prestamos.Consultar(txtBuscar.Text, null, null, null, true, null));
                _idSeleccionado = 0;
                _limiteSeleccionado = null;
                lblResumen.Text = "Seleccione un préstamo de la tabla.";
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = grid.Rows[e.RowIndex];
            _idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
            _limiteSeleccionado = Convert.ToDateTime(fila.Cells["Limite"].Value);
            lblResumen.Tag = "Préstamo #" + _idSeleccionado + " | " + Ui.Texto(fila, "Alumno") + " | " +
                             Ui.Texto(fila, "Material") + " | Límite: " + _limiteSeleccionado.Value.ToShortDateString();
            ActualizarResumen();
        }

        private void ActualizarResumen()
        {
            if (_idSeleccionado == 0 || !_limiteSeleccionado.HasValue)
                return;

            int retraso = (dtpReal.Value.Date - _limiteSeleccionado.Value.Date).Days;
            string texto = Convert.ToString(lblResumen.Tag) ?? string.Empty;

            if (retraso > 0)
            {
                lblResumen.Text = texto + " | RETRASO: " + retraso + " día(s)";
                lblResumen.ForeColor = Color.Firebrick;
            }
            else
            {
                lblResumen.Text = texto + " | En tiempo";
                lblResumen.ForeColor = SystemColors.ControlText;
            }
        }

        private void BtnRegistrar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_idSeleccionado == 0)
                {
                    Ui.Aviso(this, "Seleccione primero el préstamo que se está devolviendo.");
                    return;
                }

                string estadoMaterial = cmbEstadoMaterial.SelectedItem?.ToString() ?? EstadosMaterial.Disponible;

                if (!Ui.Confirmar(this, "¿Registrar la devolución del préstamo #" + _idSeleccionado +
                                        " con el material en estado «" + estadoMaterial + "»?"))
                    return;

                ResultadoDevolucion resultado = _devoluciones.RegistrarDevolucion(
                    _idSeleccionado, dtpReal.Value, estadoMaterial, txtObservaciones.Text);

                string mensaje = "Devolución registrada (#" + resultado.IdDevolucion + "). El material quedó en estado " +
                                 resultado.EstadoMaterial + ".";
                if (resultado.ConRetraso)
                    mensaje += "\n\nSe entregó con " + resultado.DiasRetraso + " día(s) de retraso; queda constancia en el historial.";

                Ui.Aviso(this, mensaje);
                txtObservaciones.Clear();
                dtpReal.Value = DateTime.Today;
                cmbEstadoMaterial.SelectedItem = EstadosMaterial.Disponible;
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
                Cargar();
            }
        }
    }
}
