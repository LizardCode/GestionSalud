using LizardCode.SalmaSalud.Application.Models.Impresiones;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IImpresionesBusiness
    {
        Task<PDF> GenerarImpresionFactura(int idComprobante);

        Task<PDF> GenerarImpresionOrdenPago(int idOrdenPago);
        Task<PDF> GenerarImpresionReceta(int idEvolucionReceta, int idEvolucion);

        Task<PDF> GenerarImpresionOrden(int idEvolucionOrden, int idEvolucion);
        Task<PDF> GenerarImpresionRecibo(int idRecibo);

        Task<PDF> GenerarImpresionRetenciones(int idOrdenPago, int idTipoRetencion);
    }
}