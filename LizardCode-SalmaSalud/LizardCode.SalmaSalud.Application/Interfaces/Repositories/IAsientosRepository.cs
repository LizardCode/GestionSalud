using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IAsientosRepository
    {
        Task<IList<TAsiento>> GetAll<TAsiento>(IDbTransaction transaction = null);

        Task<TAsiento> GetById<TAsiento>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TAsiento>(TAsiento entity, IDbTransaction transaction = null);

        Task<bool> Update<TAsiento>(TAsiento entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<Custom.Asiento> GetByIdCustom(int idAsiento, IDbTransaction transaction = null);
    }
}