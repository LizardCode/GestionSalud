using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ISaldosInicioBancosChequesRepository
    {
        Task<IList<TSaldosInicioBancosCheques>> GetAll<TSaldosInicioBancosCheques>(IDbTransaction transaction = null);

        Task<TSaldosInicioBancosCheques> GetById<TSaldosInicioBancosCheques>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TSaldosInicioBancosCheques>(TSaldosInicioBancosCheques entity, IDbTransaction transaction = null);

        Task<bool> Update<TSaldosInicioBancosCheques>(TSaldosInicioBancosCheques entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdSaldoInicioBanco(int idSaldoInicioBanco, IDbTransaction transaction = null);

        Task<List<Custom.SaldoInicioBancoCheques>> GetAllByIdSaldoInicioBanco(int idSaldoInicioBanco, IDbTransaction transaction = null);

    }
}