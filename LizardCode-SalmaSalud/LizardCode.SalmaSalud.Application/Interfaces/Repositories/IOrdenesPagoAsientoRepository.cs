using LizardCode.SalmaSalud.Domain.Entities;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IOrdenesPagoAsientoRepository
    {
        Task<bool> DeleteByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null);

        Task<OrdenPagoAsiento> GetByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null);

        Task<bool> Insert(OrdenPagoAsiento entity, IDbTransaction transaction = null);

    }
}