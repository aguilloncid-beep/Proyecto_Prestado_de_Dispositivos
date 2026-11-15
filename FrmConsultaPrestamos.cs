// =====================================================================
//  Autoría - Equipo 5
//  Integrantes en orden alfabético por primer apellido
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
//
//  Actividad 4.3 - Pantalla de consulta: listado con datos descriptivos (JOIN) y
//  fotografía del material del préstamo seleccionado.
// =====================================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SistemaPresatamos.Archivos;
using SistemaPresatamos.Data;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    public class FrmConsultaPrestamos : Form
    {
        private readonly ConsultaPrestamoDAO _dao = new ConsultaPrestamoDAO();

        private readonly TextBox txtBuscar;
        private readonly DataGridView grid;
        private readonly Label lblTotal;
        private readonly Label lblImagen;
        private readonly PictureBox pbxFotografia;

        public FrmConsultaPrestamos()
        {
            Sesion.ExigirSesion();

            Text = "Consulta de préstamos (detalle con fotografía)";
            ClientSize = new Size(1180, 580 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;

            Ui.Etiqueta(this, "Alumno, material, serie o categoría:", 20, 18);
            txtBuscar = Ui.Caja(this, 230, 15, 260);
            Ui.Boton(this, "Buscar", 505, 13, 110, (s, e) => Cargar());

            grid = Ui.Tabla(this, 20, 55, 900, 465);
            ConfigurarColumnas();
            grid.SelectionChanged += (s, e) => MostrarFotoSeleccionada();

            lblImagen = Ui.Etiqueta(this, "Fotografía del material", 940, 55);
            lblImagen.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            pbxFotografia = new PictureBox();
            pbxFotografia.Name = "pbxFotografia";
            pbxFotografia.Location = new Point(940, 80);
            pbxFotografia.Size = new Size(220, 220);
            pbxFotografia.BorderStyle = BorderStyle.FixedSingle;
            pbxFotografia.SizeMode = PictureBoxSizeMode.Zoom;
            pbxFotografia.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Controls.Add(pbxFotografia);

            lblTotal = Ui.Etiqueta(this, string.Empty, 20, 540);
            lblTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Ui.Boton(this, "Cerrar", 1050, 536, 110, (s, e) => Close()).Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Autoria.Agregar(this);
            Load += (s, e) => Cargar();
            FormClosed += (s, e) => MostrarImagen(null);
        }

        // =================================================================
        //  Autoría - Equipo 5
        //    FERNANDEZ Hernandez Kevin Isaid | FLORES Becerra Cid Hector | GOVEA Mijares Jorge Luis
        //
        //  Columnas explícitas con encabezados claros. Solo se agregan columnas descriptivas:
        //  IdPrestamo y RutaImagen existen en el objeto pero NO tienen columna, así que el usuario
        //  nunca ve identificadores técnicos de la base de datos.
        // =================================================================
        private void ConfigurarColumnas()
        {
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();

            AgregarColumna("Alumno", "Alumno", 140);
            AgregarColumna("NumeroEstudiante", "No. de estudiante", 95);
            AgregarColumna("Carrera", "Carrera", 120);
            AgregarColumna("Material", "Material", 130);
            AgregarColumna("Categoria", "Categoría", 85);
            AgregarColumna("NumeroSerie", "No. de serie", 80);
            AgregarColumna("FechaPrestamo", "Fecha de préstamo", 85, "dd/MM/yyyy");
            AgregarColumna("FechaLimite", "Fecha límite", 80, "dd/MM/yyyy");
            AgregarColumna("FechaDevolucion", "Devolución", 80, "dd/MM/yyyy");
            AgregarColumna("Estado", "Estado", 70);
            AgregarColumna("Observaciones", "Observaciones", 120);
        }

        private void AgregarColumna(string propiedad, string encabezado, int pesoAncho, string? formato = null)
        {
            DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn();
            columna.DataPropertyName = propiedad;     // propiedad de PrestamoVista
            columna.Name = propiedad;
            columna.HeaderText = encabezado;          // texto que ve el usuario
            columna.FillWeight = pesoAncho;
            if (formato != null) columna.DefaultCellStyle.Format = formato;
            grid.Columns.Add(columna);
        }

        // =================================================================
        //  Autoría - Equipo 5
        //    FERNANDEZ Hernandez Kevin Isaid | FLORES Becerra Cid Hector | GOVEA Mijares Jorge Luis
        //
        //  Vuelca la colección devuelta por el controlador (List<PrestamoVista>) en el DataGridView.
        // =================================================================
        private void Cargar()
        {
            try
            {
                List<PrestamoVista> lista = _dao.ListarDetalle(txtBuscar.Text);
                grid.DataSource = null;
                grid.DataSource = lista;
                lblTotal.Text = lista.Count + " préstamo(s) encontrados.";

                if (grid.Rows.Count > 0)
                    grid.Rows[0].Selected = true;
                else
                    MostrarFotoSeleccionada();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        // ------------------------------------------------------------------ Fotografía

        private void MostrarFotoSeleccionada()
        {
            PrestamoVista? prestamo = grid.CurrentRow?.DataBoundItem as PrestamoVista;
            if (grid.SelectedRows.Count > 0)
                prestamo = grid.SelectedRows[0].DataBoundItem as PrestamoVista;

            if (prestamo == null)
            {
                MostrarImagen(null);
                lblImagen.Text = "Fotografía del material";
                return;
            }

            // Ruta relativa guardada en la BD -> ruta completa dentro de ImagenesDatos (A3.6).
            string? completa = GestorImagenes.ResolverRuta(prestamo.RutaImagen);

            if (completa == null)
            {
                MostrarImagen(null);
                lblImagen.Text = prestamo.Material + ": sin fotografía";
                return;
            }

            // Validación de existencia física antes de intentar leer.
            if (!File.Exists(completa))
            {
                MostrarImagen(null);
                lblImagen.Text = prestamo.Material + ": archivo no encontrado";
                return;
            }

            Image? imagen = CargarSinBloquear(completa, out string error);
            MostrarImagen(imagen);
            lblImagen.Text = imagen != null ? prestamo.Material : prestamo.Material + ": " + error;
        }

        /// <summary>
        /// Lee todos los bytes del archivo (File.ReadAllBytes abre y cierra el archivo de inmediato) y crea la
        /// imagen con Image.FromStream sobre un MemoryStream. Se devuelve una copia (Bitmap), de modo que el
        /// archivo físico nunca queda bloqueado por el sistema operativo.
        /// </summary>
        private static Image? CargarSinBloquear(string rutaCompleta, out string error)
        {
            error = string.Empty;

            try
            {
                byte[] bytes = File.ReadAllBytes(rutaCompleta);
                using (MemoryStream flujo = new MemoryStream(bytes))
                using (Image original = Image.FromStream(flujo))
                {
                    return new Bitmap(original);
                }
            }
            catch (ArgumentException)
            {
                // Image.FromStream lanza ArgumentException si los bytes no son una imagen válida.
                error = "imagen dañada";
            }
            catch (UnauthorizedAccessException)
            {
                error = "sin permisos de lectura";
            }
            catch (IOException)
            {
                error = "no se pudo leer el archivo";
            }
            return null;
        }

        private void MostrarImagen(Image? nueva)
        {
            Image? anterior = pbxFotografia.Image;
            pbxFotografia.Image = nueva;
            anterior?.Dispose();
        }
    }
}
