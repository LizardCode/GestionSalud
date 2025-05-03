using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IAuditoriaLoginRepository
    {
        Task<IList<TAuditoriaLogin>> GetAll<TAuditoriaLogin>(IDbTransaction transaction = null);

        Task<long> Insert<TAuditoriaLogin>(TAuditoriaLogin entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery(int idEmpresa);
    }
}
