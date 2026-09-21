using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SistemaPresatamos
{
    public partial class FrmAdministrador : FrmBase, IPanelCRUD
    {
        public FrmAdministrador()
        {
            InitializeComponent();
        }

        private void FrmAdministrador_Load(object sender, EventArgs e)
        {
            // Evento de carga inicial si se requiere
        }

        #region Métodos de la Interfaz IPanelCRUD (Módulo Administrador)

        public void EjecutarGuardar()
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Por favor, ingrese el IdAdmin.", "Atención",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Por favor, ingrese el Nombre del administrador.", "Atención",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            string idAdmin = txtId.Text; // Heredado de FrmBase
            string correo = txtCorreo.Text;
            string nombre = txtNombre.Text;
            string contrasena = txtContrasena.Text;
            string rutaImagen = txtRutaImagen.Text;
            bool disponible = chkEstadoAdmin.Checked;

            MessageBox.Show($"[Guardar] Administrador '{nombre}' con ID '{idAdmin}' registrado correctamente.",
                            "Sistema de Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void EjecutarBuscar(string id, ErrorProvider alerta)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                alerta.SetError(txtId, "Ingrese un IdAdmin válido para buscar");
                return;
            }
            alerta.Clear();

            // Carga de datos de prueba idénticos al diseño
            txtId.Text = id;
            txtCorreo.Text = "cid.flores@udg.mx";
            txtNombre.Text = "Flores Becerra Cid Héctor";
            txtContrasena.Text = "admin123";
            txtRutaImagen.Text = "C:/Imagenes/admin.png";
            chkEstadoAdmin.Checked = true;

            MessageBox.Show($"[Buscar] Datos del Administrador ID '{id}' cargados en pantalla.",
                            "Sistema de Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void EjecutarActualizar()
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Seleccione o busque un administrador para actualizar.", "Atención",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show($"[Actualizar] Administrador ID '{txtId.Text}' actualizado con éxito.",
                            "Sistema de Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void EjecutarEliminar(StatusStrip barraEstado)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Seleccione un administrador para eliminar.", "Atención",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string id = txtId.Text;

            // Limpieza de interfaz
            txtId.Clear();
            txtCorreo.Clear();
            txtNombre.Clear();
            txtContrasena.Clear();
            txtRutaImagen.Clear();
            chkEstadoAdmin.Checked = false;

            if (barraEstado != null && barraEstado.Items.Count > 0)
            {
                barraEstado.Items[0].Text = $"Estado: Administrador {id} eliminado.";
            }

            MessageBox.Show($"[Eliminar] Registro de Administrador '{id}' eliminado.",
                            "Sistema de Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}