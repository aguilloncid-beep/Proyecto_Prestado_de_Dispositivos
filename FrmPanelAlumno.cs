using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    /// <summary>
    /// Vista del alumno: solo consulta sus propios préstamos y los materiales disponibles.
    /// No contiene ninguna función para modificar inventario, usuarios ni préstamos.
    /// </summary>
    public class FrmPanelAlumno : Form
    {
        private readonly DataGridView gridPrestamos;
        private readonly DataGridView gridMateriales;
        private readonly Label lblResumen;

        public FrmPanelAlumno()
        {
            Sesion.ExigirSesion();

            Text = "Panel del alumno";
            ClientSize = new Size(900, 560 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterScreen;

            Label titulo = Ui.Etiqueta(this, "Hola, " + Sesion.UsuarioActual!.Nombre, 20, 15);
            titulo.Font = new Font(Font.FontFamily, 12f, FontStyle.Bold);

            lblResumen = Ui.Etiqueta(this, string.Empty, 20, 42);

            Ui.Etiqueta(this, "Mis préstamos", 20, 75);
            gridPrestamos = Ui.Tabla(this, 20, 95, 860, 190);

            Ui.Etiqueta(this, "Materiales disponibles ahora", 20, 300);
            gridMateriales = Ui.Tabla(this, 20, 320, 860, 180);

            Button btnActualizar = Ui.Boton(this, "Actualizar", 20, 515, 120, (s, e) => Cargar());
            btnActualizar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Button btnSalir = Ui.Boton(this, "Cerrar sesión", 760, 515, 120, BtnCerrarSesion_Click);
            btnSalir.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Autoria.Agregar(this);
            Load += (s, e) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                new PrestamoDAO().ActualizarMoras();

                var prestamos = new PrestamoDAO().Consultar(null, null, null, null, false, null);
                Ui.Enlazar(gridPrestamos, prestamos);

                // El alumno no necesita ver su propio nombre en cada fila.
                if (gridPrestamos.Columns.Contains("Alumno")) gridPrestamos.Columns["Alumno"].Visible = false;
                if (gridPrestamos.Columns.Contains("No. estudiante")) gridPrestamos.Columns["No. estudiante"].Visible = false;

                var disponibles = new MaterialDAO().ListarDisponibles();
                Ui.Enlazar(gridMateriales, disponibles);
                if (gridMateriales.Columns.Contains("IdMaterial")) gridMateriales.Columns["IdMaterial"].Visible = false;
                if (gridMateriales.Columns.Contains("Texto")) gridMateriales.Columns["Texto"].Visible = false;

                int abiertos = 0;
                int mora = 0;
                foreach (System.Data.DataRow fila in prestamos.Rows)
                {
                    string estado = Convert.ToString(fila["Estado"]) ?? string.Empty;
                    if (estado == "Activo" || estado == "Mora") abiertos++;
                    if (estado == "Mora") mora++;
                }

                lblResumen.Text = "Préstamos abiertos: " + abiertos + (mora > 0 ? "   |   En mora: " + mora : string.Empty);
                lblResumen.ForeColor = mora > 0 ? Color.Firebrick : SystemColors.ControlText;
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void BtnCerrarSesion_Click(object? sender, EventArgs e)
        {
            Sesion.CerrarSesionSolicitada = true;
            Close();
        }
    }
}
