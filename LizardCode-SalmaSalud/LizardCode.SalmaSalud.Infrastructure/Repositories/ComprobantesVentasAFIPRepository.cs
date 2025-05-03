using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ComprobantesVentasAFIPRepository : BaseRepository, IComprobantesVentasAFIPRepository
    {
        public ComprobantesVentasAFIPRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT cva.* FROM ComprobantesVentasAFIP cva ");

            return base.GetAllCustomQuery(query);
        }

    }
}
