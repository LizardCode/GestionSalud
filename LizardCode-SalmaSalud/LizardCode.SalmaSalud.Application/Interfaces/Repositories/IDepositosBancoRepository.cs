using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IDepositosBancoRepository
    {
        Task<IList<TDepositosBanco>> GetAll<TDepositosBanco>(IDbTransaction transaction = null);

        Task<TDepositosBanco> GetById<TDepositosBanco>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TDepositosBanco>(TDepositosBanco entity, IDbTransaction transaction = null);

        Task<bool> Update<TDepositosBanco>(TDepositosBanco entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<List<dynamic>> GetDepositoByCheque(int idCheque);
    }
}