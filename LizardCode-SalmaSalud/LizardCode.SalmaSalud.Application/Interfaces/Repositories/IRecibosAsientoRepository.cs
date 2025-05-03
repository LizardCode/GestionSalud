using LizardCode.SalmaSalud.Domain.Entities;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IRecibosAsientoRepository
    {
        Task<bool> DeleteByIdRecibo(int idRecibo, IDbTransaction transaction = null);
        
        Task<ReciboAsiento> GetByIdRecibo(int idRecibo, IDbTransaction transaction = null);
        
        Task<bool> Insert(ReciboAsiento entity, IDbTransaction transaction = null);

    }
}