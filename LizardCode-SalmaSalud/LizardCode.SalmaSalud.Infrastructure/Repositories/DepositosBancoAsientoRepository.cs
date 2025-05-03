using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class DepositosBancoAsientoRepository : IDepositosBancoAsientoRepository
    {
        private readonly IDbContext _context;

        public DepositosBancoAsientoRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdDepositoBanco(int idDepositoBanco, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM DepositosBancoAsientos
                    WHERE 
                        IdDepositoBanco = {idDepositoBanco}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<DepositoBancoAsiento> GetByIdDepositoBanco(int idDepositoBanco, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM DepositosBancoAsientos
                    WHERE
	                    IdDepositoBanco = {idDepositoBanco}"
                );

            return await builder.QuerySingleOrDefaultAsync<DepositoBancoAsiento>(transaction);
        }

        public async Task<bool> Insert(DepositoBancoAsiento entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO DepositosBancoAsientos
                    (
                        IdDepositoBanco,
                        IdAsiento
                    )
                    VALUES
                    (
                        {entity.IdDepositoBanco},
                        {entity.IdAsiento}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
}
