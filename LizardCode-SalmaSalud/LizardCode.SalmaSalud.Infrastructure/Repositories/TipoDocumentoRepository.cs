using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class TipoDocumentoRepository : BaseRepository, ITipoDocumentoRepository
    {
        public TipoDocumentoRepository(IDbContext context) : base(context)
        {

        }
        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection.QueryBuilder($@"SELECT td.* FROM TipoDocumentos td");

            return base.GetAllCustomQuery(query);
        }
    }
}
