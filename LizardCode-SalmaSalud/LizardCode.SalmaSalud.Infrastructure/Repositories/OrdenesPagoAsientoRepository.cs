using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class OrdenesPagoAsientoRepository : IOrdenesPagoAsientoRepository
    {
        private readonly IDbContext _context;

        public OrdenesPagoAsientoRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM OrdenesPagoAsientos
                    WHERE IdOrdenPago = {idOrdenPago}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<OrdenPagoAsiento> GetByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM OrdenesPagoAsientos
                    WHERE
	                    IdOrdenPago = {idOrdenPago}"
                );

            return await builder.QuerySingleOrDefaultAsync<OrdenPagoAsiento>(transaction);
        }

        public async Task<bool> Insert(OrdenPagoAsiento entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO OrdenesPagoAsientos
                    (
                        IdOrdenPago,
                        IdAsiento
                    )
                    VALUES
                    (
                        {entity.IdOrdenPago},
                        {entity.IdAsiento}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
}
