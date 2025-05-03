using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class CondicionVentasComprasRepository : BaseRepository, ICondicionVentasComprasRepository, IDataTablesCustomQuery
    {
        public CondicionVentasComprasRepository(IDbContext context) : base(context)
        {
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						cvc.*,
						tc.Descripcion AS TipoCondicion
                    FROM CondicionVentasCompras cvc
					INNER JOIN TipoCondicion tc
						ON tc.IdTipoCondicion = cvc.IdTipoCondicion
                    WHERE cvc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}
                ");

            return base.GetAllCustomQuery(query);
        }
    }
}
