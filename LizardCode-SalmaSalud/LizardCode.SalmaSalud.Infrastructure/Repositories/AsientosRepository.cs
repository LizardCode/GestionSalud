using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class AsientosRepository : BaseRepository, IAsientosRepository
    {
        public AsientosRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection.QueryBuilder(sql);

            return base.GetAllCustomQuery(builder);
        }

        public async Task<Custom.Asiento> GetByIdCustom(int idAsiento, IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
                .QueryBuilder(sql)
                .Where($"a.IdAsiento = {idAsiento}");

            return await builder.QuerySingleAsync<Custom.Asiento>(transaction);
        }

        private FormattableString BuildCustomQuery() =>
            $@"SELECT
                a.*,
                ISNULL(ta.Descripcion, '') TipoAsiento
            FROM Asientos a
            LEFT JOIN TipoAsientos ta ON a.IdTipoAsiento = ta.IdTipoAsiento ";
    }
}
