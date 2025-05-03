using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IAlicuotasRepository
    {
        Task<IList<TAlicuota>> GetAll<TAlicuota>(IDbTransaction transaction = null);

        Task<TAlicuota> GetById<TAlicuota>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TAlicuota>(TAlicuota entity, IDbTransaction transaction = null);

        Task<bool> Update<TAlicuota>(TAlicuota entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
    }
}