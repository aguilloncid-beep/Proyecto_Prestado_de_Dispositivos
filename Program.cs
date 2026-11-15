// =====================================================================
//  Autoría - Equipo 5 
//  Integrantes en orden alfabético por primer apellido
//    FERNANDEZ HERNANDEZ Kevin Isaid - Integrante 1
//    Flores Becerra Cid Hector - Integrante 2
//    Govea Mijares Jorge Luis - Integrante 3
// =====================================================================
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    internal static class Program
    {
        /// <summary>
        ///  Punto de entrada. Flujo: FrmLogin (modal) -> si DialogResult.OK, ventana principal MDI.
        ///  Si el acceso no se autoriza o el usuario cancela, la aplicación termina.
        ///  El login no se oculta con Hide(): se cierra antes de abrir la ventana principal.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            MessageBox.Show(SistemaPresatamos.Data.ConexionBD.ProbarConexion(out string mensaje) ? "OK: " + mensaje : "ERROR: " + mensaje);
            while (true)
            {
                DialogResult resultado;
                using (FrmLogin login = new FrmLogin())
                {
                    resultado = login.ShowDialog();
                }

                if (resultado != DialogResult.OK)
                    break;

                // Los alumnos (acceso con base de datos) usan su vista de consulta; el resto, el MDI.
                bool esAlumno = Sesion.UsuarioActual != null && Sesion.UsuarioActual.Rol == Roles.Alumno;
                Form principal = esAlumno ? new FrmPanelAlumno() : new FrmPrincipal();

                Application.Run(principal);
                principal.Dispose();

                bool volverAlLogin = Sesion.CerrarSesionSolicitada;
                Sesion.Cerrar();   // también limpia SesionSistema

                if (!volverAlLogin)
                    break;
            }
        }
    }
}
