using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ComprobantesVentasTotalesRepository : IComprobantesVentasTotalesRepository
    {
        private readonly IDbContext _context;

        public ComprobantesVentasTotalesRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesVentasTotales
                    WHERE IdComprobanteVenta = {idComprobanteVenta}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> DeleteByIdComprobanteVentaAndAlicuota(int idComprobanteVenta, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesVentasTotales
                    WHERE IdComprobanteVenta = {idComprobanteVenta} AND Item = {item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<ComprobanteVentaTotales>> GetAllByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    cvt.*
                    FROM ComprobantesVentasTotales cvt
                    WHERE
                        cvt.IdComprobanteVenta = {idComprobanteVenta}");

            var results = await builder.QueryAsync<ComprobanteVentaTotales>(transaction);

            return results.AsList();
        }

        public async Task<ComprobanteVentaTotales> GetByIdComprobanteVentaAndAlicuota(int idComprobanteVenta, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM ComprobantesVentasTotales
                    WHERE
	                    IdComprobanteVenta = {idComprobanteVenta}
	                    AND Item = {item}"
                );

            return await builder.QuerySingleOrDefaultAsync<ComprobanteVentaTotales>(transaction);
        }

        public async Task<bool> Insert(ComprobanteVentaTotales entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO ComprobantesVentasTotales
                    (
                        IdComprobanteVenta,
                        Item,
                        Neto,
                        ImporteAlicuota,
                        Alicuota,
                        IdTipoAlicuota
                    )
                    VALUES
                    (
                        {entity.IdComprobanteVenta},
                        {entity.Item},
                        {entity.Neto},
                        {entity.ImporteAlicuota},
                        {entity.Alicuota},
                        {entity.IdTipoAlicuota}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> Update(ComprobanteVentaTotales entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE ComprobantesVentasTotales SET
	                    Neto = {entity.Neto},
	                    ImporteAlicuota = {entity.ImporteAlicuota},
                        Alicuota = {entity.Alicuota},
                        IdTipoAlicuota = {entity.IdTipoAlicuota}
                     WHERE
	                    IdComprobanteVenta = {entity.IdComprobanteVenta} AND Item = {entity.Item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
        public async Task<bool> RemoveAllByIdSdoCtaCteCli(int idSdoCtaCteCli, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesVentasTotales
                    WHERE IdComprobanteVenta IN (
	                    SELECT IdComprobanteVenta 
	                    FROM SaldoCtaCteCliComprobantesVentas
	                    WHERE IdSaldoCtaCteCli = {idSdoCtaCteCli}
                    )");

            var results = await builder.ExecuteAsync();

            return (results == 1);
        }
    }
}
