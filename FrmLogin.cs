// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    public class FrmLogin : Form
    {
        private const int IntentosMaximos = 5;

        private readonly TextBox txtUsuario;
        private readonly TextBox txtContrasena;
        private readonly Button btnIngresar;
        private readonly Button btnSalir;
        private readonly Label lblEstado;
        private readonly Button btnProbar;
        private int _intentos;

        public FrmLogin()
        {
            Text = "Sistema de Préstamo de Materiales - Acceso";
            ClientSize = new Size(380, 360 + Autoria.AlturaLabel);
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

            btnProbar = Ui.Boton(this, "Probar conexión (MariaDB / MongoDB)", 25, 275, 330, BtnProbarConexion_Click);

            lblEstado = Ui.Etiqueta(this, string.Empty, 25, 320);
            lblEstado.ForeColor = Color.Firebrick;

            AcceptButton = btnIngresar;
            CancelButton = btnSalir;

            Autoria.Agregar(this);
        }

        private void BtnIngresar_Click(object? sender, EventArgs e)
        {
            if (ControladorAcceso.Validar(txtUsuario.Text, txtContrasena.Text, out string mensaje))
            {
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

            lblEstado.Text = mensaje + " (" + _intentos + "/" + IntentosMaximos + ")";
            MessageBox.Show(this, mensaje, "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtContrasena.Focus();
        }

        private void BtnSalir_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private async void BtnProbarConexion_Click(object? sender, EventArgs e)
        {
            btnProbar.Enabled = false;
            try
            {
                string msgMaria = string.Empty, msgMongo = string.Empty;
                bool okMaria = false, okMongo = false;

                await Task.Run(() =>
                {
                    okMaria = ConexionBD.ProbarConexion(out msgMaria);
                    okMongo = ConexionMongoDB.ProbarConexion(out msgMongo);
                });

                string resumen = "MariaDB: " + msgMaria + "\nMongoDB: " + msgMongo + "\n\nFecha y hora: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                MessageBox.Show(this, resumen, "Prueba de conexión", MessageBoxButtons.OK,
                                (okMaria && okMongo) ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            finally
            {
                btnProbar.Enabled = true;
            }
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
