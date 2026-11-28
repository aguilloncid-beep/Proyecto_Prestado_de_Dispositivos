using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaPresatamos.Data;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    public class FrmPrincipal : Form
    {
        private readonly MenuStrip menu = new MenuStrip();
        private readonly StatusStrip barra = new StatusStrip();
        private readonly ToolStripStatusLabel lblUsuario = new ToolStripStatusLabel();
        private readonly ToolStripStatusLabel lblRol = new ToolStripStatusLabel();
        private readonly ToolStripStatusLabel lblInicio = new ToolStripStatusLabel();
        private readonly ToolStripStatusLabel lblAvisos = new ToolStripStatusLabel();
        private readonly ToolStripMenuItem mnuVentana = new ToolStripMenuItem("&Ventana");

        private readonly ToolStripMenuItem mnuAdministracion = new ToolStripMenuItem("&Administración");
        private readonly ToolStripMenuItem mnuRegistrarPrestamo = new ToolStripMenuItem();
        private readonly ToolStripMenuItem mnuRegistrarDevolucion = new ToolStripMenuItem();
        private readonly ToolStripMenuItem mnuReporte = new ToolStripMenuItem();
        private readonly ToolStripMenuItem mnuUsuarios = new ToolStripMenuItem();
        private readonly ToolStripMenuItem mnuModulos = new ToolStripMenuItem();
        private readonly ToolStripMenuItem mnuCuentas = new ToolStripMenuItem();
        private readonly ToolStripMenuItem mnuSolicitud = new ToolStripMenuItem();

        public FrmPrincipal()
        {
            Sesion.ExigirSesion();

            IsMdiContainer = true;

            Text = "Sistema de Préstamo de Materiales - " + SesionSistema.RolActivo;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1100, 700);
            WindowState = FormWindowState.Maximized;

            ConstruirMenu();
            ConstruirBarraEstado();
            Autoria.Agregar(this);

            Load += FrmPrincipal_Load;
        }

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
            Configurar(mnuSolicitud, "&Solicitud de préstamo (carrito)",
                (s, e) => AbrirFormulario(typeof(FrmPedidos), () => new FrmPedidos()));
            mnuPrestamos.DropDownItems.Add(mnuSolicitud);
            mnuPrestamos.DropDownItems.Add(Item("Historial de &pedidos",
                (s, e) => AbrirFormulario(typeof(FrmHistorialPedidos), () => new FrmHistorialPedidos())));
            mnuPrestamos.DropDownItems.Add(Item("&Historial",
                (s, e) => AbrirFormulario(typeof(FrmHistorial), () => new FrmHistorial())));
            mnuPrestamos.DropDownItems.Add(Item("&Consulta detallada (con fotografía)",
                (s, e) => AbrirFormulario(typeof(FrmConsultaPrestamos), () => new FrmConsultaPrestamos())));

            Configurar(mnuUsuarios, "&Usuarios",
                (s, e) => AbrirFormulario(typeof(FrmUsuario), () => new FrmUsuario()));
            Configurar(mnuReporte, "&Reporte de préstamos (segundo plano)",
                (s, e) => AbrirFormulario(typeof(FrmReportes), () => new FrmReportes()));
            Configurar(mnuModulos, "&Módulos adicionales (apartados, reparaciones...)",
                (s, e) => AbrirFormulario(typeof(Form1), () => new Form1()));
            Configurar(mnuCuentas, "&Cuentas de acceso (estado Activo/Inactivo)",
                (s, e) => AbrirFormulario(typeof(FrmCuentas), () => new FrmCuentas()));
            mnuAdministracion.DropDownItems.Add(mnuUsuarios);
            mnuAdministracion.DropDownItems.Add(mnuCuentas);
            mnuAdministracion.DropDownItems.Add(mnuReporte);
            mnuAdministracion.DropDownItems.Add(mnuModulos);
            mnuAdministracion.DropDownItems.Add(Item("Respaldo de &datos (JSON / XML)",
                (s, e) => AbrirFormulario(typeof(FrmRespaldo), () => new FrmRespaldo())));

            mnuVentana.DropDownItems.Add(Item("&Cascada", (s, e) => LayoutMdi(MdiLayout.Cascade)));
            mnuVentana.DropDownItems.Add(Item("Mosaico &vertical", (s, e) => LayoutMdi(MdiLayout.TileVertical)));
            mnuVentana.DropDownItems.Add(Item("Mosaico &horizontal", (s, e) => LayoutMdi(MdiLayout.TileHorizontal)));
            mnuVentana.DropDownItems.Add(new ToolStripSeparator());
            mnuVentana.DropDownItems.Add(Item("Cerrar &todas", (s, e) => CerrarTodasLasVentanas()));

            menu.Items.AddRange(new ToolStripItem[] { mnuArchivo, mnuInventario, mnuPrestamos, mnuAdministracion, mnuVentana });

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

        private void FrmPrincipal_Load(object? sender, EventArgs e)
        {
            ConfigurarInterfazPorRol();
            ActualizarAvisos();
        }

        // =====================================================================
        //  Autoría - Equipo 4
        //    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
        //    FLORES Becerra Cid Hector - Integrante 2
        //    GOVEA Mijares Jorge Luis - Integrante 3
        //  A20 - Activación/Desactivación de controles según el rol del objeto de sesión (se ejecuta al cargar):
        //  Administrador = todo; Operador (vendedor) = sin gestión de usuarios/cuentas; Invitado = solo consulta.
        // =====================================================================
        private void ConfigurarInterfazPorRol()
        {
            string rol = SesionSistema.RolActivo;

            if (rol == RolesSistema.Administrador)
            {
            }
            else if (rol == RolesSistema.Operador)
            {
                mnuUsuarios.Visible = false;
                mnuCuentas.Visible = false;
                mnuModulos.Visible = false;
            }
            else
            {
                mnuRegistrarPrestamo.Enabled = false;
                mnuRegistrarDevolucion.Enabled = false;
                mnuSolicitud.Enabled = false;
                mnuReporte.Visible = false;
                mnuUsuarios.Visible = false;
                mnuCuentas.Visible = false;
                mnuModulos.Visible = false;
                mnuAdministracion.Visible = false;
            }
        }

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
                nuevo.MdiParent = this;
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
            foreach (Form hijo in (Form[])this.MdiChildren.Clone())
                hijo.Close();
        }

        private void MnuCerrarSesion_Click(object? sender, EventArgs e)
        {
            CerrarTodasLasVentanas();

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
