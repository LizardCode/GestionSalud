using LizardCode.SalmaSalud.Domain.Entities;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IComprobantesComprasAsientoRepository
    {
        Task<bool> DeleteByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null);
        
        Task<ComprobanteCompraAsiento> GetByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null);
        
        Task<bool> Insert(ComprobanteCompraAsiento entity, IDbTransaction transaction = null);

    }
}