using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IComprobantesComprasAFIPRepository
    {
        Task<IList<TComprobantesComprasAFIP>> GetAll<TComprobantesComprasAFIP>(IDbTransaction transaction = null);

        Task<TComprobantesComprasAFIP> GetById<TComprobantesComprasAFIP>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TComprobantesComprasAFIP>(TComprobantesComprasAFIP entity, IDbTransaction transaction = null);

        Task<bool> Update<TComprobantesComprasAFIP>(TComprobantesComprasAFIP entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

    }
}