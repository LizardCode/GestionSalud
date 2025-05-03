using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ISdoCtaCteCliRepository
    {
        Task<IList<TSdoCtaCteCli>> GetAll<TSdoCtaCteCli>(IDbTransaction transaction = null);

        Task<TSdoCtaCteCli> GetById<TSdoCtaCteCli>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TSdoCtaCteCli>(TSdoCtaCteCli entity, IDbTransaction transaction = null);

        Task<bool> Update<TSdoCtaCteCli>(TSdoCtaCteCli entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
        Task<List<SdoCtaCteCliItem>> GetItemsByIdSaldoCtaCteCli(int idSaldoCtaCteCli, IDbTransaction transaction = null);
    }
}