using SistemaPresatamos.Models;

namespace SistemaPresatamos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGuardarAdmin_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Instanciamos el objeto de la clase Administrador (del namespace SistemaPresatamos.Models)
                Administrador admin = new Administrador();

                // 2. Capturamos los datos de la Vista (controles) y los asignamos a las propiedades del Modelo.
                // Aquí es donde las validaciones de tus propiedades (get/set) entran en acción automáticamente.
                admin.IdAdmin = int.Parse(txtIdAdmin.Text);
                admin.Correo = txtCorreoAdmin.Text;
                admin.Nombre = txtNombreAdmin.Text;
                admin.Contrasena = txtContrasenaAdmin.Text;
                admin.RutaImagen = txtRutaImagenAdmin.Text; // Asegúrate de que el nombre del TextBox coincida con tu diseño
                admin.Estado = chkEstadoAdmin.Checked;

                // 3. Ejecutamos la función de negocio del modelo
                bool esValido = admin.ValidarCredenciales();

                // 4. Desplegamos el resultado en la sección de salida (puedes usar un TextBox multilínea llamado txtResultadoAdmin)
                txtResultadoAdmin.Text = "=== DATOS REGISTRADOS CORRECTAMENTE ===" + Environment.NewLine +
                                         admin.ToString() + Environment.NewLine +
                                         $"Estado de validación de credenciales: {(esValido ? "Aprobado / Activo" : "Rechazado")}";
            }
            catch (FormatException)
            {
                // Se activa si escriben letras en campos que piden números enteros (como el ID)
                MessageBox.Show("Error de formato: Por favor, introduce un número válido en el campo ID.",
                                "Error de captura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException ex)
            {
                // Se activa cuando el Modelo rechaza un dato (ej. correo sin '@', contraseña menor a 6 caracteres, ID negativo)
                MessageBox.Show(ex.Message,
                                "Validación de Negocio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                // Cualquier otro error imprevisto
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Instanciamos el modelo Usuario
                Usuario usuario = new Usuario();

                // 2. Capturamos los datos desde los controles de la Vista
                usuario.IdUsuario = int.Parse(txtIdUsuario.Text);
                usuario.NumeroEstudiante = txtNumeroEstudiante.Text;
                usuario.Nombre = txtNombreUsuario.Text;

                // Obtenemos la opción seleccionada del ComboBox asegurándonos de que haya una elegida
                if (cmbCarrera.SelectedItem != null)
                {
                    usuario.Carrera = cmbCarrera.SelectedItem?.ToString() ?? string.Empty;
                }
                else
                {
                    throw new ArgumentException("Debe seleccionar una carrera válida del listado.");
                }

                usuario.RutaImagen = txtRutaImagenUsuario.Text;
                usuario.Estado = chkEstadoUsuario.Checked;

                // 3. Desplegamos el resultado (puedes tener un TextBox multilínea llamado txtResultadoUsuario)
                txtResultadoUsuario1.Text = "=== USUARIO REGISTRADO EXITOSAMENTE ===" + Environment.NewLine +
                                           usuario.ToString() + Environment.NewLine +
                                           $"Carrera Asignada: {usuario.Carrera}";
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor, introduce un formato numérico válido en el ID de usuario.",
                                "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException ex)
            {
                // Ataja cualquier regla de validación encapsulada en las propiedades de tu modelo
                MessageBox.Show(ex.Message,
                                "Validación de Negocio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbCarrera.Items.Clear();
            cmbCarrera.Items.Add("Ingeniería en Ciencias Computacionales");
            cmbCarrera.Items.Add("Ingeniería en Energia");
            cmbCarrera.Items.Add("Ingeniería en Nanotecnología");
            cmbCarrera.Items.Add("Licenciatura en Administración");
            cmbCarrera.Items.Add("Licenciatura en Contaduría Pública");

            // Seleccionar la primera opción por defecto si hay elementos
            if (cmbCarrera.Items.Count > 0)
            {
                cmbCarrera.SelectedIndex = 0;
            }
        }

        private void btnGuardarMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Instanciamos el modelo Materiales
                Materiales material = new Materiales();

                // 2. Capturamos los datos desde los controles de la Vista
                material.IdMaterial = int.Parse(txtIdMaterial.Text);
                material.NumeroSerie = txtNumeroSerie.Text;
                material.Nombre = txtNombreMaterial.Text;
                material.RutaImagen = txtRutaImagenMaterial.Text;
                material.Estado = chkEstadoMaterial.Checked;

                // 3. Ejecutamos la función de negocio del modelo (Verificar disponibilidad)
                bool estaDisponible = material.VerificarDisponibilidad();

                // 4. Desplegamos el resultado en el TextBox multilínea
                txtResultadoMaterial.Text = "=== MATERIAL REGISTRADO EXITOSAMENTE ===" + Environment.NewLine +
                                            material.ToString() + Environment.NewLine +
                                            $"¿Listo para préstamo?: {(estaDisponible ? "Sí (Disponible)" : "No (No disponible)")}";
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor, introduce un formato numérico válido en el ID del material.",
                                "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException ex)
            {
                // Ataja las validaciones del modelo (ej. número de serie vacío o ID negativo)
                MessageBox.Show(ex.Message,
                                "Validación de Negocio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarRecreativo_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Instanciamos el modelo Recreativo
                Recreativo recreativo = new Recreativo();

                // 2. Capturamos los datos desde los controles de la Vista
                recreativo.IdRecrea = int.Parse(txtIdRecrea.Text);
                recreativo.Numero = txtNumeroRecreativo.Text;
                recreativo.Nombre = txtNombreRecreativo.Text;
                recreativo.Modelo = txtModeloRecreativo.Text;
                recreativo.RutaImagen = txtRutaImagenRecreativo.Text;
                recreativo.Estado = chkEstadoRecreativo.Checked;

                // 3. Ejecutamos la función de negocio del modelo (Validar disponibilidad)
                bool disponible = recreativo.ValidarDisponibilidad();

                // 4. Desplegamos el resultado en el TextBox multilínea
                txtResultadoRecreativo.Text = "=== ARTÍCULO RECREATIVO REGISTRADO ===" + Environment.NewLine +
                                              recreativo.ToString() + Environment.NewLine +
                                              $"Estatus de disponibilidad: {(disponible ? "Disponible para préstamo" : "No disponible")}";
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor, introduce un formato numérico válido en el ID recreativo.",
                                "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException ex)
            {
                // Ataja las reglas de validación encapsuladas en el modelo (ej. campos vacíos o ID negativo)
                MessageBox.Show(ex.Message,
                                "Validación de Negocio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarPrestamo_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Instanciamos el modelo Prestamo
                Prestamo prestamo = new Prestamo();

                // 2. Capturamos los datos básicos desde los controles
                prestamo.IdPrestamo = int.Parse(txtIdPrestamo.Text);
                prestamo.IdUsuario = int.Parse(txtIdUsuarioPrestamo.Text);
                prestamo.IdElemento = int.Parse(txtIdElementoPrestamo.Text);

                // 3. Capturamos las fechas directamente desde los DateTimePicker
                prestamo.FechaPrestamo = dtpFechaPrestamo.Value;
                prestamo.FechaDevolucion = dtpFechaDevolucion.Value;

                prestamo.Estado = chkEstadoPrestamo.Checked;

                // 4. Validación básica de lógica de fechas (Regla de negocio adicional)
                if (prestamo.FechaDevolucion < prestamo.FechaPrestamo)
                {
                    throw new ArgumentException("La fecha de devolución no puede ser anterior a la fecha de préstamo.");
                }

                // 5. Desplegamos el resultado en el TextBox multilínea
                txtResultadoPrestamo.Text = "=== PRÉSTAMO REGISTRADO EXITOSAMENTE ===" + Environment.NewLine +
                                            prestamo.ToString() + Environment.NewLine +
                                            $"Fecha de Préstamo: {prestamo.FechaPrestamo.ToShortDateString()}" + Environment.NewLine +
                                            $"Fecha de Devolución: {prestamo.FechaDevolucion.ToShortDateString()}";
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor, introduce un formato numérico válido en los campos de ID.",
                                "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException ex)
            {
                // Ataja las reglas de validación de los DateTimePicker o de los setters del modelo
                MessageBox.Show(ex.Message,
                                "Validación de Negocio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarDevolucion_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Instanciamos el modelo Devolucion
                Devolucion devolucion = new Devolucion();

                // 2. Capturamos los datos estrictamente desde los controles del mapa técnico
                devolucion.IdDevolucion = int.Parse(txtIdDevolucion.Text);
                devolucion.DevolucionReal = dtpDevolucionReal.Value;
                devolucion.RutaImagen = txtRutaImagenDevolucion.Text;
                devolucion.Estado = chkEstadoDevolucion.Checked;

                // 3. Desplegamos el resultado en el TextBox multilínea de la vista
                txtResultadoDevolucion.Text = "=== DEVOLUCIÓN REGISTRADA EXITOSAMENTE ===" + Environment.NewLine +
                                              devolucion.ToString() + Environment.NewLine +
                                              $"Fecha Real: {devolucion.DevolucionReal.ToShortDateString()}";
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor, introduce un formato numérico válido en el ID de devolución.",
                                "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException ex)
            {
                // Ataja las reglas de validación encapsuladas en el modelo
                MessageBox.Show(ex.Message,
                                "Validación de Negocio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarApartado_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Instanciamos el modelo Apartado
                Apartado apartado = new Apartado();

                // 2. Capturamos los datos estrictamente desde los controles del mapa técnico
                apartado.IdApartado = int.Parse(txtIdApartado.Text);
                apartado.FechaSolicitud = dtpFechaSolicitud.Value;
                apartado.FechaReserva = dtpFechaReserva.Value;
                apartado.RutaImagen = txtRutaImagenApartado.Text;
                apartado.Estado = chkEstadoApartado.Checked;

                // 3. Validación lógica de fechas (Validación de negocio adicional)
                if (apartado.FechaReserva < apartado.FechaSolicitud)
                {
                    throw new ArgumentException("La fecha de reserva no puede ser anterior a la fecha de solicitud.");
                }

                // 4. Desplegamos el resultado en el TextBox multilínea de la vista
                txtResultadoApartado.Text = "=== APARTADO REGISTRADO EXITOSAMENTE ===" + Environment.NewLine +
                                            apartado.ToString() + Environment.NewLine +
                                            $"Fecha de Solicitud: {apartado.FechaSolicitud.ToShortDateString()}" + Environment.NewLine +
                                            $"Fecha de Reserva: {apartado.FechaReserva.ToShortDateString()}";
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor, introduce un formato numérico válido en el ID de apartado.",
                                "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException ex)
            {
                // Ataja las reglas de validación encapsuladas en el modelo o en las fechas
                MessageBox.Show(ex.Message,
                                "Validación de Negocio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarReparacion_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Instanciamos el modelo Reparacion
                Reparacion reparacion = new Reparacion();

                // 2. Capturamos los datos estrictamente desde los controles del mapa técnico
                reparacion.IdReparacion = int.Parse(txtIdReparacion.Text);
                reparacion.Material = txtMaterialReparacion.Text;
                reparacion.Fecha = dtpFechaReparacion.Value;
                reparacion.RutaImagen = txtRutaImagenReparacion.Text;
                reparacion.Estado = chkEstadoReparacion.Checked;

                // 3. Desplegamos el resultado en el TextBox multilínea de la vista
                txtResultadoReparacion.Text = "=== REPARACIÓN REGISTRADA EXITOSAMENTE ===" + Environment.NewLine +
                                              reparacion.ToString() + Environment.NewLine +
                                              $"Material en Reparación: {reparacion.Material}" + Environment.NewLine +
                                              $"Fecha de Registro: {reparacion.Fecha.ToShortDateString()}";
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor, introduce un formato numérico válido en el ID de reparación.",
                                "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException ex)
            {
                // Ataja las reglas de validación encapsuladas en el modelo
                MessageBox.Show(ex.Message,
                                "Validación de Negocio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Instanciamos el modelo Ubicacion
                Ubicacion ubicacion = new Ubicacion();

                // 2. Capturamos los datos estrictamente desde los controles del mapa técnico
                ubicacion.IdUbicacion = int.Parse(txtIdUbicacion.Text);
                ubicacion.Edificio = txtEdificio.Text;
                ubicacion.Almacen = txtAlmacen.Text;
                ubicacion.RutaImagen = txtRutaImagenUbicacion.Text;
                ubicacion.Estado = chkEstadoUbicacion.Checked;

                // 3. Desplegamos el resultado en el TextBox multilínea de la vista
                txtResultadoUbicacion.Text = "=== UBICACIÓN REGISTRADA EXITOSAMENTE ===" + Environment.NewLine +
                                             ubicacion.ToString() + Environment.NewLine +
                                             $"Edificio: {ubicacion.Edificio}" + Environment.NewLine +
                                             $"Almacén: {ubicacion.Almacen}";
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor, introduce un formato numérico válido en el ID de ubicación.",
                                "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException ex)
            {
                // Ataja las reglas de validación encapsuladas en el modelo
                MessageBox.Show(ex.Message,
                                "Validación de Negocio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarSancion_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Instanciamos el modelo Sancion
                Sancion sancion = new Sancion();

                // 2. Capturamos los datos estrictamente desde los controles del mapa técnico
                sancion.IdSancion = int.Parse(txtIdSancion.Text);
                sancion.Motivo = txtMotivoSancion.Text;
                sancion.Monto = decimal.Parse(txtMontoSancion.Text);
                sancion.Reporte = txtReporteSancion.Text;
                sancion.RutaImagen = txtRutaImagenSancion.Text;
                sancion.ActivaPagada = chkActivaPagada.Checked;
                sancion.Estado = chkEstadoSancion.Checked;

                // 3. Validación de negocio opcional (ej. monto positivo)
                if (sancion.Monto < 0)
                {
                    throw new ArgumentException("El monto de la sanción no puede ser negativo.");
                }

                // 4. Desplegamos el resultado en el TextBox multilínea de la vista
                txtResultadoSancion.Text = "=== SANCIÓN REGISTRADA EXITOSAMENTE ===" + Environment.NewLine +
                                   sancion.ToString() + Environment.NewLine +
                                   $"Motivo: {sancion.Motivo}" + Environment.NewLine +
                                   ("Monto:{sancion.Monto:N2}") + Environment.NewLine +
                                   $"Reporte: {sancion.Reporte}" + Environment.NewLine +
                                   $"Estado de Pago (Activa/Pagada): {sancion.ActivaPagada}";
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor, introduce un formato numérico o decimal válido en los campos correspondientes (ID o Monto).",
                                "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException ex)
            {
                // Ataja las reglas de validación encapsuladas en el modelo o validaciones lógicas
                MessageBox.Show(ex.Message,
                                "Validación de Negocio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}




   
   
    





