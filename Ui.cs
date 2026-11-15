using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SistemaPresatamos
{
    /// <summary>Funciones pequeñas para crear controles y mostrar mensajes de forma uniforme.</summary>
    internal static class Ui
    {
        public static readonly string[] Carreras =
        {
            "Ing. Ciencias Computacionales",
            "Ing. Energias renovables",
            "Ing. Nanotecnologia"
        };

        public static Label Etiqueta(Control padre, string texto, int x, int y)
        {
            Label etiqueta = new Label();
            etiqueta.Text = texto;
            etiqueta.AutoSize = true;
            etiqueta.Location = new Point(x, y);
            padre.Controls.Add(etiqueta);
            return etiqueta;
        }

        public static TextBox Caja(Control padre, int x, int y, int ancho, bool password = false)
        {
            TextBox caja = new TextBox();
            caja.Location = new Point(x, y);
            caja.Width = ancho;
            caja.UseSystemPasswordChar = password;
            padre.Controls.Add(caja);
            return caja;
        }

        public static ComboBox Combo(Control padre, int x, int y, int ancho, IEnumerable<string> elementos, bool soloLista = true)
        {
            ComboBox combo = new ComboBox();
            combo.Location = new Point(x, y);
            combo.Width = ancho;
            combo.DropDownStyle = soloLista ? ComboBoxStyle.DropDownList : ComboBoxStyle.DropDown;
            foreach (string elemento in elementos)
                combo.Items.Add(elemento);
            padre.Controls.Add(combo);
            return combo;
        }

        public static Button Boton(Control padre, string texto, int x, int y, int ancho, EventHandler alHacerClic)
        {
            Button boton = new Button();
            boton.Text = texto;
            boton.Location = new Point(x, y);
            boton.Size = new Size(ancho, 28);
            boton.UseVisualStyleBackColor = true;
            boton.Click += alHacerClic;
            padre.Controls.Add(boton);
            return boton;
        }

        public static DataGridView Tabla(Control padre, int x, int y, int ancho, int alto)
        {
            DataGridView tabla = new DataGridView();
            tabla.Location = new Point(x, y);
            tabla.Size = new Size(ancho, alto);
            tabla.ReadOnly = true;
            tabla.AllowUserToAddRows = false;
            tabla.AllowUserToDeleteRows = false;
            tabla.AllowUserToResizeRows = false;
            tabla.MultiSelect = false;
            tabla.RowHeadersVisible = false;
            tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tabla.BackgroundColor = SystemColors.Window;
            tabla.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            padre.Controls.Add(tabla);
            return tabla;
        }

        /// <summary>Asigna la tabla de datos a la rejilla y da formato de fecha a las columnas de fecha.</summary>
        public static void Enlazar(DataGridView rejilla, DataTable datos)
        {
            rejilla.DataSource = datos;
            foreach (DataGridViewColumn columna in rejilla.Columns)
            {
                if (columna.ValueType == typeof(DateTime))
                    columna.DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            rejilla.ClearSelection();
        }

        public static string Texto(DataGridViewRow fila, string columna)
        {
            object? valor = fila.Cells[columna].Value;
            return (valor == null || valor == DBNull.Value) ? string.Empty : Convert.ToString(valor) ?? string.Empty;
        }

        public static string Descripcion(Exception ex)
        {
            if (ex is ArgumentException || ex is InvalidOperationException || ex is UnauthorizedAccessException)
                return ex.Message;

            if (ex is MySqlException)
                return "Error de base de datos. Verifique que el servidor esté encendido y que la conexión " +
                       "(ConexionBD.cs o variable SISTEMA_PRESTAMOS_CONN) sea correcta.\n\nDetalle: " + ex.Message;

            return "Ocurrió un error inesperado: " + ex.Message;
        }

        public static void Error(IWin32Window? dueno, Exception ex)
        {
            MessageBox.Show(dueno, Descripcion(ex), "Sistema de Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void Aviso(IWin32Window? dueno, string mensaje)
        {
            MessageBox.Show(dueno, mensaje, "Sistema de Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool Confirmar(IWin32Window? dueno, string mensaje)
        {
            return MessageBox.Show(dueno, mensaje, "Sistema de Préstamos",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}
