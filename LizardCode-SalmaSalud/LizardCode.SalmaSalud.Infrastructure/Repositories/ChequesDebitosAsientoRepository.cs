using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ChequesDebitosAsientoRepository : IChequesDebitosAsientoRepository
    {
        private readonly IDbContext _context;

        public ChequesDebitosAsientoRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdCheque(int idCheque, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ChequesDebitosAsientos
                    WHERE IdCheque = {idCheque}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<ChequeDebitoAsiento> GetByIdCheque(int idCheque, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM ChequesDebitosAsientos
                    WHERE
	                    IdCheque = {idCheque}"
                );

            return await builder.QuerySingleOrDefaultAsync<ChequeDebitoAsiento>(transaction);
        }

        public async Task<bool> Insert(ChequeDebitoAsiento entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO ChequesDebitosAsientos
                    (
                        IdCheque,
                        IdAsiento
                    )
                    VALUES
                    (
                        {entity.IdCheque},
                        {entity.IdAsiento}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
}
