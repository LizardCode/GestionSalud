using LizardCode.SalmaSalud.Domain.Entities;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IComprobantesVentasAnulacionesRepository
    {
        Task<bool> DeleteByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null);
        
        Task<ComprobanteVentaAnulacion> GetByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null);
        
        Task<bool> Insert(ComprobanteVentaAnulacion entity, IDbTransaction transaction = null);

    }
}