using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IBancosRepository
    {
        Task<IList<TBanco>> GetAll<TBanco>(IDbTransaction transaction = null);

        Task<TBanco> GetById<TBanco>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TBanco>(TBanco entity, IDbTransaction transaction = null);

        Task<bool> Update<TBanco>(TBanco entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<IList<Banco>> GetBancosByIdEmpresa(int idEmpresa);

        Task<bool> UpdateEsDefault(bool esDefault, IDbTransaction transaction = null);
    }
}