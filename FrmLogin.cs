// =====================================================================
//  Autoría - Equipo 0 (sustituya por los datos reales; ver Autoria.cs)
//    APELLIDO1 Apellido2 Nombre(s) - Integrante 1
//    APELLIDO1 Apellido2 Nombre(s) - Integrante 2
//    APELLIDO1 Apellido2 Nombre(s) - Integrante 3
// =====================================================================
using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    /// <summary>
    /// Pantalla de acceso. Valida usuario y contraseña contra la colección estática UsuariosSistema (en memoria,
    /// sin base de datos), guarda el resultado en SesionSistema y responde con DialogResult.OK.
    /// El botón «Acceso de alumnos» conserva el acceso con base de datos del proyecto para los alumnos.
    /// </summary>
    public class FrmLogin : Form
    {
        private const int IntentosMaximos = 5;

        private readonly TextBox txtUsuario;
        private readonly TextBox txtContrasena;
        private readonly Button btnIngresar;
        private readonly Button btnSalir;
        private readonly Label lblEstado;
        private int _intentos;

        public FrmLogin()
        {
            Text = "Sistema de Préstamo de Materiales - Acceso";
            ClientSize = new Size(380, 320 + Autoria.AlturaLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            Label titulo = Ui.Etiqueta(this, "Iniciar sesión", 25, 20);
            titulo.Font = new Font(Font.FontFamily, 14f, FontStyle.Bold);

            Ui.Etiqueta(this, "Usuario:", 25, 70);
            txtUsuario = Ui.Caja(this, 25, 90, 330);
            txtUsuario.Name = "txtUsuario";

            Ui.Etiqueta(this, "Contraseña:", 25, 125);
            txtContrasena = Ui.Caja(this, 25, 145, 330, true);
            txtContrasena.Name = "txtContrasena";
            txtContrasena.UseSystemPasswordChar = true;

            btnIngresar = Ui.Boton(this, "Ingresar", 25, 190, 160, BtnIngresar_Click);
            btnIngresar.Name = "btnIngresar";
            btnSalir = Ui.Boton(this, "Salir", 195, 190, 160, BtnSalir_Click);
            btnSalir.Name = "btnSalir";

            Ui.Boton(this, "Acceso de alumnos (base de datos)", 25, 235, 330, BtnAlumnos_Click);

            lblEstado = Ui.Etiqueta(this, string.Empty, 25, 280);
            lblEstado.ForeColor = Color.Firebrick;

            AcceptButton = btnIngresar;
            CancelButton = btnSalir;

            Autoria.Agregar(this);
        }

        private void BtnIngresar_Click(object? sender, EventArgs e)
        {
            UsuarioEstatico? usuario;

            if (UsuariosSistema.Validar(txtUsuario.Text, txtContrasena.Text, out usuario) && usuario != null)
            {
                // La sesión queda disponible para FrmPrincipal y sus ventanas hijas sin pasar parámetros.
                SesionSistema.Iniciar(usuario.NombreCompleto, usuario.Rol);
                this.DialogResult = DialogResult.OK;
                Close();
                return;
            }

            _intentos++;
            txtContrasena.Clear();

            if (_intentos >= IntentosMaximos)
            {
                MessageBox.Show(this, "Demasiados intentos fallidos. La aplicación se cerrará.",
                                "Sistema de Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            lblEstado.Text = "Usuario o contraseña incorrectos (" + _intentos + "/" + IntentosMaximos + ").";
            MessageBox.Show(this, "El usuario o la contraseña son incorrectos.", "Acceso denegado",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtContrasena.Focus();
        }

        private void BtnSalir_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void BtnAlumnos_Click(object? sender, EventArgs e)
        {
            using (LoginForm acceso = new LoginForm())
            {
                if (acceso.ShowDialog(this) == DialogResult.OK && Sesion.UsuarioActual != null)
                {
                    SesionSistema.Iniciar(Sesion.UsuarioActual.Nombre, Sesion.UsuarioActual.Rol);
                    this.DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }
    }
}
