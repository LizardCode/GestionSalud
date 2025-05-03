using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ComprobantesComprasAFIPRepository : BaseRepository, IComprobantesComprasAFIPRepository
    {
        public ComprobantesComprasAFIPRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT cca.* FROM ComprobantesComprasAFIP cca ");

            return base.GetAllCustomQuery(query);
        }

    }
}
