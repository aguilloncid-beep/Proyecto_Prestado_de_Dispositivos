using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    public class FrmPrestamos : Form
    {
        private readonly UsuarioDAO _usuarios = new UsuarioDAO();
        private readonly MaterialDAO _materiales = new MaterialDAO();
        private readonly PrestamoDAO _prestamos = new PrestamoDAO();

        private readonly TextBox txtAlumno;
        private readonly Label lblAlumno;
        private readonly ComboBox cmbMaterial;
        private readonly DateTimePicker dtpPrestamo;
        private readonly DateTimePicker dtpLimite;
        private readonly DataGridView grid;

        private Usuario? _alumno;

        public FrmPrestamos()
        {
            Sesion.ExigirPersonal();

            Text = "Registrar préstamo";
            ClientSize = new Size(900, 560 + Autoria.AlturaLabel);
            StartPosition = FormStartPosition.CenterParent;

            Ui.Etiqueta(this, "Alumno (correo, No. de estudiante o Id):", 20, 20);
            txtAlumno = Ui.Caja(this, 270, 17, 180);
            txtAlumno.TextChanged += (s, e) => { _alumno = null; lblAlumno.Text = string.Empty; };
            Ui.Boton(this, "Buscar alumno", 460, 15, 110, BtnBuscarAlumno_Click);

            lblAlumno = Ui.Etiqueta(this, string.Empty, 20, 50);

            Ui.Etiqueta(this, "Material disponible:", 20, 85);
            cmbMaterial = new ComboBox();
            cmbMaterial.Location = new Point(150, 82);
            cmbMaterial.Width = 420;
            cmbMaterial.DropDownStyle = ComboBoxStyle.DropDownList;
            Controls.Add(cmbMaterial);

            Ui.Etiqueta(this, "Fecha de préstamo:", 600, 20);
            dtpPrestamo = new DateTimePicker();
            dtpPrestamo.Format = DateTimePickerFormat.Short;
            dtpPrestamo.Location = new Point(730, 17);
            dtpPrestamo.Width = 130;
            dtpPrestamo.MaxDate = DateTime.Today;
            dtpPrestamo.Value = DateTime.Today;
            Controls.Add(dtpPrestamo);

            Ui.Etiqueta(this, "Fecha límite:", 600, 55);
            dtpLimite = new DateTimePicker();
            dtpLimite.Format = DateTimePickerFormat.Short;
            dtpLimite.Location = new Point(730, 52);
            dtpLimite.Width = 130;
            dtpLimite.Value = DateTime.Today.AddDays(3);
            Controls.Add(dtpLimite);

            Ui.Etiqueta(this, "Duración máxima: " + Material.DiasMaximosPrestamo + " días.", 600, 85);
            Ui.Boton(this, "Registrar préstamo", 600, 108, 260, BtnRegistrar_Click);

            Ui.Etiqueta(this, "Préstamos abiertos (Activo / Mora)", 20, 150);
            grid = Ui.Tabla(this, 20, 172, 860, 330);

            Ui.Boton(this, "Cerrar", 770, 515, 110, (s, e) => Close()).Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Autoria.Agregar(this);
            Load += (s, e) => Recargar();
        }

        private void Recargar()
        {
            try
            {
                _prestamos.ActualizarMoras();

                DataTable disponibles = _materiales.ListarDisponibles();
                cmbMaterial.DataSource = disponibles;
                cmbMaterial.DisplayMember = "Texto";
                cmbMaterial.ValueMember = "IdMaterial";
                if (disponibles.Rows.Count == 0) cmbMaterial.SelectedIndex = -1;

                Ui.Enlazar(grid, _prestamos.Consultar(null, null, null, null, true, null));
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void BtnBuscarAlumno_Click(object? sender, EventArgs e)
        {
            try
            {
                Usuario? encontrado = _usuarios.Buscar(txtAlumno.Text, false);
                if (encontrado == null)
                {
                    _alumno = null;
                    lblAlumno.Text = "No se encontró un usuario activo con ese dato.";
                    return;
                }

                int abiertos = _usuarios.ContarPrestamosAbiertos(encontrado.Id);
                _alumno = encontrado;
                string carrera = string.IsNullOrEmpty(encontrado.Carrera) ? "sin carrera" : encontrado.Carrera;
                lblAlumno.Text = encontrado.Nombre + " | " + carrera + " | Préstamos abiertos: " + abiertos +
                                 " de " + Usuario.LimitePrestamosActivos;
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void BtnRegistrar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_alumno == null)
                {
                    Ui.Aviso(this, "Busque y seleccione primero al alumno.");
                    return;
                }

                if (cmbMaterial.SelectedValue == null)
                {
                    Ui.Aviso(this, "No hay un material disponible seleccionado.");
                    return;
                }

                int idMaterial = Convert.ToInt32(cmbMaterial.SelectedValue);
                int id = _prestamos.RegistrarPrestamo(_alumno.Id, idMaterial, dtpPrestamo.Value, dtpLimite.Value);

                Ui.Aviso(this, "Préstamo #" + id + " registrado. Fecha límite: " + dtpLimite.Value.ToShortDateString() + ".");
                txtAlumno.Clear();
                Recargar();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
                Recargar();
            }
        }
    }
}
