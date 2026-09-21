using System;
using System.Windows.Forms;

namespace SistemaPresatamos
{
    public partial class FrmUsuario : FrmBase, IPanelCRUD
    {
        public FrmUsuario()
        {
            InitializeComponent();
        }

        private void FrmUsuario_Load(object sender, EventArgs e)
        {
            // Opcional: Cargar opciones iniciales si no las agregaste desde el diseñador
            if (cmbCarrera.Items.Count == 0)
            {
                cmbCarrera.Items.Add("Ingeniería en Computación");
                cmbCarrera.Items.Add("Ingeniería Informática");
                cmbCarrera.Items.Add("Licenciatura en Sistemas");
            }
        }

        #region Métodos de la Interfaz IPanelCRUD (Lógica del Módulo)

        public void EjecutarGuardar()
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Por favor, ingrese el IdUsuario.", "Atención",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Por favor, ingrese el Nombre del estudiante.", "Atención",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            // Lectura de variables desde la interfaz
            string idUsuario = txtId.Text; // Heredado de FrmBase
            string numEstudiante = txtNumeroEstudiante.Text;
            string nombre = txtNombre.Text;
            string carrera = cmbCarrera.SelectedItem?.ToString() ?? "Sin Carrera";
            string rutaImagen = txtRutaImagen.Text;
            bool estado = chkEstadoUsuario.Checked;

            // AQUÍ PUEDES INSTANCIAR TU CLASE DE MODELO 'Usuario' SI LA TIENES:
            // Usuario nuevoUsuario = new Usuario(idUsuario, numEstudiante, nombre, carrera, rutaImagen, estado);
            // listaUsuarios.Add(nuevoUsuario);

            MessageBox.Show($"[Guardar] Usuario '{nombre}' (No. {numEstudiante}) registrado con éxito.",
                            "Sistema de Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void EjecutarBuscar(string id, ErrorProvider alerta)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                alerta.SetError(txtId, "Ingrese un ID de usuario válido para buscar");
                return;
            }
            alerta.Clear();

            // Simulación de búsqueda (o búsqueda en tu lista/BD)
            txtId.Text = id;
            txtNumeroEstudiante.Text = "218765432";
            txtNombre.Text = "Kevin Isaid Fernández";
            cmbCarrera.SelectedItem = "Ingeniería en Computación";
            txtRutaImagen.Text = "C:/Imagenes/kevin.png";
            chkEstadoUsuario.Checked = true;

            MessageBox.Show($"[Buscar] Datos del Usuario ID '{id}' cargados en pantalla.",
                            "Sistema de Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void EjecutarActualizar()
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Seleccione o busque un usuario para actualizar.", "Atención",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show($"[Actualizar] La información del Usuario ID '{txtId.Text}' ha sido actualizada.",
                            "Sistema de Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void EjecutarEliminar(StatusStrip barraEstado)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Seleccione un usuario para eliminar.", "Atención",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string id = txtId.Text;

            // Limpieza de campos
            txtId.Clear();
            txtNumeroEstudiante.Clear();
            txtNombre.Clear();
            cmbCarrera.SelectedIndex = -1;
            txtRutaImagen.Clear();
            chkEstadoUsuario.Checked = false;

            if (barraEstado != null && barraEstado.Items.Count > 0)
            {
                barraEstado.Items[0].Text = $"Estado: Usuario {id} eliminado correctamente.";
            }

            MessageBox.Show($"[Eliminar] Registro de Usuario '{id}' eliminado.",
                            "Sistema de Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        private void pnlFormularioBase_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
