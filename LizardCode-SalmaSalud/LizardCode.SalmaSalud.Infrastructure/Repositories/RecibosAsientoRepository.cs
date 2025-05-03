using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class RecibosAsientoRepository : IRecibosAsientoRepository
    {
        private readonly IDbContext _context;

        public RecibosAsientoRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdRecibo(int idRecibo, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM RecibosAsientos
                    WHERE IdRecibo = {idRecibo}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<ReciboAsiento> GetByIdRecibo(int idRecibo, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM RecibosAsientos
                    WHERE
	                    IdRecibo = {idRecibo}"
                );

            return await builder.QuerySingleOrDefaultAsync<ReciboAsiento>(transaction);
        }

        public async Task<bool> Insert(ReciboAsiento entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO RecibosAsientos
                    (
                        IdRecibo,
                        IdAsiento
                    )
                    VALUES
                    (
                        {entity.IdRecibo},
                        {entity.IdAsiento}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
}
