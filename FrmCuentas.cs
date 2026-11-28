// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    // A20 - Gestión de cuentas de acceso (solo Administrador): alta, cambio de rol, contraseña y Estado.
    public class FrmCuentas : Form
    {
        private readonly CuentaAccesoDAO _dao = new CuentaAccesoDAO();

        private readonly TextBox txtId;
        private readonly TextBox txtUsuario;
        private readonly TextBox txtNombre;
        private readonly TextBox txtPassword;
        private readonly ComboBox cmbRol;
        private readonly ComboBox cmbEstado;
        private readonly DataGridView grid;

        public FrmCuentas()
        {
            Sesion.ExigirAdministrador();

            Text = "Cuentas de acceso";
            ClientSize = new Size(700, 500 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;

            Ui.Etiqueta(this, "Id:", 20, 23);
            txtId = Ui.Caja(this, 110, 20, 80);
            txtId.ReadOnly = true;

            Ui.Etiqueta(this, "Usuario:", 20, 53);
            txtUsuario = Ui.Caja(this, 110, 50, 200);

            Ui.Etiqueta(this, "Nombre:", 20, 83);
            txtNombre = Ui.Caja(this, 110, 80, 250);

            Ui.Etiqueta(this, "Contraseña:", 20, 113);
            txtPassword = Ui.Caja(this, 110, 110, 200, true);
            Ui.Etiqueta(this, "Al modificar, déjela vacía para conservar la actual (mín. 8 caracteres con letras y números).", 320, 113);

            Ui.Etiqueta(this, "Rol:", 20, 143);
            cmbRol = Ui.Combo(this, 110, 140, 200, new[] { RolesSistema.Administrador, RolesSistema.Operador, RolesSistema.Invitado });

            Ui.Etiqueta(this, "Estado:", 20, 173);
            cmbEstado = Ui.Combo(this, 110, 170, 200, new[] { CuentaAcceso.EstadoActivo, CuentaAcceso.EstadoInactivo });

            Ui.Boton(this, "Alta (nuevo)", 20, 210, 110, BtnAlta_Click);
            Ui.Boton(this, "Modificar", 140, 210, 110, BtnModificar_Click);
            Ui.Boton(this, "Activar", 260, 210, 110, (s, e) => CambiarEstado(CuentaAcceso.EstadoActivo));
            Ui.Boton(this, "Desactivar", 380, 210, 110, (s, e) => CambiarEstado(CuentaAcceso.EstadoInactivo));
            Ui.Boton(this, "Limpiar", 500, 210, 90, (s, e) => Limpiar());

            grid = Ui.Tabla(this, 20, 255, 660, 190);
            grid.CellClick += Grid_CellClick;

            Ui.Boton(this, "Cerrar", 570, 455, 110, (s, e) => Close());

            Limpiar();
            Autoria.Agregar(this);
            Load += (s, e) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                Ui.Enlazar(grid, _dao.Listar());
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void Limpiar()
        {
            txtId.Clear();
            txtUsuario.Clear();
            txtUsuario.ReadOnly = false;
            txtNombre.Clear();
            txtPassword.Clear();
            cmbRol.SelectedItem = RolesSistema.Operador;
            cmbEstado.SelectedItem = CuentaAcceso.EstadoActivo;
            grid?.ClearSelection();
        }

        private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = grid.Rows[e.RowIndex];
            txtId.Text = Ui.Texto(fila, "Id");
            txtUsuario.Text = Ui.Texto(fila, "Usuario");
            txtUsuario.ReadOnly = true;
            txtNombre.Text = Ui.Texto(fila, "Nombre");
            txtPassword.Clear();
            cmbRol.SelectedItem = Ui.Texto(fila, "Rol");
            cmbEstado.SelectedItem = Ui.Texto(fila, "Estado");
        }

        private void BtnAlta_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtId.Text))
                {
                    Ui.Aviso(this, "Hay una cuenta seleccionada. Pulse «Limpiar» para capturar una nueva.");
                    return;
                }

                _dao.Crear(txtUsuario.Text, txtNombre.Text, cmbRol.Text, txtPassword.Text);
                Ui.Aviso(this, "Cuenta creada con rol " + cmbRol.Text + ".");
                Limpiar();
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
                if (!int.TryParse(txtId.Text, out int id))
                {
                    Ui.Aviso(this, "Seleccione una cuenta de la tabla para modificarla.");
                    return;
                }

                _dao.Actualizar(id, txtNombre.Text, cmbRol.Text, cmbEstado.Text, txtPassword.Text);
                Ui.Aviso(this, "Cuenta actualizada.");
                Limpiar();
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void CambiarEstado(string estado)
        {
            try
            {
                if (!int.TryParse(txtId.Text, out int id))
                {
                    Ui.Aviso(this, "Seleccione una cuenta de la tabla.");
                    return;
                }

                _dao.CambiarEstado(id, estado);
                Ui.Aviso(this, "La cuenta quedó en estado " + estado + ".");
                Limpiar();
                Cargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }
    }
}
