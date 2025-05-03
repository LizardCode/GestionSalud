using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IRubrosContablesRepository
    {
        Task<IList<TRubroContable>> GetAll<TRubroContable>(IDbTransaction transaction = null);

        Task<TRubroContable> GetById<TRubroContable>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TRubroContable>(TRubroContable entity, IDbTransaction transaction = null);

        Task<bool> Update<TRubroContable>(TRubroContable entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<IList<RubroContable>> GetRubrosContablesByIdEmpresa(int idEmpresa);

        Task<List<Domain.EntitiesCustom.Select2Custom>> GetRubrosContablesByIdEmpresaAndTerm(int idEmpresa, string term);

        Task<IList<RubroContable>> GetRubrosContablesByIdEmpresa(int idEmpresa, IDbTransaction transaction = null);
    }
}