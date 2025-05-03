using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ISaldosInicioBancosAnticiposRepository
    {
        Task<IList<TSaldosInicioBancosAnticipos>> GetAll<TSaldosInicioBancosAnticipos>(IDbTransaction transaction = null);

        Task<TSaldosInicioBancosAnticipos> GetById<TSaldosInicioBancosAnticipos>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TSaldosInicioBancosAnticipos>(TSaldosInicioBancosAnticipos entity, IDbTransaction transaction = null);

        Task<bool> Update<TSaldosInicioBancosAnticipos>(TSaldosInicioBancosAnticipos entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdSaldoInicioBanco(int idSaldoInicioBanco, IDbTransaction transaction = null);

        Task<IList<Custom.SaldoInicioBancoAnticipos>> GetAllByIdSaldoInicioBanco(int idSaldoInicioBanco, int idTipoSaldo, IDbTransaction transaction = null);
    }
}