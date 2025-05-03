using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class TiposAsientosCuentasRepository : BaseRepository, ITiposAsientosCuentasRepository
    {
        public TiposAsientosCuentasRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT tac.* FROM TipoAsientosCuentas tac ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<bool> RemoveAllByIdTipoAsiento(int idTipoAsiento, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM TipoAsientosCuentas
                    WHERE IdTipoAsiento = {idTipoAsiento} ");

            var results = await builder.ExecuteAsync();

            return (results == 1);
        }
    }
}
