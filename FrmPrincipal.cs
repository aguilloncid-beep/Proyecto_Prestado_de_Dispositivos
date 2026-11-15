using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    /// <summary>
    /// Formulario maestro MDI (Parent) del administrador. Centraliza la navegación: todos los módulos
    /// se abren como ventanas hijas dentro de este contenedor (no pueden salir de sus límites) y
    /// solo puede haber una instancia de cada módulo (AbrirFormulario). Reemplaza al antiguo panel de botones (SDI).
    /// Las opciones visibles dependen del perfil guardado en SesionSistema (ver FrmPrincipal_Load).
    /// </summary>
    public class FrmPrincipal : Form
    {
        private readonly MenuStrip menu = new MenuStrip();
        private readonly StatusStrip barra = new StatusStrip();
        private readonly ToolStripStatusLabel lblUsuario = new ToolStripStatusLabel();
        private readonly ToolStripStatusLabel lblRol = new ToolStripStatusLabel();
        private readonly ToolStripStatusLabel lblInicio = new ToolStripStatusLabel();
        private readonly ToolStripStatusLabel lblAvisos = new ToolStripStatusLabel();
        private readonly ToolStripMenuItem mnuVentana = new ToolStripMenuItem("&Ventana");

        // Opciones que se restringen según el perfil.
        private readonly ToolStripMenuItem mnuAdministracion = new ToolStripMenuItem("&Administración");
        private readonly ToolStripMenuItem mnuRegistrarPrestamo = new ToolStripMenuItem();
        private readonly ToolStripMenuItem mnuRegistrarDevolucion = new ToolStripMenuItem();
        private readonly ToolStripMenuItem mnuReporte = new ToolStripMenuItem();
        private readonly ToolStripMenuItem mnuUsuarios = new ToolStripMenuItem();
        private readonly ToolStripMenuItem mnuModulos = new ToolStripMenuItem();

        public FrmPrincipal()
        {
            Sesion.ExigirSesion();

            // Convierte el formulario en contenedor MDI (el lienzo adopta el fondo gris oscuro del espacio de trabajo).
            IsMdiContainer = true;

            Text = "Sistema de Préstamo de Materiales - Administrador";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1100, 700);
            WindowState = FormWindowState.Maximized;

            ConstruirMenu();
            ConstruirBarraEstado();
            Autoria.Agregar(this);

            Load += FrmPrincipal_Load;
        }

        // ------------------------------------------------------------------ Menú

        private void ConstruirMenu()
        {
            ToolStripMenuItem mnuArchivo = new ToolStripMenuItem("&Archivo");
            mnuArchivo.DropDownItems.Add(Item("&Información de sesión",
                (s, e) => AbrirFormulario(typeof(FrmInfoSesion), () => new FrmInfoSesion())));
            mnuArchivo.DropDownItems.Add(new ToolStripSeparator());
            mnuArchivo.DropDownItems.Add(Item("&Cerrar sesión", MnuCerrarSesion_Click));
            mnuArchivo.DropDownItems.Add(new ToolStripSeparator());
            mnuArchivo.DropDownItems.Add(Item("&Salir", (s, e) => Close()));

            ToolStripMenuItem mnuInventario = new ToolStripMenuItem("&Inventario");
            mnuInventario.DropDownItems.Add(Item("&Materiales (alta, modificación, baja)",
                (s, e) => AbrirFormulario(typeof(FrmInventario), () => new FrmInventario())));

            ToolStripMenuItem mnuPrestamos = new ToolStripMenuItem("&Préstamos");
            Configurar(mnuRegistrarPrestamo, "Registrar &préstamo",
                (s, e) => AbrirFormulario(typeof(FrmPrestamos), () => new FrmPrestamos()));
            Configurar(mnuRegistrarDevolucion, "Registrar &devolución",
                (s, e) => AbrirFormulario(typeof(FrmDevolucion), () => new FrmDevolucion()));
            mnuPrestamos.DropDownItems.Add(mnuRegistrarPrestamo);
            mnuPrestamos.DropDownItems.Add(mnuRegistrarDevolucion);
            mnuPrestamos.DropDownItems.Add(Item("&Historial",
                (s, e) => AbrirFormulario(typeof(FrmHistorial), () => new FrmHistorial())));
            mnuPrestamos.DropDownItems.Add(Item("&Consulta detallada (con fotografía)",
                (s, e) => AbrirFormulario(typeof(FrmConsultaPrestamos), () => new FrmConsultaPrestamos())));   // A4.3

            Configurar(mnuUsuarios, "&Usuarios",
                (s, e) => AbrirFormulario(typeof(FrmUsuario), () => new FrmUsuario()));
            Configurar(mnuReporte, "&Reporte de préstamos (segundo plano)",
                (s, e) => AbrirFormulario(typeof(FrmReportes), () => new FrmReportes()));
            Configurar(mnuModulos, "&Módulos adicionales (apartados, reparaciones...)",
                (s, e) => AbrirFormulario(typeof(Form1), () => new Form1()));
            mnuAdministracion.DropDownItems.Add(mnuUsuarios);
            mnuAdministracion.DropDownItems.Add(mnuReporte);
            mnuAdministracion.DropDownItems.Add(mnuModulos);
            mnuAdministracion.DropDownItems.Add(Item("Respaldo de &datos (JSON / XML)",
                (s, e) => AbrirFormulario(typeof(FrmRespaldo), () => new FrmRespaldo())));   // A4.2

            // Menú Ventana: organiza las ventanas hijas abiertas.
            mnuVentana.DropDownItems.Add(Item("&Cascada", (s, e) => LayoutMdi(MdiLayout.Cascade)));
            mnuVentana.DropDownItems.Add(Item("Mosaico &vertical", (s, e) => LayoutMdi(MdiLayout.TileVertical)));
            mnuVentana.DropDownItems.Add(Item("Mosaico &horizontal", (s, e) => LayoutMdi(MdiLayout.TileHorizontal)));
            mnuVentana.DropDownItems.Add(new ToolStripSeparator());
            mnuVentana.DropDownItems.Add(Item("Cerrar &todas", (s, e) => CerrarTodasLasVentanas()));

            menu.Items.AddRange(new ToolStripItem[] { mnuArchivo, mnuInventario, mnuPrestamos, mnuAdministracion, mnuVentana });

            // C# agrega automáticamente al final de "Ventana" la lista de formularios hijos abiertos.
            menu.MdiWindowListItem = mnuVentana;

            MainMenuStrip = menu;
            Controls.Add(menu);
        }

        private static void Configurar(ToolStripMenuItem item, string texto, EventHandler alHacerClic)
        {
            item.Text = texto;
            item.Click += alHacerClic;
        }

        private static ToolStripMenuItem Item(string texto, EventHandler alHacerClic)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(texto);
            item.Click += alHacerClic;
            return item;
        }

        private void ConstruirBarraEstado()
        {
            // Datos permanentes de la sesión, leídos de la clase estática SesionSistema.
            lblUsuario.Text = "Usuario: " + SesionSistema.UsuarioActivo;
            lblRol.Text = "Rol: " + SesionSistema.RolActivo;
            lblInicio.Text = "Inicio de sesión: " + SesionSistema.HoraInicioSesion.ToString("dd/MM/yyyy HH:mm:ss");
            lblAvisos.Spring = true;
            lblAvisos.TextAlign = ContentAlignment.MiddleRight;
            barra.Items.AddRange(new ToolStripItem[]
            {
                lblUsuario, new ToolStripStatusLabel("|"), lblRol, new ToolStripStatusLabel("|"), lblInicio, lblAvisos
            });
            Controls.Add(barra);
        }

        // ------------------------------------------------------------------ Restricciones por perfil

        /// <summary>
        /// Aplica las restricciones según SesionSistema.RolActivo con Enabled = false (deshabilitar) y
        /// Visible = false (ocultar).
        ///  - Administrador: acceso completo.
        ///  - Operador: consulta, registro y actualización; sin Usuarios ni Módulos adicionales.
        ///  - Invitado: solo consulta (inventario e historial).
        /// </summary>
        private void FrmPrincipal_Load(object? sender, EventArgs e)
        {
            string rol = SesionSistema.RolActivo;

            if (rol == RolesSistema.Administrador)
            {
                // Acceso completo: no se restringe ninguna opción.
            }
            else if (rol == RolesSistema.Operador)
            {
                mnuUsuarios.Visible = false;
                mnuModulos.Visible = false;
            }
            else
            {
                // Invitado u otro perfil: únicamente consulta.
                mnuRegistrarPrestamo.Enabled = false;
                mnuRegistrarDevolucion.Enabled = false;
                mnuReporte.Visible = false;
                mnuUsuarios.Visible = false;
                mnuModulos.Visible = false;
                mnuAdministracion.Visible = false;
            }

            ActualizarAvisos();
        }

        // ------------------------------------------------------------------ Instancia única de ventanas hijas

        /// <summary>
        /// Abre un formulario hijo MDI. Antes de crear uno nuevo recorre MdiChildren: si ya existe una ventana
        /// de ese tipo la restaura (si está minimizada), la trae al frente y aborta la creación.
        /// </summary>
        private void AbrirFormulario(Type tipo, Func<Form> fabrica)
        {
            foreach (Form hijo in this.MdiChildren)
            {
                if (hijo.GetType() == tipo)
                {
                    if (hijo.WindowState == FormWindowState.Minimized)
                        hijo.WindowState = FormWindowState.Normal;

                    hijo.Activate();
                    return;
                }
            }

            try
            {
                Form nuevo = fabrica();
                nuevo.MdiParent = this;                           // obligatorio antes de Show()
                nuevo.FormClosed += (s, e) => ActualizarAvisos();
                nuevo.Show();
            }
            catch (Exception ex)
            {
                Ui.Error(this, ex);
            }
        }

        private void CerrarTodasLasVentanas()
        {
            // Se copia la lista porque cerrar una ventana modifica MdiChildren.
            foreach (Form hijo in (Form[])this.MdiChildren.Clone())
                hijo.Close();
        }

        // ------------------------------------------------------------------ Sesión y avisos

        private void MnuCerrarSesion_Click(object? sender, EventArgs e)
        {
            CerrarTodasLasVentanas();

            // Una ventana (p. ej. un reporte en curso) puede negarse a cerrar mientras detiene su proceso.
            if (this.MdiChildren.Length > 0)
            {
                Ui.Aviso(this, "Hay un proceso en ejecución que se está deteniendo. Inténtelo de nuevo en un momento.");
                return;
            }

            Sesion.CerrarSesionSolicitada = true;
            Close();
        }

        private void ActualizarAvisos()
        {
            if (IsDisposed || Disposing) return;

            try
            {
                PrestamoDAO dao = new PrestamoDAO();
                dao.ActualizarMoras();
                int enMora = dao.ContarEnMora();
                lblAvisos.Text = enMora == 0 ? "Sin préstamos en mora." : "Atención: " + enMora + " préstamo(s) en mora.";
                lblAvisos.ForeColor = enMora == 0 ? SystemColors.ControlText : Color.Firebrick;
            }
            catch (Exception ex)
            {
                lblAvisos.Text = "No se pudo consultar la base de datos: " + ex.Message;
                lblAvisos.ForeColor = Color.Firebrick;
            }
        }
    }
}
