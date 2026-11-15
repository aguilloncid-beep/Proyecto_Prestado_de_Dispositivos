using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    /// <summary>Alta, modificación y baja lógica (desactivación) de materiales, con filtros por categoría y estado.</summary>
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

        public FrmInventario()
        {
            Sesion.ExigirAdministrador();

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
            txtDescripcion = Ui.Caja(this, 490, 380, 300);
            txtDescripcion.Multiline = true;
            txtDescripcion.Height = 55;

            Ui.Etiqueta(this, "El estado «Prestado» lo asigna el registro de un préstamo; «Baja» equivale a desactivar.", 20, 425);

            // ---- Botones
            Ui.Boton(this, "Limpiar", 20, 465, 110, (s, e) => LimpiarCampos());
            Ui.Boton(this, "Alta (nuevo)", 140, 465, 110, BtnAlta_Click);
            Ui.Boton(this, "Modificar", 260, 465, 110, BtnModificar_Click);
            Ui.Boton(this, "Desactivar", 380, 465, 110, BtnDesactivar_Click);
            Ui.Boton(this, "Cerrar", 770, 465, 110, (s, e) => Close());

            LimpiarCampos();
            Autoria.Agregar(this);
            Load += (s, e) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                string categoria = cmbFiltroCategoria.SelectedIndex <= 0 ? string.Empty : cmbFiltroCategoria.Text;
                string estado = cmbFiltroEstado.SelectedIndex <= 0 ? string.Empty : cmbFiltroEstado.Text;

                Ui.Enlazar(grid, _dao.Listar(txtBuscar.Text, categoria, estado));
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

                int id = _dao.Insertar(LeerFormulario());
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

                _dao.Actualizar(LeerFormulario());
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
