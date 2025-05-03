using Dapper.DataTables.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Domain.Entities;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IFinanciadoresRepository
    {
        Task<IList<TFinanciador>> GetAll<TFinanciador>(IDbTransaction transaction = null);

        Task<TFinanciador> GetById<TFinanciador>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TFinanciador>(TFinanciador entity, IDbTransaction transaction = null);

        Task<bool> Update<TFinanciador>(TFinanciador entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<bool> ValidarCUITExistente(string cuit, int? id, IDbTransaction transaction = null);

        Task<List<Financiador>> GetAllFinanciadoresLookup();

        Task<Financiador> GetFinanciadorByCUIT(string cuit, IDbTransaction transaction = null);
    }
}
