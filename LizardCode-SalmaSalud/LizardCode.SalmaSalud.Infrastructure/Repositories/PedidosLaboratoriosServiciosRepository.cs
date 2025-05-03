using Dapper;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;


namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class PedidosLaboratoriosServiciosRepository : BaseRepository, IPedidosLaboratoriosServiciosRepository
    {
        public PedidosLaboratoriosServiciosRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<PedidoLaboratorioServicio>> GetAllByIdPedidoLaboratorio(long idPedidoLaboratorio, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    PLS.*
                    FROM PedidosLaboratoriosServicios PLS
                    WHERE
                        PLS.idPedidoLaboratorio = {idPedidoLaboratorio}");

            var results = await builder.QueryAsync<PedidoLaboratorioServicio>(transaction);

            return results.AsList();
        }
    }
}
