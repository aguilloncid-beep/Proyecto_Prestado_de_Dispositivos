// =====================================================================
//  Autoría - Equipo 4
//    FERNANDEZ Hernandez Kevin Isaid - Integrante 1
//    FLORES Becerra Cid Hector - Integrante 2
//    GOVEA Mijares Jorge Luis - Integrante 3
// =====================================================================
using System.Data;
using SistemaPresatamos.Seguridad;

namespace SistemaPresatamos.Data
{
    public class CategoriaDAO
    {
        public DataTable ListarActivas()
        {
            Sesion.ExigirSesion();

            const string sql = "SELECT IdCategoria, Nombre FROM Categoria WHERE Activa = 1 ORDER BY Nombre;";
            return Db.Tabla(sql);
        }
    }
}
