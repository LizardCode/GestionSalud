using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ComprobantesComprasTotalesRepository : IComprobantesComprasTotalesRepository
    {
        private readonly IDbContext _context;

        public ComprobantesComprasTotalesRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesComprasTotales
                    WHERE IdComprobanteCompra = {idComprobanteCompra}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> DeleteByIdComprobanteCompraAndAlicuota(int idComprobanteCompra, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesComprasTotales
                    WHERE IdComprobanteVenta = {idComprobanteCompra} AND Item = {item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<ComprobanteCompraTotales>> GetAllByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    cct.*
                    FROM ComprobantesComprasTotales cct
                    WHERE
                        cct.IdComprobanteVenta = {idComprobanteCompra}");

            var results = await builder.QueryAsync<ComprobanteCompraTotales>(transaction);

            return results.AsList();
        }

        public async Task<ComprobanteCompraTotales> GetByIdComprobanteCompraAndAlicuota(int idComprobanteCompra, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM ComprobantesComprasTotales
                    WHERE
	                    IdComprobanteCompra = {idComprobanteCompra}
	                    AND Item = {item}"
                );

            return await builder.QuerySingleOrDefaultAsync<ComprobanteCompraTotales>(transaction);
        }

        public async Task<List<Domain.EntitiesCustom.ComprobanteCompraTotales>> GetTotalesByFechaMonotributo(DateTime fecha, int canMeses, int idEmpresa, int idProveedor, IDbTransaction transaction = null)
        {
            //MIGRAR_A_MSSQL
            var fechaDesde = (new DateTime(fecha.Year, fecha.Month, 1)).AddMonths(-canMeses);
            var fechaHasta = fecha.AddMonths(1).AddDays(-1);

            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT cc.IdComprobanteCompra, cc.IdComprobante, c.EsCredito, SUM(cct.neto) * IF(op.MonedaPago = {Monedas.MonedaLocal}, 1, op.Cotizacion) AS Neto, SUM(cct.importeAlicuota) * IF(op.MonedaPago = {Monedas.MonedaLocal}, 1, op.Cotizacion) AS Impuestos, op.ImportePagoTotal FROM ComprobantesComprasTotales cct
                        INNER JOIN ComprobantesCompras cc ON cc.IdComprobanteCompra = cct.IdComprobanteCompra
                        INNER JOIN Comprobantes c ON c.IdComprobante = cct.IdComprobante
                        INNER JOIN OrdenesPagoComprobantes opc ON cc.IdComprobanteCompra = opc.IdComprobanteCompra
                        INNER JOIN OrdenesPago op ON op.IdOrdenPago = opc.IdOrdenPago
                        WHERE
                            cc.Fecha BETWEEN {fechaDesde} AND {fechaHasta} AND
                            cc.IdProveedor = {idProveedor} AND
                            cc.IdEmpresa = {idEmpresa}
                        GROUP BY cc.IdComprobanteCompra");

            var results = await builder.QueryAsync<Domain.EntitiesCustom.ComprobanteCompraTotales>(transaction);

            return results.AsList();
        }

        public async Task<List<Domain.EntitiesCustom.ComprobanteCompraTotales>> GetTotalesByFechaPriUlt(DateTime fecha, int idEmpresa, int idProveedor, IDbTransaction transaction = null)
        {
            //MIGRAR_A_MSSQL
            var fechaDesde = new DateTime(fecha.Year, fecha.Month, 1);
            var fechaHasta = fechaDesde.AddMonths(1).AddDays(-1);

            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT cc.IdComprobanteCompra, cc.IdComprobante, c.EsCredito, SUM(cct.neto) * IF(op.MonedaPago = {Monedas.MonedaLocal}, 1, op.Cotizacion) AS Neto, SUM(cct.importeAlicuota) * IF(op.MonedaPago = {Monedas.MonedaLocal}, 1, op.Cotizacion) AS Impuestos, op.ImportePagoTotal FROM comprobantes_compras_totales cct
                        INNER JOIN ComprobantesCompras cc ON cc.IdComprobanteCompra = cct.IdComprobanteCompra
                        INNER JOIN Comprobantes c ON c.IdComprobante = cc.IdComprobante
                        INNER JOIN (
                            SELECT opc.IdComprobanteCompra, op.MonedaPago, op.Cotizacion, SUM(opc.Importe * IF(op.MonedaPago = {Monedas.MonedaLocal}, 1, opc.Cotizacion)) AS ImportePagoTotal FROM ordenes_pago_comprobantes opc
                            INNER JOIN OrdenesPago op ON opc.IdOrdenPago = op.IdOrdenPago
                            INNER JOIN ComprobantesCompras cc ON opc.IdComprobanteCompra = cc.IdComprobanteCompra
                            WHERE
                                op.Fecha BETWEEN {fechaDesde} AND {fechaHasta} AND
                                op.IdEmpresa = {idEmpresa} AND
                                op.IdProveedor = {idProveedor}
                            GROUP BY opc.IdComprobanteCompra, op.MonedaPago, opc.Cotizacion
                        ) AS op ON op.IdComprobanteCompra = cc.IdComprobanteCompra
                        GROUP BY cc.IdComprobanteCompra");

            var results = await builder.QueryAsync<Domain.EntitiesCustom.ComprobanteCompraTotales>(transaction);

            return results.AsList();
        }

        public async Task<List<Domain.EntitiesCustom.ComprobanteCompraTotales>> GetTotalesByIdOrdenPago(int idEmpresa, int idOrdenPago, IDbTransaction transaction = null)
        {
            //MIGRAR_A_MSSQL
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT cc.IdComprobanteCompra, cc.IdComprobante, c.EsCredito, SUM(cct.neto) * IF(op.MonedaPago = {Monedas.MonedaLocal}, 1, op.Cotizacion) AS Neto, SUM(cct.importeAlicuota) * IF(op.MonedaPago = {Monedas.MonedaLocal}, 1, op.Cotizacion) AS Impuestos, opc.Importe * IF(op.MonedaPago = {Monedas.MonedaLocal}, 1, op.Cotizacion) AS ImportePagoTotal FROM comprobantes_compras_totales cct
                        INNER JOIN ComprobantesCompras cc ON cc.IdComprobanteCompra = cct.IdComprobanteCompra
                        INNER JOIN Comprobantes c ON c.IdComprobante = cc.IdComprobante
                        INNER JOIN OrdenesPagoComprobantes opc ON cc.IdComprobanteCompra = opc.IdComprobanteCompra
                        INNER JOIN OrdenesPago op ON op.IdOrdenPago = opc.IdOrdenPago
                        WHERE
                            opc.IdOrdenPago = {idOrdenPago} AND
                            cc.IdEmpresa = {idEmpresa}
                        GROUP BY cc.IdComprobanteCompra");

            var results = await builder.QueryAsync<Domain.EntitiesCustom.ComprobanteCompraTotales>(transaction);

            return results.AsList();
        }

        public async Task<bool> Insert(ComprobanteCompraTotales entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO ComprobantesComprasTotales
                    (
                        IdComprobanteCompra,
                        Item,
                        Neto,
                        ImporteAlicuota,
                        Alicuota
                    )
                    VALUES
                    (
                        {entity.IdComprobanteCompra},
                        {entity.Item},
                        {entity.Neto},
                        {entity.ImporteAlicuota},
                        {entity.Alicuota}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> Update(ComprobanteCompraTotales entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE ComprobantesComprasTotales SET
	                    Neto = {entity.Neto},
	                    ImporteAlicuota = {entity.ImporteAlicuota},
                        Alicuota = {entity.Alicuota}
                     WHERE
	                    IdComprobanteCompra = {entity.IdComprobanteCompra} AND Item = {entity.Item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> RemoveAllByIdSdoCtaCtePrv(int idSdoCtaCtePrv, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesComprasTotales
                    WHERE IdComprobanteCompra IN (
	                    SELECT IdComprobanteCompra
	                    FROM SaldoCtaCtePrvComprobantesCompras
	                    WHERE IdSaldoCtaCtePrv = {idSdoCtaCtePrv}
                    )");

            var results = await builder.ExecuteAsync();

            return (results == 1);
        }
    }
}
