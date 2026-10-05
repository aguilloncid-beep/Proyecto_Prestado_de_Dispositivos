using System;
using System.Windows.Forms;
using SistemaPresatamos.Models;

namespace SistemaPresatamos
{
    public partial class Form1 : Form
    {
        private Prestamo _modeloPrestamo;

        public Form1()
        {
            InitializeComponent();

            _modeloPrestamo = new Prestamo();

            CargarOpcionesIniciales();
            ActualizarPanelHistorial();
        }

        private void CargarOpcionesIniciales()
        {
            // Llenar el ComboBox de carreras en la pestaña de usuarios
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
            RegistrarEstadoPrestamo();
        }

        private void btnRegistrarEstado_Click(object sender, EventArgs e)
        {
            RegistrarEstadoPrestamo();
        }

        private void RegistrarEstadoPrestamo()
        {
            int idPrestamo;
            int idUsuario;
            int idElemento;

            if (!int.TryParse(txtIdPrestamo.Text, out idPrestamo) ||
                !int.TryParse(txtIdUsuarioPrestamo.Text, out idUsuario) ||
                !int.TryParse(txtIdElementoPrestamo.Text, out idElemento))
            {
                MessageBox.Show(
                    "ID de préstamo, usuario y elemento deben ser números enteros.",
                    "Dato inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (dtpFechaDevolucion.Value < dtpFechaPrestamo.Value)
            {
                MessageBox.Show(
                    "La fecha de devolución no puede ser anterior a la fecha de préstamo.",
                    "Fechas inválidas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            Prestamo nuevoPrestamo = new Prestamo(
                idPrestamo,
                dtpFechaPrestamo.Value,
                dtpFechaDevolucion.Value,
                txtRutaImagenPrestamo.Text,
                DateTime.Now,
                chkEstadoPrestamo.Checked
            );

            nuevoPrestamo.IdUsuario = idUsuario;
            nuevoPrestamo.IdElemento = idElemento;

            // Primero se guarda el registro y despues se apila su estado
            _modeloPrestamo.InsertarRegistro(nuevoPrestamo);
            _modeloPrestamo.ApilarAccion(nuevoPrestamo);

            txtResultadoPrestamo.Text =
                "PRÉSTAMO REGISTRADO Y APILADO:" + Environment.NewLine +
                "ID Préstamo: " + nuevoPrestamo.Id + Environment.NewLine +
                "ID Usuario: " + nuevoPrestamo.IdUsuario + Environment.NewLine +
                "ID Elemento: " + nuevoPrestamo.IdElemento + Environment.NewLine +
                "Fecha Préstamo: " + nuevoPrestamo.FechaPrestamo.ToShortDateString() + Environment.NewLine +
                "Fecha Devolución: " + nuevoPrestamo.FechaDevolucion.ToShortDateString() + Environment.NewLine +
                "Estado: " + (nuevoPrestamo.EsActivo ? "Activo" : "Finalizado") + Environment.NewLine +
                Environment.NewLine +
                "Cima: " + _modeloPrestamo.InspeccionarCima();

            ActualizarPanelHistorial();
        }

        private void btnDeshacer_Click(object sender, EventArgs e)
        {
            Prestamo revertido = _modeloPrestamo.DesapilarYRevertir();

            if (revertido == null)
            {
                MessageBox.Show(
                    "No hay elementos en la pila para deshacer.",
                    "Historial vacío",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                ActualizarPanelHistorial();
                return;
            }

            // Reflejamos el estado revertido en el formulario.
            txtIdPrestamo.Text = revertido.Id.ToString();
            txtIdUsuarioPrestamo.Text = revertido.IdUsuario.ToString();
            txtIdElementoPrestamo.Text = revertido.IdElemento.ToString();
            dtpFechaPrestamo.Value = revertido.FechaPrestamo;
            dtpFechaDevolucion.Value = revertido.FechaDevolucion;
            txtRutaImagenPrestamo.Text = revertido.RutaImagen;
            chkEstadoPrestamo.Checked = revertido.EsActivo;

            txtResultadoPrestamo.Text =
                "ÚLTIMO PRÉSTAMO DESHECHO:" + Environment.NewLine +
                revertido.ToString() + Environment.NewLine +
                Environment.NewLine +
                "El elemento fue extraído con Pop() y eliminado del almacenamiento.";

            ActualizarPanelHistorial();
        }

        private void btnVaciarHistorial_Click(object sender, EventArgs e)
        {
            _modeloPrestamo.VaciarHistorial();
            txtResultadoPrestamo.Text =
                "HISTORIAL VACIADO" + Environment.NewLine +
                "La pila fue limpiada con Clear().";

            ActualizarPanelHistorial();
        }

        private void ActualizarPanelHistorial()
        {
            lstHistorialPrestamos.Items.Clear();

            Prestamo[] historial = _modeloPrestamo.VolcadoAArregloLineal();

            for (int i = 0; i < historial.Length; i++)
            {
                lstHistorialPrestamos.Items.Add(historial[i]);
            }

            int cantidad = _modeloPrestamo.ContarHistorial();
            lblConteoHistorial.Text = "Elementos en pila: " + cantidad;

            Prestamo cima = _modeloPrestamo.InspeccionarCima();

            if (cima != null)
            {
                lblCimaHistorial.Text = "Siguiente acción a revertir: Préstamo #" + cima.Id;
            }
            else
            {
                lblCimaHistorial.Text = "Siguiente acción a revertir: Ninguna";
            }


            btnDeshacer.Enabled = cantidad > 0;
            btnVaciarHistorial.Enabled = cantidad > 0;
        }


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
