using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IFinanciadoresPrestacionesRepository
    {
        Task<IList<TFinanciadorPrestacion>> GetAll<TFinanciadorPrestacion>(IDbTransaction transaction = null);

        Task<TFinanciadorPrestacion> GetById<TFinanciadorPrestacion>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TFinanciadorPrestacion>(TFinanciadorPrestacion entity, IDbTransaction transaction = null);

        Task<bool> Update<TFinanciadorPrestacion>(TFinanciadorPrestacion entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdFinanciador(long idFinanciador, IDbTransaction transaction = null);

        Task<IList<FinanciadorPrestacion>> GetAllByIdFinanciador(long idFinanciador, IDbTransaction transaction = null);
        Task<bool> RemoveById(long idFinanciadorPrestacion, IDbTransaction transaction = null);
        Task<FinanciadorPrestacion> GetFinanciadorPrestacionById(long idFinanciadorPrestacion, IDbTransaction transaction = null);
        Task<IList<FinanciadorPrestacion>> GetAllByIdFinanciadorAndIdPlan(long idFinanciador, long idFinanciadorPlan, IDbTransaction transaction = null);
        DataTablesCustomQuery GetAllCustomQuery();
        Task<FinanciadorPrestacion> GetByCodigo(string codigo, IDbTransaction transaction = null);
    }
}