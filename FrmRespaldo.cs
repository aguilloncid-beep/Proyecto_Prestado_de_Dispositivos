// =====================================================================
//  Autoría - Equipo 5
//  Integrantes en orden alfabético por primer apellido
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
//
//  Actividad 4.2 - Vista de respaldo: exportar e importar colecciones en JSON y XML.
// =====================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SistemaPresatamos.Models;
using SistemaPresatamos.Procesos;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    /// <summary>
    /// Módulo de importación y exportación. La vista solo elige archivos y muestra resultados;
    /// la serialización y el manejo del disco los hace BackupController (MVC).
    /// </summary>
    public class FrmRespaldo : Form
    {
        private const string TituloAlertas = "Sistema de Préstamos - Respaldos";
        private const string Materiales = "Materiales";
        private const string Prestamos = "Préstamos";

        private readonly ComboBox cmbDatos;
        private readonly DataGridView grid;
        private readonly Label lblEstado;

        public FrmRespaldo()
        {
            Sesion.ExigirPersonal();

            Text = "Respaldo de datos (JSON / XML)";
            ClientSize = new Size(1000, 580 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;

            Ui.Etiqueta(this, "Datos:", 20, 18);
            cmbDatos = Ui.Combo(this, 70, 15, 150, new[] { Materiales, Prestamos });
            cmbDatos.SelectedIndex = 0;
            cmbDatos.SelectedIndexChanged += (s, e) => MostrarDesdeBaseDeDatos();

            Ui.Boton(this, "Exportar JSON", 240, 13, 120, (s, e) => Exportar(json: true));
            Ui.Boton(this, "Exportar XML", 370, 13, 120, (s, e) => Exportar(json: false));
            Ui.Boton(this, "Importar archivo...", 500, 13, 150, BtnImportar_Click);
            Ui.Boton(this, "Abrir carpeta Backups", 660, 13, 170, BtnAbrirCarpeta_Click);

            grid = Ui.Tabla(this, 20, 55, 960, 465);

            lblEstado = Ui.Etiqueta(this, string.Empty, 20, 540);
            lblEstado.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Ui.Boton(this, "Ver datos actuales", 600, 536, 150, (s, e) => MostrarDesdeBaseDeDatos()).Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Ui.Boton(this, "Cerrar", 870, 536, 110, (s, e) => Close()).Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Autoria.Agregar(this);
            Load += (s, e) => MostrarDesdeBaseDeDatos();
        }

        private bool EsMateriales { get { return cmbDatos.Text == Materiales; } }

        // ------------------------------------------------------------------ Mostrar

        private void MostrarDesdeBaseDeDatos()
        {
            try
            {
                if (EsMateriales) Mostrar(BackupController.ObtenerMateriales(), "la base de datos");
                else Mostrar(BackupController.ObtenerPrestamos(), "la base de datos");
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        /// <summary>Vincula la colección directamente al DataGridView para actualizar la interfaz de inmediato.</summary>
        private void Mostrar<T>(List<T> lista, string origen)
        {
            grid.DataSource = null;
            grid.DataSource = lista;
            foreach (DataGridViewColumn columna in grid.Columns)
            {
                if (columna.ValueType == typeof(DateTime) || columna.ValueType == typeof(DateTime?))
                    columna.DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            grid.ClearSelection();
            lblEstado.Text = cmbDatos.Text + ": " + lista.Count + " registro(s) cargados desde " + origen + ".";
        }

        // ------------------------------------------------------------------ Exportar

        private void Exportar(bool json)
        {
            try
            {
                bool exito;
                string? ruta;
                string error;

                if (EsMateriales)
                {
                    List<MaterialRespaldo> lista = BackupController.ObtenerMateriales();
                    exito = json ? BackupController.TryExportarJson(lista, "Materiales", out ruta, out error)
                                 : BackupController.TryExportarXml(lista, "Materiales", out ruta, out error);
                    Mostrar(lista, "la base de datos");
                }
                else
                {
                    List<PrestamoRespaldo> lista = BackupController.ObtenerPrestamos();
                    exito = json ? BackupController.TryExportarJson(lista, "Prestamos", out ruta, out error)
                                 : BackupController.TryExportarXml(lista, "Prestamos", out ruta, out error);
                    Mostrar(lista, "la base de datos");
                }

                if (exito)
                    MessageBox.Show(this, "Respaldo " + (json ? "JSON" : "XML") + " generado correctamente:\n\n" + ruta,
                                    TituloAlertas, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(this, error, TituloAlertas, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);   // errores de base de datos al obtener la colección
            }
        }

        // ------------------------------------------------------------------ Importar

        private void BtnImportar_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Title = "Importar respaldo de " + cmbDatos.Text.ToLower();
                dialogo.Filter = "Respaldos (*.json, *.xml)|*.json;*.xml|JSON (*.json)|*.json|XML (*.xml)|*.xml";
                if (BackupController.AsegurarCarpeta(out _))
                    dialogo.InitialDirectory = BackupController.CarpetaRespaldos;

                if (dialogo.ShowDialog(this) != DialogResult.OK) return;

                string ruta = dialogo.FileName;
                bool esJson = Path.GetExtension(ruta).Equals(".json", StringComparison.OrdinalIgnoreCase);
                string origen = "el archivo " + Path.GetFileName(ruta);
                bool exito;
                string error;

                // La tabla actual no se toca hasta que la lista importada se reconstruyó y validó por completo.
                if (EsMateriales)
                {
                    List<MaterialRespaldo>? lista;
                    exito = esJson ? BackupController.TryImportarJson(ruta, out lista, out error)
                                   : BackupController.TryImportarXml(ruta, out lista, out error);
                    if (exito && lista != null) Mostrar(lista, origen);
                }
                else
                {
                    List<PrestamoRespaldo>? lista;
                    exito = esJson ? BackupController.TryImportarJson(ruta, out lista, out error)
                                   : BackupController.TryImportarXml(ruta, out lista, out error);
                    if (exito && lista != null) Mostrar(lista, origen);
                }

                if (exito)
                    MessageBox.Show(this, "Importación exitosa. Los datos del archivo se muestran en la tabla.\n" +
                                          "(Solo se cargan en memoria; la base de datos no se modifica.)",
                                    TituloAlertas, MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(this, error, "Archivo no válido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAbrirCarpeta_Click(object? sender, EventArgs e)
        {
            if (!BackupController.AsegurarCarpeta(out string error))
            {
                MessageBox.Show(this, error, TituloAlertas, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo { FileName = BackupController.CarpetaRespaldos, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }
    }
}
