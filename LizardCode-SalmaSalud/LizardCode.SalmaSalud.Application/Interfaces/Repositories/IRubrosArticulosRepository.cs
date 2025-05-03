using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IRubrosArticulosRepository
    {
        Task<IList<TRubroArticulo>> GetAll<TRubroArticulo>(IDbTransaction transaction = null);

        Task<TRubroArticulo> GetById<TRubroArticulo>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TRubroArticulo>(TRubroArticulo entity, IDbTransaction transaction = null);

        Task<bool> Update<TRubroArticulo>(TRubroArticulo entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<IList<RubroArticulo>> GetAllByIdEmpresa(int idEmpresa);
    }
}