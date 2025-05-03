using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class PlanillaGastosComprobantesComprasRepository : IPlanillaGastosComprobantesComprasRepository
    {
        private readonly IDbContext _context;

        public PlanillaGastosComprobantesComprasRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM PlanillaGastosComprobantesCompras
                    WHERE IdPlanillaGastos = {idPlanillaGastos}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<List<PlanillaGastoComprobanteCompra>> GetByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM PlanillaGastosComprobantesCompras
                    WHERE
	                    IdPlanillaGastos = {idPlanillaGastos}"
                );

            var result = await builder.QueryAsync<PlanillaGastoComprobanteCompra>(transaction);

            return result.AsList();
        }

        public async Task<bool> Insert(PlanillaGastoComprobanteCompra entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO PlanillaGastosComprobantesCompras
                    (
                        IdPlanillaGastos,
                        IdComprobanteCompra
                    )
                    VALUES
                    (
                        {entity.IdPlanillaGastos},
                        {entity.IdComprobanteCompra}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
}
