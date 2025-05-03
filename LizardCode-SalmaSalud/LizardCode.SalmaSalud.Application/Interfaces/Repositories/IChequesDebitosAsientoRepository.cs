using LizardCode.SalmaSalud.Domain.Entities;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IChequesDebitosAsientoRepository
    {
        Task<bool> DeleteByIdCheque(int idCheque, IDbTransaction transaction = null);
        
        Task<ChequeDebitoAsiento> GetByIdCheque(int idCheque, IDbTransaction transaction = null);
        
        Task<bool> Insert(ChequeDebitoAsiento entity, IDbTransaction transaction = null);

    }
}