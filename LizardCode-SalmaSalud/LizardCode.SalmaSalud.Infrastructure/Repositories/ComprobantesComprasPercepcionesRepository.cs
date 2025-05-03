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
    public class ComprobantesComprasPercepcionesRepository : IComprobantesComprasPercepcionesRepository
    {
        private readonly IDbContext _context;

        public ComprobantesComprasPercepcionesRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesComprasPercepciones
                    WHERE IdComprobanteCompra = {idComprobanteCompra}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> DeleteByIdComprobanteCompraAndCuenta(int idComprobanteCompra, int idCuentaContable, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesComprasPercepciones
                    WHERE IdComprobanteCompra = {idComprobanteCompra} AND IdCuentaContable = {idCuentaContable}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<ComprobanteCompraPercepcion>> GetAllByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ccp.*
                    FROM ComprobantesComprasPercepciones ccp
                    WHERE
                        ccp.IdComprobanteCompra = {idComprobanteCompra}");

            var results = await builder.QueryAsync<ComprobanteCompraPercepcion>(transaction);

            return results.AsList();
        }

        public async Task<ComprobanteCompraPercepcion> GetByIdComprobanteCompraAndCuenta(int idComprobanteCompra, int idCuentaContable, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM ComprobantesComprasPercepciones
                    WHERE
	                    IdComprobanteCompra = {idComprobanteCompra}
	                    AND IdCuentaContable = {idCuentaContable}"
                );

            return await builder.QuerySingleOrDefaultAsync<ComprobanteCompraPercepcion>(transaction);
        }

        public async Task<bool> Insert(ComprobanteCompraPercepcion entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO ComprobantesComprasPercepciones
                    (
                        IdComprobanteCompra,
                        IdCuentaContable,
                        Importe
                    )
                    VALUES
                    (
                        {entity.IdComprobanteCompra},
                        {entity.IdCuentaContable},
                        {entity.Importe}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> Update(ComprobanteCompraPercepcion entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE ComprobantesComprasPercepciones SET
	                    IdComprobanteCompra = {entity.IdComprobanteCompra},
	                    IdCuentaContable = {entity.IdCuentaContable},
                        Importe = {entity.Importe}
                     WHERE
	                    IdComprobanteCompra = {entity.IdComprobanteCompra} AND IdCuentaContable = {entity.IdCuentaContable}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> RemoveAllByIdSdoCtaCtePrv(int idSdoCtaCtePrv, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesComprasPercepciones
                    WHERE IdComprobanteCompra IN (
	                    SELECT IdComprobanteCompra
	                    FROM saldo_cta_cte_prv_comprobantes_compras
	                    WHERE IdSaldoCtaCtePrv = {idSdoCtaCtePrv}
                    )");

            var results = await builder.ExecuteAsync();

            return (results == 1);
        }
    }
}
