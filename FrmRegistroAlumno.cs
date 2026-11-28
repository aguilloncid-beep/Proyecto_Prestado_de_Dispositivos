using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Data;

namespace SistemaPresatamos
{
    public class FrmRegistroAlumno : Form
    {
        private readonly TextBox txtNombre;
        private readonly TextBox txtCorreo;
        private readonly TextBox txtNumero;
        private readonly ComboBox cmbCarrera;
        private readonly TextBox txtPassword;
        private readonly TextBox txtConfirmar;

        public string CorreoRegistrado { get; private set; } = string.Empty;

        public FrmRegistroAlumno()
        {
            Text = "Registro de alumno";
            ClientSize = new Size(400, 420 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Ui.Etiqueta(this, "Nombre completo:", 25, 20);
            txtNombre = Ui.Caja(this, 25, 40, 350);

            Ui.Etiqueta(this, "Correo:", 25, 75);
            txtCorreo = Ui.Caja(this, 25, 95, 350);

            Ui.Etiqueta(this, "Número de estudiante:", 25, 130);
            txtNumero = Ui.Caja(this, 25, 150, 350);

            Ui.Etiqueta(this, "Carrera:", 25, 185);
            cmbCarrera = Ui.Combo(this, 25, 205, 350, Ui.Carreras, false);

            Ui.Etiqueta(this, "Contraseña (mínimo 8, con letras y números):", 25, 240);
            txtPassword = Ui.Caja(this, 25, 260, 350, true);

            Ui.Etiqueta(this, "Confirmar contraseña:", 25, 295);
            txtConfirmar = Ui.Caja(this, 25, 315, 350, true);

            Button btnRegistrar = Ui.Boton(this, "Registrarme", 25, 365, 170, BtnRegistrar_Click);
            Button btnCancelar = Ui.Boton(this, "Cancelar", 205, 365, 170, (s, e) => DialogResult = DialogResult.Cancel);

            AcceptButton = btnRegistrar;
            CancelButton = btnCancelar;
            Autoria.Agregar(this);
        }

        private void BtnRegistrar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (txtPassword.Text != txtConfirmar.Text)
                    throw new ArgumentException("Las contraseñas no coinciden.");

                new UsuarioDAO().RegistrarAlumno(
                    txtNombre.Text,
                    txtCorreo.Text,
                    txtNumero.Text,
                    cmbCarrera.Text,
                    txtPassword.Text);

                CorreoRegistrado = txtCorreo.Text.Trim().ToLowerInvariant();
                Ui.Aviso(this, "Cuenta creada. Ya puede iniciar sesión.");
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }
    }
}
