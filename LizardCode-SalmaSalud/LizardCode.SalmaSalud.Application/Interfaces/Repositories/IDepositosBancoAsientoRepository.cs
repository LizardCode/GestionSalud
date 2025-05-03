using LizardCode.SalmaSalud.Domain.Entities;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IDepositosBancoAsientoRepository
    {
        Task<bool> DeleteByIdDepositoBanco(int idDepositoBanco, IDbTransaction transaction = null);
        
        Task<DepositoBancoAsiento> GetByIdDepositoBanco(int idDepositoBanco, IDbTransaction transaction = null);
        
        Task<bool> Insert(DepositoBancoAsiento entity, IDbTransaction transaction = null);

    }
}