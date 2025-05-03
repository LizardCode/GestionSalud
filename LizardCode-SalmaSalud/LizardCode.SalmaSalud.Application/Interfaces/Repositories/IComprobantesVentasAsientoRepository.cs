using LizardCode.SalmaSalud.Domain.Entities;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IComprobantesVentasAsientoRepository
    {
        Task<bool> DeleteByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null);
        
        Task<ComprobanteVentaAsiento> GetByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null);
        
        Task<bool> Insert(ComprobanteVentaAsiento entity, IDbTransaction transaction = null);

    }
}