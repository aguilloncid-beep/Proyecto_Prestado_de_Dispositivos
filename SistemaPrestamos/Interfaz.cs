using System.Windows.Forms;

namespace SistemaPresatamos
{
    public interface IPanelCRUD
    {
        void EjecutarGuardar();
        void EjecutarBuscar(string id, ErrorProvider alerta);
        void EjecutarActualizar();
        void EjecutarEliminar(StatusStrip barraEstado);
    }
}
