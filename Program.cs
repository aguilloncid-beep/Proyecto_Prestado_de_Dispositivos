// =====================================================================
//  Autoría - Equipo 4
//  Integrantes en orden alfabético por primer apellido
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using SistemaPresatamos.Models;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    internal static class Program
    {
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

                bool esAlumno = Sesion.UsuarioActual != null && Sesion.UsuarioActual.Rol == Roles.Alumno;
                Form principal = esAlumno ? new FrmPanelAlumno() : new FrmPrincipal();

                Application.Run(principal);
                principal.Dispose();

                bool volverAlLogin = Sesion.CerrarSesionSolicitada;
                Sesion.Cerrar();

                if (!volverAlLogin)
                    break;
            }
        }
    }
}
