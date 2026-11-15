using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    /// <summary>Pide correo y contraseña, valida contra el hash guardado y deja el usuario (con su rol) en Sesion.</summary>
    public class LoginForm : Form
    {
        private const int IntentosMaximos = 5;

        private readonly TextBox txtCorreo;
        private readonly TextBox txtPassword;
        private readonly Label lblEstado;
        private int _intentos;

        public LoginForm()
        {
            Text = "Sistema de Préstamo de Materiales - Iniciar sesión";
            ClientSize = new Size(380, 280 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Label titulo = Ui.Etiqueta(this, "Iniciar sesión", 25, 20);
            titulo.Font = new Font(Font.FontFamily, 14f, FontStyle.Bold);

            Ui.Etiqueta(this, "Correo:", 25, 70);
            txtCorreo = Ui.Caja(this, 25, 90, 330);

            Ui.Etiqueta(this, "Contraseña:", 25, 125);
            txtPassword = Ui.Caja(this, 25, 145, 330, true);

            Button btnIngresar = Ui.Boton(this, "Ingresar", 25, 190, 160, BtnIngresar_Click);
            Ui.Boton(this, "Registrarme (alumno)", 195, 190, 160, BtnRegistrar_Click);

            lblEstado = Ui.Etiqueta(this, string.Empty, 25, 235);
            lblEstado.ForeColor = Color.Firebrick;

            AcceptButton = btnIngresar;
            Autoria.Agregar(this);
        }

        private void BtnIngresar_Click(object? sender, EventArgs e)
        {
            try
            {
                Usuario? usuario = new UsuarioDAO().Autenticar(txtCorreo.Text, txtPassword.Text);

                if (usuario == null)
                {
                    _intentos++;
                    txtPassword.Clear();

                    if (_intentos >= IntentosMaximos)
                    {
                        MessageBox.Show(this, "Demasiados intentos fallidos. La aplicación se cerrará.",
                                        "Sistema de Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        DialogResult = DialogResult.Cancel;
                        return;
                    }

                    lblEstado.Text = "Correo o contraseña incorrectos (" + _intentos + "/" + IntentosMaximos + ").";
                    txtPassword.Focus();
                    return;
                }

                Sesion.Iniciar(usuario);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void BtnRegistrar_Click(object? sender, EventArgs e)
        {
            using (FrmRegistroAlumno registro = new FrmRegistroAlumno())
            {
                if (registro.ShowDialog(this) == DialogResult.OK)
                {
                    txtCorreo.Text = registro.CorreoRegistrado;
                    txtPassword.Clear();
                    txtPassword.Focus();
                    lblEstado.Text = string.Empty;
                }
            }
        }
    }
}
