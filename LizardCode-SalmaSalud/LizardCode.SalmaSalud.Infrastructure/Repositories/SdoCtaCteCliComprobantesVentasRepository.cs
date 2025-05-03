using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    internal class SdoCtaCteCliComprobantesVentasRepository : BaseRepository, ISdoCtaCteCliComprobantesVentasRepository
    {
        public SdoCtaCteCliComprobantesVentasRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT scv.* FROM SaldoCtaCteCliComprobantesVentas scv ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<bool> RemoveAllByIdSdoCtaCteCli(int idSdoCtaCteCli, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM SaldoCtaCteCliComprobantesVentas
                    WHERE IdSaldoCtaCteCli = {idSdoCtaCteCli} ");

            var results = await builder.ExecuteAsync();

            return (results == 1);
        }
    }
}
