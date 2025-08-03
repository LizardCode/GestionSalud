using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using Dapper.DataTables.Models;
using DapperQueryBuilder;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class EspecialidadesRepository : BaseRepository, IEspecialidadesRepository
    {
        public EspecialidadesRepository(IDbContext context) : base(context)
        {
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT E.* FROM Especialidades E ");

            return base.GetAllCustomQuery(query);
        }

    }
}
