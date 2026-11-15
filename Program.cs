using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos
{
    internal static class Program
    {
       
        ///  Punto de entrada: inicio de sesión y apertura del panel según el rol.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            while (true)
            {
                using (LoginForm login = new LoginForm())
                {
                    if (login.ShowDialog() != DialogResult.OK)
                        break;
                }

                // Panel distinto según el rol: el alumno nunca recibe las ventanas de administración.
                Form panel = Sesion.EsAdministrador ? new FrmPanelAdmin() : new FrmPanelAlumno();
                Application.Run(panel);
                panel.Dispose();

                bool volverAlLogin = Sesion.CerrarSesionSolicitada;
                Sesion.Cerrar();

                if (!volverAlLogin)
                    break;
            }
        }
    }
}
