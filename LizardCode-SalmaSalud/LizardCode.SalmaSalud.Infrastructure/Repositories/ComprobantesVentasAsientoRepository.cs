using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ComprobantesVentasAsientoRepository : IComprobantesVentasAsientoRepository
    {
        private readonly IDbContext _context;

        public ComprobantesVentasAsientoRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesVentasAsientos
                    WHERE IdComprobanteVenta = {idComprobanteVenta}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<ComprobanteVentaAsiento> GetByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM ComprobantesVentasAsientos
                    WHERE
	                    IdComprobanteVenta = {idComprobanteVenta}"
                );

            return await builder.QuerySingleAsync<ComprobanteVentaAsiento>(transaction);
        }

        public async Task<bool> Insert(ComprobanteVentaAsiento entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO ComprobantesVentasAsientos
                    (
                        IdComprobanteVenta,
                        IdAsiento
                    )
                    VALUES
                    (
                        {entity.IdComprobanteVenta},
                        {entity.IdAsiento}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
}
