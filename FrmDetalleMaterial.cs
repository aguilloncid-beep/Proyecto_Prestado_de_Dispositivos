// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SistemaPresatamos.Archivos;
using SistemaPresatamos.Models;

namespace SistemaPresatamos
{
    // A4.6 - Formulario modal de solo lectura: paso previo obligatorio antes de dar de baja o eliminar un material.
    public class FrmDetalleMaterial : Form
    {
        private readonly PictureBox pbxFotografia;

        public FrmDetalleMaterial(Material material, bool eliminacionDefinitiva)
        {
            if (material == null) throw new ArgumentNullException(nameof(material));

            Text = eliminacionDefinitiva ? "Detalle del material - eliminación definitiva" : "Detalle del material - baja lógica";
            ClientSize = new Size(640, 360 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Label aviso = Ui.Etiqueta(this, eliminacionDefinitiva
                ? "Revise los datos. Al confirmar, el registro se ELIMINARÁ para siempre."
                : "Revise los datos. Al confirmar, el material quedará en Baja (se conserva su historial).", 20, 15);
            aviso.ForeColor = Color.Firebrick;

            SoloLectura("Id:", material.Id.ToString(), 20, 50, 90);
            SoloLectura("No. de serie:", material.NumeroSerie ?? string.Empty, 20, 80, 230);
            SoloLectura("Nombre:", material.Nombre, 20, 110, 230);
            SoloLectura("Categoría:", material.Categoria, 20, 140, 230);
            SoloLectura("Estado:", material.Estado, 20, 170, 230);
            SoloLectura("Precio:", material.Precio.ToString("C2"), 20, 200, 110);
            SoloLectura("Stock:", material.Stock.ToString(), 20, 230, 110);

            Ui.Etiqueta(this, "Descripción:", 20, 263);
            TextBox txtDescripcion = Ui.Caja(this, 110, 260, 230);
            txtDescripcion.Multiline = true;
            txtDescripcion.Height = 60;
            txtDescripcion.Text = material.Descripcion ?? string.Empty;
            txtDescripcion.ReadOnly = true;

            Ui.Etiqueta(this, "Fotografía:", 400, 50);
            pbxFotografia = new PictureBox();
            pbxFotografia.Location = new Point(400, 75);
            pbxFotografia.Size = new Size(220, 150);
            pbxFotografia.SizeMode = PictureBoxSizeMode.StretchImage;
            pbxFotografia.BorderStyle = BorderStyle.FixedSingle;
            pbxFotografia.Enabled = false;
            Controls.Add(pbxFotografia);
            CargarFotografia(material.RutaImagen);

            Button btnConfirmar = Ui.Boton(this, eliminacionDefinitiva ? "Eliminar definitivamente" : "Confirmar baja lógica", 300, 320, 190, (s, e) => { });
            btnConfirmar.DialogResult = DialogResult.OK;
            Button btnCancelar = Ui.Boton(this, "Cancelar", 500, 320, 120, (s, e) => { });
            btnCancelar.DialogResult = DialogResult.Cancel;

            AcceptButton = btnCancelar;
            CancelButton = btnCancelar;
            FormClosed += (s, e) => LiberarImagen();
            Autoria.Agregar(this);
        }

        private void SoloLectura(string etiqueta, string valor, int x, int y, int ancho)
        {
            Ui.Etiqueta(this, etiqueta, x, y + 3);
            TextBox caja = Ui.Caja(this, x + 90, y, ancho);
            caja.Text = valor;
            caja.ReadOnly = true;
        }

        private void CargarFotografia(string? rutaRelativa)
        {
            string? completa = GestorImagenes.ResolverRuta(rutaRelativa);
            if (completa == null || !File.Exists(completa)) return;

            if (GestorImagenes.TryCargarVistaPrevia(completa, out Image? imagen, out string _))
                pbxFotografia.Image = imagen;
        }

        // Libera el archivo para que File.Delete pueda borrarlo después de confirmar.
        private void LiberarImagen()
        {
            Image? imagen = pbxFotografia.Image;
            pbxFotografia.Image = null;
            if (imagen != null) imagen.Dispose();
        }
    }
}
