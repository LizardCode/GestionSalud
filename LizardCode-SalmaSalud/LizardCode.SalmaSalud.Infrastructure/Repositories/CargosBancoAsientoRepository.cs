using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class CargosBancoAsientoRepository : ICargosBancoAsientoRepository
    {
        private readonly IDbContext _context;

        public CargosBancoAsientoRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdCargoBanco(int idCargoBanco, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM CargosBancoAsientos
                    WHERE IdCargoBanco = {idCargoBanco}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<CargoBancoAsiento> GetByIdCargoBanco(int idCargoBanco, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM CargosBancoAsientos
                    WHERE
	                    IdCargoBanco = {idCargoBanco}"
                );

            return await builder.QuerySingleOrDefaultAsync<CargoBancoAsiento>(transaction);
        }

        public async Task<bool> Insert(CargoBancoAsiento entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO CargosBancoAsientos
                    (
                        IdCargoBanco,
                        IdAsiento
                    )
                    VALUES
                    (
                        {entity.IdCargoBanco},
                        {entity.IdAsiento}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
}
