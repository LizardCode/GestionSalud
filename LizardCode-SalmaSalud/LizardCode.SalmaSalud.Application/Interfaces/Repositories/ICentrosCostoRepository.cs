using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICentrosCostoRepository
    {
        Task<IList<TCentroCosto>> GetAll<TCentroCosto>(IDbTransaction transaction = null);

        Task<TCentroCosto> GetById<TCentroCosto>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TCentroCosto>(TCentroCosto entity, IDbTransaction transaction = null);

        Task<bool> Update<TCentroCosto>(TCentroCosto entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<IList<CentroCosto>> GetAllByIdEmpresa(int idEmpresa);
    }
}