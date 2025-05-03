using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Appointments.Application.Interfaces.Repositories;

namespace LizardCode.SalmaSalud.Appointments.Infrastructure.Repositories
{
    public class TurnosHistorialRepository : BaseRepository, ITurnosHistorialRepository
    {
        public TurnosHistorialRepository(IDbContext context) : base(context)
        {

        }
        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection.QueryBuilder(sql);

            return base.GetAllCustomQuery(builder);
        }

        private FormattableString BuildCustomQuery() =>
                $@"SELECT th.*,
		                    u.Nombre as Usuario,
		                    et.Descripcion as Estado
                    FROM TurnosHistorial th
                    INNER JOIN Usuarios u ON (u.IdUsuario = th.IdUsuario)
                    INNER JOIN EstadoTurno et ON (th.IdEstadoTurno = et.IdEstadoTurno) ";
    }
}
