using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICargosBancoRepository
    {
        Task<IList<TCargoBanco>> GetAll<TCargoBanco>(IDbTransaction transaction = null);

        Task<TCargoBanco> GetById<TCargoBanco>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TCargoBanco>(TCargoBanco entity, IDbTransaction transaction = null);

        Task<bool> Update<TCargoBanco>(TCargoBanco entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<Custom.CargoBanco> GetByIdCustom(int idCargoBanco, IDbTransaction transaction = null);

    }
}