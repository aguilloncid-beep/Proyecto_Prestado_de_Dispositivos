using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    /// <summary>
    /// Ventana hija que demuestra la comunicación entre formularios con datos estáticos: lee directamente
    /// SesionSistema; nadie le pasó el usuario ni el rol por el constructor ni consultó una base de datos.
    /// </summary>
    public class FrmInfoSesion : Form
    {
        private readonly Label lblTranscurrido;
        private readonly System.Windows.Forms.Timer _reloj = new System.Windows.Forms.Timer();

        public FrmInfoSesion()
        {
            Sesion.ExigirSesion();

            Text = "Información de sesión";
            ClientSize = new Size(440, 230 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;

            Label titulo = Ui.Etiqueta(this, "Datos leídos de SesionSistema", 20, 15);
            titulo.Font = new Font(Font.FontFamily, 11f, FontStyle.Bold);

            Ui.Etiqueta(this, "Usuario activo:", 20, 60);
            Ui.Etiqueta(this, SesionSistema.UsuarioActivo, 150, 60);

            Ui.Etiqueta(this, "Rol activo:", 20, 90);
            Ui.Etiqueta(this, SesionSistema.RolActivo, 150, 90);

            Ui.Etiqueta(this, "Inicio de sesión:", 20, 120);
            Ui.Etiqueta(this, SesionSistema.HoraInicioSesion.ToString("dd/MM/yyyy HH:mm:ss"), 150, 120);

            Ui.Etiqueta(this, "Tiempo de sesión:", 20, 150);
            lblTranscurrido = Ui.Etiqueta(this, string.Empty, 150, 150);

            Label nota = Ui.Etiqueta(this, "Estos valores se comparten con todos los formularios mediante propiedades estáticas.", 20, 190);
            nota.ForeColor = SystemColors.GrayText;

            _reloj.Interval = 1000;
            _reloj.Tick += (s, e) => ActualizarTiempo();
            _reloj.Start();
            ActualizarTiempo();
            FormClosed += (s, e) => _reloj.Dispose();

            Autoria.Agregar(this);
        }

        private void ActualizarTiempo()
        {
            TimeSpan t = DateTime.Now - SesionSistema.HoraInicioSesion;
            lblTranscurrido.Text = t.ToString(@"hh\:mm\:ss");
        }
    }
}
