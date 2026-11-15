using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    /// <summary>
    /// Ventana principal del administrador. Está separada de la vista del alumno (FrmPanelAlumno) y
    /// exige rol Administrador, así un alumno no puede alterar el inventario.
    /// </summary>
    public class FrmPanelAdmin : Form
    {
        private readonly Label lblAvisos;

        public FrmPanelAdmin()
        {
            Sesion.ExigirAdministrador();

            Text = "Panel del administrador";
            ClientSize = new Size(440, 510 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Label titulo = Ui.Etiqueta(this, "Bienvenido, " + Sesion.UsuarioActual!.Nombre, 25, 20);
            titulo.Font = new Font(Font.FontFamily, 12f, FontStyle.Bold);
            Ui.Etiqueta(this, "Rol: " + Sesion.UsuarioActual.Rol, 25, 48);

            lblAvisos = Ui.Etiqueta(this, string.Empty, 25, 72);

            int y = 105;
            Ui.Boton(this, "Inventario de materiales (alta, modificación, baja)", 25, y, 390, (s, e) => Abrir(() => new FrmInventario()));
            y += 40;
            Ui.Boton(this, "Registrar préstamo", 25, y, 390, (s, e) => Abrir(() => new FrmPrestamos()));
            y += 40;
            Ui.Boton(this, "Registrar devolución", 25, y, 390, (s, e) => Abrir(() => new FrmDevolucion()));
            y += 40;
            Ui.Boton(this, "Historial de préstamos y devoluciones", 25, y, 390, (s, e) => Abrir(() => new FrmHistorial()));
            y += 40;
            Ui.Boton(this, "Reporte de préstamos (segundo plano)", 25, y, 390, (s, e) => Abrir(() => new FrmReportes()));
            y += 40;
            Ui.Boton(this, "Usuarios (alumnos y administradores)", 25, y, 390, (s, e) => Abrir(() => new FrmUsuario()));
            y += 40;
            Ui.Boton(this, "Módulos adicionales (apartados, reparaciones...)", 25, y, 390, (s, e) => Abrir(() => new Form1()));
            y += 55;
            Ui.Boton(this, "Cerrar sesión", 25, y, 390, BtnCerrarSesion_Click);

            Autoria.Agregar(this);
            Load += (s, e) => ActualizarAvisos();
        }

        private void Abrir(Func<Form> fabrica)
        {
            try
            {
                using (Form ventana = fabrica())
                {
                    ventana.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }

            ActualizarAvisos();
        }

        private void ActualizarAvisos()
        {
            try
            {
                PrestamoDAO dao = new PrestamoDAO();
                dao.ActualizarMoras();
                int enMora = dao.ContarEnMora();
                lblAvisos.Text = enMora == 0 ? "Sin préstamos en mora." : "Atención: " + enMora + " préstamo(s) en mora.";
                lblAvisos.ForeColor = enMora == 0 ? SystemColors.ControlText : Color.Firebrick;
            }
            catch (Exception ex)
            {
                lblAvisos.Text = "No se pudo consultar la base de datos: " + ex.Message;
                lblAvisos.ForeColor = Color.Firebrick;
            }
        }

        private void BtnCerrarSesion_Click(object? sender, EventArgs e)
        {
            Sesion.CerrarSesionSolicitada = true;
            Close();
        }
    }
}
