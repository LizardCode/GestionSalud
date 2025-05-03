using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class CargosBancoRepository : BaseRepository, ICargosBancoRepository
    {
        public CargosBancoRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        cb.*,
                        b.Descripcion AS Banco
                    FROM CargosBanco cb
                    INNER JOIN Bancos b ON cb.IdBanco = b.IdBanco");

            return base.GetAllCustomQuery(builder);
        }

        public async Task<Custom.CargoBanco> GetByIdCustom(int idCargoBanco, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        cb.*,
                        b.Descripcion AS Banco
                    FROM CargosBanco cb
                    INNER JOIN Bancos b ON cb.IdBanco = b.IdBanco
                    WHERE
                        cb.IdCargoBanco = {idCargoBanco}");

            return await builder.QuerySingleAsync<Custom.CargoBanco>(transaction);
        }
    }
}
