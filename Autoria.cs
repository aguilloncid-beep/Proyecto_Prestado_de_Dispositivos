using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaPresatamos
{

    internal static class Autoria
    {
        public const int AlturaLabel = 34;

        public const string NumeroEquipo = "4";

        public static readonly string[] Integrantes =
        {
            "FERNANDEZ Hernandez Kevin Isaid - Integrante 1",
            "FLORES Becerra Cid Hector - Integrante 2",
            "GOVEA Mijares Jorge Luis - Integrante 3"
        };

        public static string Texto
        {
            get { return "Equipo " + NumeroEquipo + "  |  " + string.Join("  |  ", Integrantes); }
        }

        public static Label Agregar(Form formulario)
        {
            Label etiqueta = new Label();
            etiqueta.Name = "lblAutoria";
            etiqueta.Text = Texto;
            etiqueta.Dock = DockStyle.Bottom;
            etiqueta.AutoSize = false;
            etiqueta.Height = AlturaLabel;
            etiqueta.TextAlign = ContentAlignment.MiddleLeft;
            etiqueta.Padding = new Padding(8, 0, 8, 0);
            etiqueta.Font = new Font(formulario.Font.FontFamily, 8f, FontStyle.Regular);
            etiqueta.ForeColor = SystemColors.GrayText;
            etiqueta.BorderStyle = BorderStyle.FixedSingle;
            formulario.Controls.Add(etiqueta);
            return etiqueta;
        }
    }
}
