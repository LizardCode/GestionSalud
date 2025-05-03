using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class TiposTurnoRepository : BaseRepository, ITiposTurnoRepository
    {
        public TiposTurnoRepository(IDbContext context) : base(context)
        {

        }
        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection.QueryBuilder($@"SELECT tt.* FROM TipoTurnos tt");

            return base.GetAllCustomQuery(query);
        }
    }
}