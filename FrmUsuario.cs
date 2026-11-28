using System;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    public partial class FrmUsuario : FrmBase, IPanelCRUD
    {
        private readonly UsuarioDAO _dao = new UsuarioDAO();

        public FrmUsuario()
        {
            Sesion.ExigirAdministrador();
            InitializeComponent();
            txtId.ReadOnly = true;
            Autoria.Agregar(this);
            cmbRol.SelectedItem = Roles.Alumno;
        }

        private void FrmUsuario_Load(object? sender, EventArgs e)
        {
            Recargar();
        }

        private void Recargar()
        {
            try
            {
                Ui.Enlazar(gridUsuarios, _dao.Listar(null));
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void LimpiarCampos()
        {
            txtId.Clear();
            txtNumeroEstudiante.Clear();
            txtNombre.Clear();
            txtCorreo.Clear();
            txtPassword.Clear();
            cmbRol.SelectedItem = Roles.Alumno;
            cmbCarrera.SelectedIndex = -1;
            cmbCarrera.Text = string.Empty;
            chkEstadoUsuario.Checked = true;
            errorProvider1.Clear();
        }

        private void CargarEnPantalla(Usuario u)
        {
            txtId.Text = u.Id.ToString();
            txtNumeroEstudiante.Text = u.NumeroEstudiante ?? string.Empty;
            txtNombre.Text = u.Nombre;
            txtCorreo.Text = u.Correo;
            txtPassword.Clear();
            cmbRol.SelectedItem = u.Rol;
            cmbCarrera.Text = u.Carrera ?? string.Empty;
            chkEstadoUsuario.Checked = u.EsActivo;
        }

        private Usuario LeerFormulario()
        {
            Usuario u = new Usuario();
            if (int.TryParse(txtId.Text, out int id)) u.Id = id;
            u.Nombre = txtNombre.Text;
            u.Correo = txtCorreo.Text;
            u.NumeroEstudiante = txtNumeroEstudiante.Text;
            u.Carrera = cmbCarrera.Text;
            u.Rol = cmbRol.SelectedItem?.ToString() ?? Roles.Alumno;
            u.EsActivo = chkEstadoUsuario.Checked;
            return u;
        }

        #region Métodos de la Interfaz IPanelCRUD

        public void EjecutarGuardar()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtId.Text))
                {
                    Ui.Aviso(this, "Hay un usuario cargado. Pulse «Limpiar» para un alta nueva o «Modificar» para guardar cambios.");
                    return;
                }

                Usuario nuevo = LeerFormulario();
                int id = _dao.Insertar(nuevo, txtPassword.Text);

                Ui.Aviso(this, "Usuario '" + nuevo.Nombre + "' registrado con Id " + id + " y rol " + nuevo.Rol + ".");
                LimpiarCampos();
                Recargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        public void EjecutarBuscar(string id, ErrorProvider alerta)
        {
            alerta.Clear();

            if (string.IsNullOrWhiteSpace(id))
            {
                alerta.SetError(txtBuscar, "Ingrese un correo, número de estudiante o Id para buscar");
                return;
            }

            try
            {
                Usuario? encontrado = _dao.Buscar(id, true);
                if (encontrado == null)
                {
                    alerta.SetError(txtBuscar, "No se encontró ningún usuario con ese dato");
                    return;
                }

                CargarEnPantalla(encontrado);
                lblEstadoBarra.Text = "Estado: usuario " + encontrado.Id + " cargado.";
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        public void EjecutarActualizar()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    Ui.Aviso(this, "Seleccione o busque un usuario para actualizar.");
                    return;
                }

                Usuario cambios = LeerFormulario();
                string? nuevaPassword = string.IsNullOrEmpty(txtPassword.Text) ? null : txtPassword.Text;
                _dao.Actualizar(cambios, nuevaPassword);

                Ui.Aviso(this, "La información del usuario " + cambios.Id + " ha sido actualizada.");
                LimpiarCampos();
                Recargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        public void EjecutarEliminar(StatusStrip barraEstado)
        {
            try
            {
                if (!int.TryParse(txtId.Text, out int id))
                {
                    Ui.Aviso(this, "Seleccione un usuario para desactivar.");
                    return;
                }

                if (!Ui.Confirmar(this, "¿Desactivar al usuario " + id + "? No podrá iniciar sesión, pero se conserva su historial."))
                    return;

                _dao.Desactivar(id);

                if (barraEstado != null && barraEstado.Items.Count > 0)
                    barraEstado.Items[0].Text = "Estado: usuario " + id + " desactivado.";

                LimpiarCampos();
                Recargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        #endregion

        private void btnGuardar_Click(object? sender, EventArgs e) { EjecutarGuardar(); }
        private void btnActualizar_Click(object? sender, EventArgs e) { EjecutarActualizar(); }
        private void btnDesactivar_Click(object? sender, EventArgs e) { EjecutarEliminar(statusStrip1); }
        private void btnLimpiar_Click(object? sender, EventArgs e) { LimpiarCampos(); }
        private void btnBuscar_Click(object? sender, EventArgs e) { EjecutarBuscar(txtBuscar.Text, errorProvider1); }

        private void gridUsuarios_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                int id = Convert.ToInt32(gridUsuarios.Rows[e.RowIndex].Cells["Id"].Value);
                Usuario? u = _dao.ObtenerPorId(id);
                if (u != null) CargarEnPantalla(u);
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }
    }
}
