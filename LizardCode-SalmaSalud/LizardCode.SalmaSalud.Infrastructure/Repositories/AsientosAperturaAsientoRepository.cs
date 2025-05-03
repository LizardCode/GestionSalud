using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class AsientosAperturaAsientoRepository : IAsientosAperturaAsientoRepository
    {
        private readonly IDbContext _context;

        public AsientosAperturaAsientoRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdAsientoApertura(int idAsientoAperturaCierre, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM AsientosAperturaCierreAsientos
                    WHERE IdAsientoAperturaCierre = {idAsientoAperturaCierre}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<AsientoAperturaCierreAsiento> GetByIdAsientoApertura(int idAsientoAperturaCierre, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM AsientosAperturaCierreAsientos
                    WHERE
	                    IdAsientoAperturaCierre = {idAsientoAperturaCierre}"
                );

            return await builder.QuerySingleAsync<AsientoAperturaCierreAsiento>(transaction);
        }

        public async Task<bool> Insert(AsientoAperturaCierreAsiento entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO AsientosAperturaCierreAsientos
                    (
                        IdAsientoAperturaCierre,
                        IdAsiento
                    )
                    VALUES
                    (
                        {entity.IdAsientoAperturaCierre},
                        {entity.IdAsiento}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
}
