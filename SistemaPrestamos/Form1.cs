using System;
using System.Windows.Forms;

namespace SistemaPresatamos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CargarOpcionesIniciales();
        }

        private void CargarOpcionesIniciales()
        {
            // Llenar el ComboBox de carreras en la pestaña de Usuarios
            cmbCarrera.Items.Add("Ingeniería Informática");
            cmbCarrera.Items.Add("Ingeniería en Sistemas");
            cmbCarrera.Items.Add("Licenciatura en Administración");
            cmbCarrera.Items.Add("Ingeniería Industrial");
            if (cmbCarrera.Items.Count > 0)
                cmbCarrera.SelectedIndex = 0;
        }

        // 1. Administrador
        private void btnGuardarAdmin_Click(object sender, EventArgs e)
        {
            string id = txtIdAdmin.Text;
            string correo = txtCorreoAdmin.Text;
            string nombre = txtNombreAdmin.Text;
            string pass = txtContrasenaAdmin.Text;
            string ruta = txtRutaImagenAdmin.Text;
            bool estado = chkEstadoAdmin.Checked;

            txtResultadoAdmin.Text = $"ADMINISTRADOR REGISTRADO:\r\n" +
                                     $"ID: {id}\r\n" +
                                     $"Correo: {correo}\r\n" +
                                     $"Nombre: {nombre}\r\n" +
                                     $"Ruta Imagen: {ruta}\r\n" +
                                     $"Estado: {(estado ? "Disponible" : "No disponible")}";
        }

        // 2. Usuario
        private void btnGuardarUsuario_Click(object sender, EventArgs e)
        {
            string id = txtIdUsuario.Text;
            string numEstudiante = txtNumeroEstudiante.Text;
            string nombre = txtNombreUsuario.Text;
            string carrera = cmbCarrera.SelectedItem?.ToString() ?? "";
            string ruta = txtRutaImagenUsuario.Text;
            bool estado = chkEstadoUsuario.Checked;

            txtResultadoUsuario1.Text = $"USUARIO REGISTRADO:\r\n" +
                                        $"ID Usuario: {id}\r\n" +
                                        $"No. Estudiante: {numEstudiante}\r\n" +
                                        $"Nombre: {nombre}\r\n" +
                                        $"Carrera: {carrera}\r\n" +
                                        $"Ruta Imagen: {ruta}\r\n" +
                                        $"Estado: {(estado ? "Activo" : "Inactivo")}";
        }

        // 3. Materiales
        private void btnGuardarMaterial_Click(object sender, EventArgs e)
        {
            string id = txtIdMaterial.Text;
            string serie = txtNumeroSerie.Text;
            string nombre = txtNombreMaterial.Text;
            string ruta = txtRutaImagenMaterial.Text;
            bool estado = chkEstadoMaterial.Checked;

            txtResultadoMaterial.Text = $"MATERIAL REGISTRADO:\r\n" +
                                        $"ID Material: {id}\r\n" +
                                        $"No. Serie: {serie}\r\n" +
                                        $"Nombre: {nombre}\r\n" +
                                        $"Ruta Imagen: {ruta}\r\n" +
                                        $"Estado: {(estado ? "En uso" : "Disponible")}";
        }

        // 4. Recreativos
        private void btnGuardarRecreativo_Click(object sender, EventArgs e)
        {
            string id = txtIdRecrea.Text;
            string numSerie = txtNumeroRecreativo.Text;
            string nombre = txtNombreRecreativo.Text;
            string modelo = txtModeloRecreativo.Text;
            string ruta = txtRutaImagenRecreativo.Text;
            bool estado = chkEstadoRecreativo.Checked;

            txtResultadoRecreativo.Text = $"RECREATIVO REGISTRADO:\r\n" +
                                          $"ID: {id}\r\n" +
                                          $"No. Serie/Código: {numSerie}\r\n" +
                                          $"Nombre: {nombre}\r\n" +
                                          $"Modelo: {modelo}\r\n" +
                                          $"Ruta Imagen: {ruta}\r\n" +
                                          $"Estado: {(estado ? "Activo" : "Inactivo")}";
        }

        // 5. Préstamos
        private void btnGuardarPrestamo_Click(object sender, EventArgs e)
        {
            string idPrestamo = txtIdPrestamo.Text;
            string idUsuario = txtIdUsuarioPrestamo.Text;
            string idElemento = txtIdElementoPrestamo.Text;
            string fechaP = dtpFechaPrestamo.Value.ToShortDateString();
            string fechaD = dtpFechaDevolucion.Value.ToShortDateString();
            string ruta = txtRutaImagenPrestamo.Text;
            bool estado = chkEstadoPrestamo.Checked;

            txtResultadoPrestamo.Text = $"PRÉSTAMO REGISTRADO:\r\n" +
                                        $"ID Préstamo: {idPrestamo}\r\n" +
                                        $"ID Usuario: {idUsuario}\r\n" +
                                        $"ID Elemento: {idElemento}\r\n" +
                                        $"Fecha Préstamo: {fechaP}\r\n" +
                                        $"Fecha Devolución: {fechaD}\r\n" +
                                        $"Ruta Imagen: {ruta}\r\n" +
                                        $"Estado: {(estado ? "Activo" : "Finalizado")}";
        }

        // 6. Devoluciones
        private void btnGuardarDevolucion_Click(object sender, EventArgs e)
        {
            string id = txtIdDevolucion.Text;
            string fechaReal = dtpDevolucionReal.Value.ToShortDateString();
            string ruta = txtRutaImagenDevolucion.Text;
            bool estado = chkEstadoDevolucion.Checked;

            txtResultadoDevolucion.Text = $"DEVOLUCIÓN REGISTRADA:\r\n" +
                                          $"ID Devolución: {id}\r\n" +
                                          $"Fecha Real: {fechaReal}\r\n" +
                                          $"Ruta Imagen: {ruta}\r\n" +
                                          $"Estado: {(estado ? "Completado" : "Pendiente")}";
        }

        // 7. Apartados
        private void btnGuardarApartado_Click(object sender, EventArgs e)
        {
            string id = txtIdApartado.Text;
            string fechaSolicitud = dtpFechaSolicitud.Value.ToShortDateString();
            string fechaReserva = dtpFechaReserva.Value.ToShortDateString();
            string ruta = txtRutaImagenApartado.Text;
            bool estado = chkEstadoApartado.Checked;

            txtResultadoApartado.Text = $"APARTADO REGISTRADO:\r\n" +
                                        $"ID Apartado: {id}\r\n" +
                                        $"Fecha Solicitud: {fechaSolicitud}\r\n" +
                                        $"Fecha Reserva: {fechaReserva}\r\n" +
                                        $"Ruta Imagen: {ruta}\r\n" +
                                        $"Estado: {(estado ? "Confirmado" : "Cancelado/Pendiente")}";
        }

        // 8. Reparaciones
        private void btnGuardarReparacion_Click(object sender, EventArgs e)
        {
            string id = txtIdReparacion.Text;
            string material = txtMaterialReparacion.Text;
            string fecha = dtpFechaReparacion.Value.ToShortDateString();
            string ruta = txtRutaImagenReparacion.Text;
            bool estado = chkEstadoReparacion.Checked;

            txtResultadoReparacion.Text = $"REPARACIÓN REGISTRADA:\r\n" +
                                          $"ID Reparación: {id}\r\n" +
                                          $"Material: {material}\r\n" +
                                          $"Fecha Reparación: {fecha}\r\n" +
                                          $"Ruta Imagen: {ruta}\r\n" +
                                          $"Estado: {(estado ? "En Proceso" : "Concluido")}";
        }

        // 9. Ubicaciones
        private void btnGuardarUbicacion_Click(object sender, EventArgs e)
        {
            string id = txtIdUbicacion.Text;
            string edificio = txtEdificio.Text;
            string almacen = txtAlmacen.Text;
            string ruta = txtRutaImagenUbicacion.Text;
            bool estado = chkEstadoUbicacion.Checked;

            txtResultadoUbicacion.Text = $"UBICACIÓN REGISTRADA:\r\n" +
                                         $"ID Ubicación: {id}\r\n" +
                                         $"Edificio: {edificio}\r\n" +
                                         $"Almacén: {almacen}\r\n" +
                                         $"Ruta Imagen: {ruta}\r\n" +
                                         $"Estado: {(estado ? "Disponible" : "Ocupado")}";
        }

        // 10. Sanciones
        private void btnGuardarSancion_Click(object sender, EventArgs e)
        {
            string id = txtIdSancion.Text;
            string motivo = txtMotivoSancion.Text;
            string monto = txtMontoSancion.Text;
            string reporte = txtReporteSancion.Text;
            string ruta = txtRutaImagenSancion.Text;
            bool activaPagada = chkActivaPagada.Checked;
            bool estado = chkEstadoSancion.Checked;

            txtResultadoSancion.Text = $"SANCIÓN REGISTRADA:\r\n" +
                                       $"ID Sanción: {id}\r\n" +
                                       $"Motivo: {motivo}\r\n" +
                                       $"Monto: ${monto}\r\n" +
                                       $"Reporte: {reporte}\r\n" +
                                       $"Ruta Imagen: {ruta}\r\n" +
                                       $"Condición: {(activaPagada ? "Pagada" : "Activa")}\r\n" +
                                       $"Estado: {(estado ? "Vigente" : "Expirada")}";
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}
