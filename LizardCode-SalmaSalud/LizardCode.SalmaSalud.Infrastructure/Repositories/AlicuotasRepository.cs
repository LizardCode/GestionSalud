using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class AlicuotasRepository : BaseRepository, IAlicuotasRepository, IDataTablesCustomQuery
    {
        public AlicuotasRepository(IDbContext context) : base(context)
        {
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						a.*,
						ta.Descripcion AS TipoAlicuota
                    FROM Alicuotas a
					INNER JOIN TipoAlicuotas ta
						ON ta.IdTipoAlicuota = a.IdTipoAlicuota
                    WHERE a.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}
                ");

            return base.GetAllCustomQuery(query);
        }
    }
}
