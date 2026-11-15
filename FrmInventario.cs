// =====================================================================
//  Autoría - Equipo 0 (sustituya por los datos reales; ver Autoria.cs)
//    APELLIDO1 Apellido2 Nombre(s) - Integrante 1
//    APELLIDO1 Apellido2 Nombre(s) - Integrante 2
//    APELLIDO1 Apellido2 Nombre(s) - Integrante 3
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
    /// <summary>
    /// Alta, modificación y baja lógica de materiales, con filtros por categoría y estado y fotografía opcional.
    /// El guardado coordina tres piezas desacopladas: GestorImagenes (archivo físico), Material (guarda solo la
    /// ruta relativa) y MaterialDAO (base de datos).
    /// </summary>
    public class FrmInventario : Form
    {
        private const string TodasLasCategorias = "(Todas)";
        private const string TodosLosEstados = "(Todos)";

        private readonly MaterialDAO _dao = new MaterialDAO();

        private readonly TextBox txtBuscar;
        private readonly ComboBox cmbFiltroCategoria;
        private readonly ComboBox cmbFiltroEstado;
        private readonly DataGridView grid;

        private readonly TextBox txtId;
        private readonly TextBox txtSerie;
        private readonly TextBox txtNombre;
        private readonly ComboBox cmbCategoria;
        private readonly ComboBox cmbEstado;
        private readonly TextBox txtDescripcion;

        private readonly Button btnAlta;
        private readonly Button btnModificar;
        private readonly Button btnDesactivar;
        private readonly Button btnCargarImagen;

        private readonly Label lblImagen;
        private readonly PictureBox pbxFotografia;

        private string? _rutaOrigen;        // imagen elegida por el usuario, aún no copiada
        private string? _rutaImagenActual;  // ruta relativa guardada en el registro seleccionado

        public FrmInventario()
        {
            Sesion.ExigirSesion();          // consulta: cualquier perfil; las altas/bajas se restringen abajo

            Text = "Inventario de materiales";
            ClientSize = new Size(900, 510 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;

            // ---- Filtros
            Ui.Etiqueta(this, "Buscar:", 20, 18);
            txtBuscar = Ui.Caja(this, 75, 15, 200);

            Ui.Etiqueta(this, "Categoría:", 290, 18);
            List<string> categoriasFiltro = new List<string> { TodasLasCategorias };
            categoriasFiltro.AddRange(CategoriasMaterial.Todas);
            cmbFiltroCategoria = Ui.Combo(this, 355, 15, 150, categoriasFiltro);
            cmbFiltroCategoria.SelectedIndex = 0;

            Ui.Etiqueta(this, "Estado:", 520, 18);
            List<string> estadosFiltro = new List<string> { TodosLosEstados };
            estadosFiltro.AddRange(EstadosMaterial.Todos);
            cmbFiltroEstado = Ui.Combo(this, 570, 15, 130, estadosFiltro);
            cmbFiltroEstado.SelectedIndex = 0;

            Ui.Boton(this, "Buscar", 715, 13, 80, (s, e) => Cargar());

            // ---- Tabla
            grid = Ui.Tabla(this, 20, 50, 860, 245);
            grid.CellClick += Grid_CellClick;

            // ---- Formulario
            Ui.Etiqueta(this, "Id:", 20, 323);
            txtId = Ui.Caja(this, 110, 320, 80);
            txtId.ReadOnly = true;

            Ui.Etiqueta(this, "No. de serie:", 20, 353);
            txtSerie = Ui.Caja(this, 110, 350, 230);

            Ui.Etiqueta(this, "Nombre:", 20, 383);
            txtNombre = Ui.Caja(this, 110, 380, 230);

            Ui.Etiqueta(this, "Categoría:", 400, 323);
            cmbCategoria = Ui.Combo(this, 490, 320, 230, CategoriasMaterial.Todas, false);

            Ui.Etiqueta(this, "Estado:", 400, 353);
            cmbEstado = Ui.Combo(this, 490, 350, 230, EstadosMaterial.Todos);

            Ui.Etiqueta(this, "Descripción:", 400, 383);
            txtDescripcion = Ui.Caja(this, 490, 380, 210);
            txtDescripcion.Multiline = true;
            txtDescripcion.Height = 55;

            lblImagen = Ui.Etiqueta(this, "Fotografía: (sin imagen)", 720, 300);

            pbxFotografia = new PictureBox();
            pbxFotografia.Name = "pbxFotografia";
            pbxFotografia.Location = new Point(720, 320);
            pbxFotografia.Size = new Size(160, 108);
            pbxFotografia.SizeMode = PictureBoxSizeMode.StretchImage;
            pbxFotografia.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(pbxFotografia);

            btnCargarImagen = Ui.Boton(this, "Cargar Imagen", 720, 432, 160, BtnCargarImagen_Click);
            btnCargarImagen.Height = 26;

            Ui.Etiqueta(this, "El estado «Prestado» lo asigna el registro de un préstamo; «Baja» equivale a desactivar.", 20, 425);

            // ---- Botones
            Ui.Boton(this, "Limpiar", 20, 465, 110, (s, e) => LimpiarCampos());
            btnAlta = Ui.Boton(this, "Alta (nuevo)", 140, 465, 110, BtnAlta_Click);
            btnModificar = Ui.Boton(this, "Modificar", 260, 465, 110, BtnModificar_Click);
            btnDesactivar = Ui.Boton(this, "Desactivar", 380, 465, 110, BtnDesactivar_Click);
            Ui.Boton(this, "Cerrar", 770, 465, 110, (s, e) => Close());

            LimpiarCampos();
            AplicarPermisos();
            FormClosed += (s, e) => MostrarImagen(null);
            Autoria.Agregar(this);
            Load += (s, e) => Cargar();
        }

        /// <summary>
        /// Restricciones por perfil (SesionSistema.RolActivo): Administrador, todo; Operador, alta y modificación
        /// pero no la baja; Invitado, solo consulta.
        /// </summary>
        private void AplicarPermisos()
        {
            if (!Sesion.EsAdministrador)
                btnDesactivar.Enabled = false;

            if (!Sesion.EsPersonal)
            {
                btnAlta.Enabled = false;
                btnModificar.Enabled = false;
                btnCargarImagen.Enabled = false;
            }
        }

        private void Cargar()
        {
            try
            {
                string categoria = cmbFiltroCategoria.SelectedIndex <= 0 ? string.Empty : cmbFiltroCategoria.Text;
                string estado = cmbFiltroEstado.SelectedIndex <= 0 ? string.Empty : cmbFiltroEstado.Text;

                Ui.Enlazar(grid, _dao.Listar(txtBuscar.Text, categoria, estado));
                if (grid.Columns.Contains("RutaImagen")) grid.Columns["RutaImagen"].Visible = false;
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void LimpiarCampos()
        {
            txtId.Clear();
            txtSerie.Clear();
            txtNombre.Clear();
            cmbCategoria.Text = string.Empty;
            cmbEstado.SelectedItem = EstadosMaterial.Disponible;
            txtDescripcion.Clear();
            _rutaOrigen = null;
            _rutaImagenActual = null;
            MostrarImagen(null);
            lblImagen.Text = "Fotografía: (sin imagen)";
            grid?.ClearSelection();
        }

        private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = grid.Rows[e.RowIndex];
            txtId.Text = Ui.Texto(fila, "Id");
            txtSerie.Text = Ui.Texto(fila, "Serie");
            txtNombre.Text = Ui.Texto(fila, "Nombre");
            cmbCategoria.Text = Ui.Texto(fila, "Categoria");
            cmbEstado.SelectedItem = Ui.Texto(fila, "Estado");
            txtDescripcion.Text = Ui.Texto(fila, "Descripcion");

            _rutaOrigen = null;
            string ruta = Ui.Texto(fila, "RutaImagen");
            _rutaImagenActual = string.IsNullOrWhiteSpace(ruta) ? null : ruta;
            MostrarFotoGuardada();
        }

        // ------------------------------------------------------------------ Fotografía

        private void MostrarImagen(Image? nueva)
        {
            Image? anterior = pbxFotografia.Image;
            pbxFotografia.Image = nueva;
            if (anterior != null) anterior.Dispose();
        }

        private void MostrarFotoGuardada()
        {
            string? completa = GestorImagenes.ResolverRuta(_rutaImagenActual);

            if (_rutaImagenActual == null)
            {
                MostrarImagen(null);
                lblImagen.Text = "Fotografía: (sin imagen)";
                return;
            }

            if (completa == null || !File.Exists(completa))
            {
                MostrarImagen(null);
                lblImagen.Text = "Fotografía: (archivo no encontrado)";
                return;
            }

            if (GestorImagenes.TryCargarVistaPrevia(completa, out Image? imagen, out string error))
            {
                MostrarImagen(imagen);
                lblImagen.Text = "Fotografía guardada";
            }
            else
            {
                MostrarImagen(null);
                lblImagen.Text = "Fotografía: (no se pudo leer)";
            }
        }

        private void BtnCargarImagen_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Title = "Seleccione la fotografía del material";
                dialogo.Filter = GestorImagenes.FiltroDialogo;
                dialogo.CheckFileExists = true;
                dialogo.Multiselect = false;

                if (dialogo.ShowDialog(this) != DialogResult.OK)
                    return;

                if (!GestorImagenes.ValidarArchivo(dialogo.FileName, out string error) ||
                    !GestorImagenes.TryCargarVistaPrevia(dialogo.FileName, out Image? imagen, out error))
                {
                    AvisarErrorArchivo(error);
                    return;
                }

                MostrarImagen(imagen);
                _rutaOrigen = dialogo.FileName;
                lblImagen.Text = "Fotografía nueva (se copiará al guardar)";
            }
        }

        private void AvisarErrorArchivo(string mensaje)
        {
            MessageBox.Show(this, mensaje, "Sistema de Préstamos - Archivos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Si hay una imagen nueva elegida la copia al repositorio local (nombre único) y deja su ruta relativa en el
        /// modelo; si no, conserva la que ya tenía. Devuelve false (tras avisar al usuario) si falla la copia.
        /// </summary>
        private bool PrepararImagen(Material material, out string? rutaNueva)
        {
            rutaNueva = null;

            if (_rutaOrigen == null)
            {
                material.RutaImagen = _rutaImagenActual;
                return true;
            }

            if (!GestorImagenes.TryGuardar(_rutaOrigen, out rutaNueva, out string error))
            {
                AvisarErrorArchivo(error);
                return false;
            }

            material.RutaImagen = rutaNueva;
            return true;
        }

        private Material LeerFormulario()
        {
            Material material = new Material();
            if (int.TryParse(txtId.Text, out int id)) material.Id = id;
            material.NumeroSerie = txtSerie.Text;
            material.Nombre = txtNombre.Text;
            material.Categoria = cmbCategoria.Text;
            material.Estado = cmbEstado.SelectedItem?.ToString() ?? EstadosMaterial.Disponible;
            material.Descripcion = txtDescripcion.Text;
            return material;
        }

        private void BtnAlta_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtId.Text))
                {
                    Ui.Aviso(this, "Hay un material seleccionado. Pulse «Limpiar» para capturar uno nuevo, o «Modificar» para guardar cambios.");
                    return;
                }

                Material nuevo = LeerFormulario();
                if (!PrepararImagen(nuevo, out string? rutaNueva))
                    return;

                int id;
                try
                {
                    id = _dao.Insertar(nuevo);
                }
                catch
                {
                    GestorImagenes.EliminarSiExiste(rutaNueva);   // no dejar archivos huérfanos si falla la BD
                    throw;
                }

                Ui.Aviso(this, "Material registrado con Id " + id + ".");
                LimpiarCampos();
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void BtnModificar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    Ui.Aviso(this, "Seleccione un material de la tabla para modificarlo.");
                    return;
                }

                Material cambios = LeerFormulario();
                if (!PrepararImagen(cambios, out string? rutaNueva))
                    return;

                try
                {
                    _dao.Actualizar(cambios);
                }
                catch
                {
                    GestorImagenes.EliminarSiExiste(rutaNueva);
                    throw;
                }

                // La imagen anterior ya no se usa: se elimina solo después de guardar el registro.
                if (rutaNueva != null && _rutaImagenActual != null)
                    GestorImagenes.EliminarSiExiste(_rutaImagenActual);

                Ui.Aviso(this, "Material actualizado.");
                LimpiarCampos();
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void BtnDesactivar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtId.Text, out int id))
                {
                    Ui.Aviso(this, "Seleccione un material de la tabla para desactivarlo.");
                    return;
                }

                if (!Ui.Confirmar(this, "¿Dar de baja el material «" + txtNombre.Text + "»? Se conservará su historial de préstamos."))
                    return;

                _dao.Desactivar(id);
                Ui.Aviso(this, "Material desactivado (estado Baja).");
                LimpiarCampos();
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }
    }
}
