using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    /// <summary>
    /// Historial permanente de préstamos y devoluciones leído de la base de datos
    /// (no depende de la pila en memoria). Búsqueda por alumno, material, fecha o estado.
    /// </summary>
    public class FrmHistorial : Form
    {
        private const string TodosLosEstados = "(Todos)";

        private readonly PrestamoDAO _dao = new PrestamoDAO();

        private readonly TextBox txtBuscar;
        private readonly ComboBox cmbEstado;
        private readonly CheckBox chkDesde;
        private readonly DateTimePicker dtpDesde;
        private readonly CheckBox chkHasta;
        private readonly DateTimePicker dtpHasta;
        private readonly DataGridView grid;
        private readonly Label lblTotal;

        public FrmHistorial()
        {
            Sesion.ExigirSesion();

            Text = "Historial de préstamos y devoluciones";
            ClientSize = new Size(1000, 580 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;

            Ui.Etiqueta(this, "Alumno o material:", 20, 18);
            txtBuscar = Ui.Caja(this, 135, 15, 180);

            Ui.Etiqueta(this, "Estado:", 330, 18);
            List<string> estados = new List<string> { TodosLosEstados };
            estados.AddRange(EstadosPrestamo.Todos);
            cmbEstado = Ui.Combo(this, 380, 15, 110, estados);
            cmbEstado.SelectedIndex = 0;

            chkDesde = new CheckBox();
            chkDesde.Text = "Desde";
            chkDesde.AutoSize = true;
            chkDesde.Location = new Point(510, 17);
            Controls.Add(chkDesde);

            dtpDesde = new DateTimePicker();
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(570, 15);
            dtpDesde.Width = 105;
            dtpDesde.Value = DateTime.Today.AddMonths(-1);
            Controls.Add(dtpDesde);

            chkHasta = new CheckBox();
            chkHasta.Text = "Hasta";
            chkHasta.AutoSize = true;
            chkHasta.Location = new Point(690, 17);
            Controls.Add(chkHasta);

            dtpHasta = new DateTimePicker();
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(748, 15);
            dtpHasta.Width = 105;
            Controls.Add(dtpHasta);

            Ui.Boton(this, "Buscar", 870, 13, 110, (s, e) => Cargar());

            grid = Ui.Tabla(this, 20, 55, 960, 465);
            grid.DataBindingComplete += (s, e) => ResaltarRetrasos();

            lblTotal = Ui.Etiqueta(this, string.Empty, 20, 540);
            lblTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Ui.Boton(this, "Cerrar", 870, 536, 110, (s, e) => Close()).Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Autoria.Agregar(this);
            Load += (s, e) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                _dao.ActualizarMoras();

                string estado = cmbEstado.SelectedIndex <= 0 ? string.Empty : cmbEstado.Text;
                DateTime? desde = chkDesde.Checked ? dtpDesde.Value.Date : (DateTime?)null;
                DateTime? hasta = chkHasta.Checked ? dtpHasta.Value.Date : (DateTime?)null;

                var tabla = _dao.Consultar(txtBuscar.Text, estado, desde, hasta, false, null);
                Ui.Enlazar(grid, tabla);
                lblTotal.Text = "Registros: " + tabla.Rows.Count + "   (en rojo: préstamos con retraso o en mora)";
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void ResaltarRetrasos()
        {
            if (!grid.Columns.Contains("Dias de retraso")) return;

            foreach (DataGridViewRow fila in grid.Rows)
            {
                int dias = 0;
                int.TryParse(Ui.Texto(fila, "Dias de retraso"), out dias);

                if (dias > 0 || Ui.Texto(fila, "Estado") == EstadosPrestamo.Mora)
                    fila.DefaultCellStyle.ForeColor = Color.Firebrick;
            }
        }
    }
}
