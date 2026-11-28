// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SistemaPresatamos.Archivos;
using SistemaPresatamos.Data;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    public class FrmInventario : Form
    {
        private const string TodasLasCategorias = "(Todas)";
        private const string TodosLosEstados = "(Todos)";

        private readonly MaterialDAO _dao = new MaterialDAO();
        private readonly CategoriaDAO _categorias = new CategoriaDAO();
        private readonly MaterialMongoDAO _mongo = new MaterialMongoDAO();

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
        private readonly TextBox txtPrecio;
        private readonly TextBox txtStock;

        private readonly Button btnAlta;
        private readonly Button btnModificar;
        private readonly Button btnDesactivar;
        private readonly Button btnEliminar;
        private readonly Button btnCargarImagen;

        private readonly Label lblImagen;
        private readonly PictureBox pbxFotografia;

        private string? _rutaOrigen;
        private string? _rutaImagenActual;

        public FrmInventario()
        {
            Sesion.ExigirSesion();

            Text = "Inventario de materiales";
            ClientSize = new Size(900, 545 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;

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

            grid = Ui.Tabla(this, 20, 50, 860, 245);
            grid.CellClick += Grid_CellClick;
            grid.CellDoubleClick += Grid_CellClick;

            Ui.Etiqueta(this, "Id:", 20, 323);
            txtId = Ui.Caja(this, 110, 320, 80);
            txtId.ReadOnly = true;

            Ui.Etiqueta(this, "No. de serie:", 20, 353);
            txtSerie = Ui.Caja(this, 110, 350, 230);

            Ui.Etiqueta(this, "Nombre:", 20, 383);
            txtNombre = Ui.Caja(this, 110, 380, 230);

            Ui.Etiqueta(this, "Precio:", 20, 413);
            txtPrecio = Ui.Caja(this, 110, 410, 90);

            Ui.Etiqueta(this, "Stock:", 215, 413);
            txtStock = Ui.Caja(this, 260, 410, 80);

            Ui.Etiqueta(this, "Categoría:", 400, 323);
            cmbCategoria = Ui.Combo(this, 490, 320, 230, Array.Empty<string>());

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

            Ui.Etiqueta(this, "El estado «Prestado» lo asigna el registro de un préstamo; «Baja» equivale a desactivar.", 20, 470);

            Ui.Boton(this, "Limpiar", 20, 500, 110, (s, e) => LimpiarCampos());
            btnAlta = Ui.Boton(this, "Alta (nuevo)", 140, 500, 110, BtnAlta_Click);
            btnModificar = Ui.Boton(this, "Modificar", 260, 500, 110, BtnModificar_Click);
            btnDesactivar = Ui.Boton(this, "Desactivar", 380, 500, 110, BtnDesactivar_Click);
            btnEliminar = Ui.Boton(this, "Eliminar...", 500, 500, 110, BtnEliminar_Click);
            Ui.Boton(this, "Cerrar", 770, 500, 110, (s, e) => Close());

            LimpiarCampos();
            AplicarPermisos();
            FormClosed += (s, e) => MostrarImagen(null);
            Autoria.Agregar(this);
            Load += (s, e) => { CargarCategorias(); Cargar(); };
        }

        private void AplicarPermisos()
        {
            // A20 - Administrador: acceso total. Operador: sin eliminar ni modificar precios. Invitado: solo lectura.
            if (!Sesion.EsAdministrador)
            {
                btnDesactivar.Enabled = false;
                btnEliminar.Enabled = false;
                txtPrecio.ReadOnly = true;
            }

            if (!Sesion.EsPersonal)
            {
                btnAlta.Enabled = false;
                btnModificar.Enabled = false;
                btnCargarImagen.Enabled = false;

                txtSerie.ReadOnly = true;
                txtNombre.ReadOnly = true;
                txtDescripcion.ReadOnly = true;
                txtStock.ReadOnly = true;
                cmbCategoria.Enabled = false;
                cmbEstado.Enabled = false;
            }
        }

        private void CargarCategorias()
        {
            try
            {
                DataTable catalogo = _categorias.ListarActivas();

                cmbCategoria.DataSource = null;
                // DisplayMember: texto visible (Nombre). ValueMember: llave que se guarda (IdCategoria).
                cmbCategoria.DisplayMember = "Nombre";
                cmbCategoria.ValueMember = "IdCategoria";
                cmbCategoria.DataSource = catalogo;
                cmbCategoria.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
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
                if (grid.Columns.Contains("IdCategoria")) grid.Columns["IdCategoria"].Visible = false;
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
            cmbCategoria.SelectedIndex = -1;
            cmbEstado.SelectedItem = EstadosMaterial.Disponible;
            txtDescripcion.Clear();
            txtPrecio.Text = "0";
            txtStock.Text = "1";
            _rutaOrigen = null;
            _rutaImagenActual = null;
            MostrarImagen(null);
            lblImagen.Text = "Fotografía: (sin imagen)";
            grid?.ClearSelection();
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.5 - Precarga para edición: con un clic o doble clic se toma el Id de la fila y se consulta el registro
        //  completo por llave primaria. SelectedValue recibe el IdCategoria para que el ComboBox muestre la categoría.
        // =====================================================================
        private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (!int.TryParse(Ui.Texto(grid.Rows[e.RowIndex], "Id"), out int id)) return;

            try
            {
                CargarFormularioPorId(id);
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void CargarFormularioPorId(int id)
        {
            Material? material = _dao.ObtenerPorId(id);
            if (material == null)
            {
                Ui.Aviso(this, "El material seleccionado ya no existe. Se actualizará la lista.");
                LimpiarCampos();
                Cargar();
                return;
            }

            txtId.Text = material.Id.ToString();
            txtSerie.Text = material.NumeroSerie ?? string.Empty;
            txtNombre.Text = material.Nombre;
            txtDescripcion.Text = material.Descripcion ?? string.Empty;
            txtPrecio.Text = material.Precio.ToString("0.00");
            txtStock.Text = material.Stock.ToString();
            cmbEstado.SelectedItem = material.Estado;

            if (material.IdCategoria != null)
                cmbCategoria.SelectedValue = material.IdCategoria.Value;
            else
                cmbCategoria.SelectedIndex = cmbCategoria.FindStringExact(material.Categoria);

            _rutaOrigen = null;
            _rutaImagenActual = string.IsNullOrWhiteSpace(material.RutaImagen) ? null : material.RutaImagen;
            MostrarFotoGuardada();
        }

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
            if (int.TryParse(Convert.ToString(cmbCategoria.SelectedValue), out int idCategoria))
                material.IdCategoria = idCategoria;
            material.Estado = cmbEstado.SelectedItem?.ToString() ?? EstadosMaterial.Disponible;
            material.Descripcion = txtDescripcion.Text;
            if (decimal.TryParse(txtPrecio.Text, out decimal precio)) material.Precio = precio;
            if (int.TryParse(txtStock.Text, out int stock)) material.Stock = stock;
            return material;
        }

        private bool ValidarCaptura()
        {
            if (txtNombre.Text.Trim().Length == 0)
                return Rechazar("El nombre del material es obligatorio.", txtNombre);
            if (txtNombre.Text.Trim().Length > 120)
                return Rechazar("El nombre no puede exceder 120 caracteres.", txtNombre);
            if (txtSerie.Text.Trim().Length > 80)
                return Rechazar("El número de serie no puede exceder 80 caracteres.", txtSerie);
            if (txtDescripcion.Text.Trim().Length > 500)
                return Rechazar("La descripción no puede exceder 500 caracteres.", txtDescripcion);

            // Presencia de campos obligatorios y longitudes del modelo
            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio < 0)
                return Rechazar("El precio debe ser un número mayor o igual a cero.", txtPrecio);
            if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
                return Rechazar("El stock debe ser un número entero mayor o igual a cero.", txtStock);

            // SelectedValue se convierte con int.TryParse: evita excepciones de tipo y llaves foráneas inválidas
            if (cmbCategoria.SelectedValue == null ||
                !int.TryParse(Convert.ToString(cmbCategoria.SelectedValue), out int idCategoria) || idCategoria <= 0)
                return Rechazar("Seleccione una categoría de la lista.", cmbCategoria);

            if (cmbEstado.SelectedItem == null)
                return Rechazar("Seleccione el estado del material.", cmbEstado);

            if (!string.IsNullOrWhiteSpace(txtId.Text) && !int.TryParse(txtId.Text, out _))
                return Rechazar("El Id del material no es un número válido.", txtId);

            return true;
        }

        private bool Rechazar(string mensaje, Control campo)
        {
            Ui.Aviso(this, mensaje);
            campo.Focus();
            return false;
        }

        private async System.Threading.Tasks.Task<string> GuardarEnMongoAsync(Material material, int idRelacional)
        {
            try
            {
                MongoDB.Bson.ObjectId idDocumento = await _mongo.InsertarAsync(material, idRelacional);
                return "\nTambién se guardó en MongoDB (ObjectId " + idDocumento + ").";
            }
            catch (Exception ex)
            {
                return "\n\nAtención: el registro quedó en MariaDB, pero NO se pudo guardar en MongoDB.\n" + ex.Message;
            }
        }

        private async System.Threading.Tasks.Task<string> ActualizarEnMongoAsync(Material material)
        {
            try
            {
                MongoDB.Bson.ObjectId idDocumento = await _mongo.ActualizarAsync(material);
                return "\nTambién se actualizó en MongoDB (ObjectId " + idDocumento + ").";
            }
            catch (Exception ex)
            {
                return "\n\nAtención: el registro se actualizó en MariaDB, pero NO se pudo actualizar en MongoDB.\n" + ex.Message;
            }
        }

        private async void BtnAlta_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtId.Text))
                {
                    Ui.Aviso(this, "Hay un material seleccionado. Pulse «Limpiar» para capturar uno nuevo, o «Modificar» para guardar cambios.");
                    return;
                }

                if (!ValidarCaptura())
                    return;

                btnAlta.Enabled = false;

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
                    GestorImagenes.EliminarSiExiste(rutaNueva);
                    throw;
                }

                string avisoMongo = await GuardarEnMongoAsync(nuevo, id);

                Ui.Aviso(this, "Material registrado en MariaDB con Id " + id + "." + avisoMongo);
                LimpiarCampos();
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
            finally
            {
                btnAlta.Enabled = Sesion.EsPersonal;
            }
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.5 - Modificar: se vuelven a ejecutar las validaciones y se actualizan ambos motores
        //  (UPDATE parametrizado en MariaDB y ReplaceOneAsync por _id en MongoDB). La imagen anterior solo se
        //  borra cuando la nueva ya se copió y la base relacional confirmó el cambio.
        // =====================================================================
        private async void BtnModificar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    Ui.Aviso(this, "Seleccione un material de la tabla para modificarlo.");
                    return;
                }

                if (!ValidarCaptura())
                    return;

                btnModificar.Enabled = false;

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

                if (rutaNueva != null && _rutaImagenActual != null)
                    GestorImagenes.EliminarSiExiste(_rutaImagenActual);

                string avisoMongo = await ActualizarEnMongoAsync(cambios);

                Ui.Aviso(this, "Material actualizado en MariaDB." + avisoMongo);
                LimpiarCampos();
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
            finally
            {
                btnModificar.Enabled = Sesion.EsPersonal;
            }
        }

        private bool ConfirmarBorrado(string mensaje)
        {
            return MessageBox.Show(this, mensaje, "Sistema de Préstamos - Confirmar borrado",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        private Material? MostrarDetalle(bool definitivo)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                Ui.Aviso(this, "Seleccione un material de la tabla.");
                return null;
            }

            Material? material = _dao.ObtenerPorId(id);
            if (material == null)
            {
                Ui.Aviso(this, "El material ya no existe. Se actualizará la lista.");
                LimpiarCampos();
                Cargar();
                return null;
            }

            using (FrmDetalleMaterial detalle = new FrmDetalleMaterial(material, definitivo))
            {
                return detalle.ShowDialog(this) == DialogResult.OK ? material : null;
            }
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.6 - Borrado LÓGICO: detalle de solo lectura, doble confirmación (advertencia Sí/No),
        //  UPDATE en MariaDB (Estado = Baja, Activo = 0) y $set activo = false en MongoDB.
        // =====================================================================
        private async void BtnDesactivar_Click(object? sender, EventArgs e)
        {
            try
            {
                Material? material = MostrarDetalle(false);
                if (material == null)
                    return;

                if (!ConfirmarBorrado("¿Dar de baja el material «" + material.Nombre + "»?\nQuedará oculto en los listados y se conservará su historial."))
                    return;

                btnDesactivar.Enabled = false;

                _dao.Desactivar(material.Id);

                string avisoMongo;
                try
                {
                    bool encontrado = await _mongo.BajaLogicaAsync(material.Id);
                    avisoMongo = encontrado ? "\nEn MongoDB el documento quedó con activo = false." : "\nNo había documento en MongoDB para este material.";
                }
                catch (Exception ex)
                {
                    avisoMongo = "\n\nAtención: la baja se aplicó en MariaDB, pero NO en MongoDB.\n" + ex.Message;
                }

                Ui.Aviso(this, "Material desactivado (estado Baja)." + avisoMongo);
                LimpiarCampos();
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
            finally
            {
                btnDesactivar.Enabled = Sesion.EsAdministrador;
            }
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A4.6 - Borrado FÍSICO: detalle de solo lectura, doble confirmación, DELETE por llave primaria en MariaDB
        //  (con aviso si hay llaves foráneas), DeleteOneAsync por _id en MongoDB y, solo después de confirmar el
        //  borrado en las bases, File.Delete de la imagen (verificando que no esté en uso).
        // =====================================================================
        private async void BtnEliminar_Click(object? sender, EventArgs e)
        {
            try
            {
                Material? material = MostrarDetalle(true);
                if (material == null)
                    return;

                if (!ConfirmarBorrado("¿ELIMINAR DEFINITIVAMENTE el material «" + material.Nombre + "»?\nEsta acción no se puede deshacer."))
                    return;

                btnEliminar.Enabled = false;

                string rutaImagen = material.RutaImagen ?? string.Empty;
                bool tieneImagen = !string.IsNullOrWhiteSpace(rutaImagen);

                _dao.Eliminar(material.Id);

                bool mongoCorrecto = true;
                string avisoMongo;
                try
                {
                    bool borrado = await _mongo.EliminarAsync(material.Id);
                    avisoMongo = borrado ? "\nTambién se eliminó el documento de MongoDB." : "\nNo había documento en MongoDB para este material.";
                }
                catch (Exception ex)
                {
                    mongoCorrecto = false;
                    avisoMongo = "\n\nAtención: se eliminó en MariaDB, pero NO en MongoDB.\n" + ex.Message;
                }

                string avisoImagen = string.Empty;
                if (tieneImagen && mongoCorrecto)
                {
                    MostrarImagen(null);
                    GestorImagenes.TryEliminar(rutaImagen, out string mensajeImagen);
                    avisoImagen = "\n" + mensajeImagen;
                }

                Ui.Aviso(this, "Material eliminado de MariaDB." + avisoMongo + avisoImagen);
                LimpiarCampos();
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
            finally
            {
                btnEliminar.Enabled = Sesion.EsAdministrador;
            }
        }
    }
}
