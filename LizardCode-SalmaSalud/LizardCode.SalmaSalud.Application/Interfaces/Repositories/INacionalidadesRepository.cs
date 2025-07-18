using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface INacionalidadesRepository
    {
        Task<IList<TNacionalidad>> GetAll<TNacionalidad>(IDbTransaction transaction = null);

        Task<TNacionalidad> GetById<TNacionalidad>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TNacionalidad>(TNacionalidad entity, IDbTransaction transaction = null);

        Task<bool> Update<TNacionalidad>(TNacionalidad entity, IDbTransaction transaction = null);
    }
}
