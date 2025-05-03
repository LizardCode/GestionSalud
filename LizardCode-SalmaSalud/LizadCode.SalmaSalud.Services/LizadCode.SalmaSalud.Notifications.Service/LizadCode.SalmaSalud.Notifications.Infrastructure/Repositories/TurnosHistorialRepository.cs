using Dapper.DataTables.Models;
using DapperQueryBuilder;
using Dawa.Framework.Application.Interfaces.Context;
using LizadCode.SalmaSalud.Notifications.Application.Interfaces.Repositories;

namespace LizadCode.SalmaSalud.Notifications.Infrastructure.Repositories
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
