using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ISaldosInicioBancosRepository
    {
        Task<IList<TSaldosInicioBancos>> GetAll<TSaldosInicioBancos>(IDbTransaction transaction = null);

        Task<TSaldosInicioBancos> GetById<TSaldosInicioBancos>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TSaldosInicioBancos>(TSaldosInicioBancos entity, IDbTransaction transaction = null);

        Task<bool> Update<TSaldosInicioBancos>(TSaldosInicioBancos entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<Custom.SaldoInicioBanco> GetByIdCustom(int idSaldoInicioBanco, IDbTransaction transaction = null);

    }
}