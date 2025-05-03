using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ISdoCtaCtePrvRepository
    {
        Task<IList<TSdoCtaCtePrv>> GetAll<TSdoCtaCtePrv>(IDbTransaction transaction = null);

        Task<TSdoCtaCtePrv> GetById<TSdoCtaCtePrv>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TSdoCtaCtePrv>(TSdoCtaCtePrv entity, IDbTransaction transaction = null);

        Task<bool> Update<TSdoCtaCtePrv>(TSdoCtaCtePrv entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
        Task<List<SdoCtaCtePrvItem>> GetItemsByIdSaldoCtaCtePrv(int idSaldoCtaCtePrv, IDbTransaction transaction = null);
    }
}