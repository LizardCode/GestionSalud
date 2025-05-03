using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class SaldosInicioBancosRepository : BaseRepository, ISaldosInicioBancosRepository
    {
        public SaldosInicioBancosRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT sib.* FROM SaldoInicioBancos sib");

            return base.GetAllCustomQuery(query);
        }

        public async Task<Custom.SaldoInicioBanco> GetByIdCustom(int idSaldoInicioBanco, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                                    sib.*
                                FROM SaldoInicioBancos sib")
                .Where($"sib.IdOrdenPago = {idSaldoInicioBanco}");

            return await builder.QuerySingleAsync<Custom.SaldoInicioBanco>(transaction);
        }
    }
}