using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaPresatamos
{
   
    /// Encabezado de autoría: un Label permanente en cada formulario con el número de equipo y los
    /// integrantes (primer apellido en MAYÚSCULAS). 
   
    internal static class Autoria
    {
        public const int AlturaLabel = 34;

        public const string NumeroEquipo = "5";

        public static readonly string[] Integrantes =
        {
            "FERNANDEZ HERNANDEZ KEVIN ISAID - Integrante 1",
            "Flores Becerra Cid Hector - Integrante 2",
            "Govea Mijares Jorge Luis - Integrante 3"
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
