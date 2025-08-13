using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class DiasRepository : BaseRepository, IDiasRepository
    {
        public DiasRepository(IDbContext context) : base(context)
        {
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT rh.*, e.descripcion as Especialidad
                                    FROM TipoDia rh
                                    INNER JOIN Especialidades e on (rh.idEspecialidad = e.IdEspecialidad)");

            return base.GetAllCustomQuery(query);
        }
    }
}