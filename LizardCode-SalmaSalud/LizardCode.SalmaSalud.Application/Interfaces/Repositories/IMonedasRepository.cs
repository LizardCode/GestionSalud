using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IMonedasRepository
    {
        Task<IList<TMoneda>> GetAll<TMoneda>(IDbTransaction transaction = null);

        Task<TMoneda> GetById<TMoneda>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TMoneda>(TMoneda entity, IDbTransaction transaction = null);

        Task<bool> Update<TMoneda>(TMoneda entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
        DataTablesCustomQuery GetTiposDeCambio();
    }
}