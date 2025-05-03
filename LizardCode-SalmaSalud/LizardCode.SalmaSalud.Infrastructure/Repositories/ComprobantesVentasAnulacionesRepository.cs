using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ComprobantesVentasAnulacionesRepository : IComprobantesVentasAnulacionesRepository
    {
        private readonly IDbContext _context;

        public ComprobantesVentasAnulacionesRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesVentasAnulaciones
                    WHERE IdComprobanteVenta = {idComprobanteVenta}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<ComprobanteVentaAnulacion> GetByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM ComprobantesVentasAnulaciones
                    WHERE
	                    IdComprobanteVenta = {idComprobanteVenta}"
                );

            return await builder.QuerySingleAsync<ComprobanteVentaAnulacion>(transaction);
        }

        public async Task<bool> Insert(ComprobanteVentaAnulacion entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO ComprobantesVentasAnulaciones
                    (
                        IdComprobanteVenta,
                        IdComprobanteVentaAnulado
                    )
                    VALUES
                    (
                        {entity.IdComprobanteVenta},
                        {entity.IdComprobanteVentaAnulado}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
}
