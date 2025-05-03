using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ComprobantesComprasAsientoRepository : IComprobantesComprasAsientoRepository
    {
        private readonly IDbContext _context;

        public ComprobantesComprasAsientoRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesComprasAsientos
                    WHERE IdComprobanteCompra = {idComprobanteCompra}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<ComprobanteCompraAsiento> GetByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM ComprobantesComprasAsientos
                    WHERE
	                    IdComprobanteCompra = {idComprobanteCompra}"
                );

            return await builder.QuerySingleAsync<ComprobanteCompraAsiento>(transaction);
        }

        public async Task<bool> Insert(ComprobanteCompraAsiento entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO ComprobantesComprasAsientos
                    (
                        IdComprobanteCompra,
                        IdAsiento
                    )
                    VALUES
                    (
                        {entity.IdComprobanteCompra},
                        {entity.IdAsiento}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
}
