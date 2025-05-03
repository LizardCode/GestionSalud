using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class PlanillaGastosAsientoRepository : IPlanillaGastosAsientoRepository
    {
        private readonly IDbContext _context;

        public PlanillaGastosAsientoRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM PlanillaGastosAsientos
                    WHERE IdPlanillaGastos = {idPlanillaGastos}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<PlanillaGastoAsiento> GetByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM PlanillaGastosAsientos
                    WHERE
	                    IdPlanillaGastos = {idPlanillaGastos}"
                );

            return await builder.QuerySingleAsync<PlanillaGastoAsiento>(transaction);
        }

        public async Task<bool> Insert(PlanillaGastoAsiento entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO PlanillaGastosAsientos
                    (
                        IdPlanillaGastos,
                        IdAsiento
                    )
                    VALUES
                    (
                        {entity.IdPlanillaGastos},
                        {entity.IdAsiento}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
}
