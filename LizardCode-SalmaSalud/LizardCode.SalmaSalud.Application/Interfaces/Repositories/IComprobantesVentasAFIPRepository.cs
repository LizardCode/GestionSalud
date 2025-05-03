using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IComprobantesVentasAFIPRepository
    {
        Task<IList<TComprobantesVentasAFIP>> GetAll<TComprobantesVentasAFIP>(IDbTransaction transaction = null);

        Task<TComprobantesVentasAFIP> GetById<TComprobantesVentasAFIP>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TComprobantesVentasAFIP>(TComprobantesVentasAFIP entity, IDbTransaction transaction = null);

        Task<bool> Update<TComprobantesVentasAFIP>(TComprobantesVentasAFIP entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

    }
}