using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    internal class SdoCtaCtePrvComprobantesComprasRepository : BaseRepository, ISdoCtaCtePrvComprobantesComprasRepository
    {
        public SdoCtaCtePrvComprobantesComprasRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT spc.* FROM SaldoCtaCtePrvComprobantesCompras spc ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<bool> RemoveAllByIdSdoCtaCtePrv(int idSdoCtaCtePrv, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM SaldoCtaCtePrvComprobantesCompras
                    WHERE IdSaldoCtaCtePrv = {idSdoCtaCtePrv} ");

            var results = await builder.ExecuteAsync();

            return (results == 1);
        }
    }
}
