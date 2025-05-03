using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ComprobantesVentasRepository : BaseRepository, IComprobantesVentasRepository
    {
        public ComprobantesVentasRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var builder = _context.Connection.QueryBuilder($@"
                    SELECT
                        cv.*,
                        cl.RazonSocial Cliente,
                        co.Descripcion Comprobante
                    FROM ComprobantesVentas cv
                    INNER JOIN Clientes cl ON cv.IdCliente = cl.IdCliente
                    INNER JOIN Comprobantes co ON cv.IdComprobante = co.IdComprobante
                    INNER JOIN Ejercicios ej ON cv.IdEjercicio = ej.IdEjercicio");

            return base.GetAllCustomQuery(builder);
        }

        public async Task<Custom.ComprobanteVenta> GetByIdCustom(int idComprobanteVenta, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        cv.*,
                        cl.RazonSocial Cliente,
                        co.Descripcion Comprobante
                    FROM ComprobantesVentas cv
                    INNER JOIN Clientes cl ON cv.IdCliente = cl.IdCliente
                    INNER JOIN Comprobantes co ON cv.IdComprobante = co.IdComprobante
                    INNER JOIN Ejercicios ej ON cv.IdEjercicio = ej.IdEjercicio
                    WHERE
                        cv.IdComprobanteVenta = {idComprobanteVenta}");

            return await builder.QuerySingleAsync<Custom.ComprobanteVenta>(transaction);
        }

        public async Task<int?> GetLastNumeroByComprobante(int idComprobante, int idSucursal, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                            MAX(cv.Numero) AS Numero
                        FROM ComprobantesVentas cv
                        WHERE
                            cv.IdComprobante = {idComprobante} AND 
                            cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                            cv.IdEmpresa = {idEmpresa} AND 
                            cv.IdSucursal = {idSucursal}");

            return await builder.QuerySingleOrDefaultAsync<int?>(transaction);
        }

        public async Task<Custom.ComprobanteVentaAFIP> GetComprobanteVentaAFIP(int idComprobanteVta, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        cv.*,
                        c.IdTipoDocuemnto DocTipo,
                        co.Codigo CbteTipo,
                        co.EsCredito,
                        co.EsMiPymes,
                        c.CUIT
                    FROM ComprobantesVentas cv
                        INNER JOIN Comprobantes co ON cv.IdComprobante = co.IdComprobante
                        INNER JOIN Clientes c ON cv.IdCliente = c.IdCliente
                        WHERE
                            cv.IdComprobanteVenta = {idComprobanteVta}
                ");

            var comprobanteAFIP = await builder.QueryFirstOrDefaultAsync<Custom.ComprobanteVentaAFIP>(transaction);

            builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        a.CodigoAFIP Id,
                        SUM(Importe) BaseImponible,
                        SUM(Impuestos) Importe
                    FROM ComprobantesVentasItems cvi
                    INNER JOIN Alicuotas a ON cvi.Alicuota = a.Valor
                        WHERE
                            cvi.IdComprobanteVenta = {idComprobanteVta}
                    GROUP BY a.CodigoAFIP
                ");

            comprobanteAFIP.ComprobanteVentaItemsAFIP = (await builder.QueryAsync<Custom.ComprobanteVentaItemAFIP>(transaction)).AsList();
            comprobanteAFIP.ComprobanteVentaCbtAsociadosAFIP = new List<Custom.ComprobanteVentaCbtAsociadoAFIP>();
            comprobanteAFIP.ComprobanteVentaOpcionalesAFIP = new List<Custom.ComprobanteVentaOpcionalAFIP>();

            return comprobanteAFIP;
        }

        public async Task<List<Custom.ComprobanteVentaSubdiario>> GetAllSubdiarioCustomQuery(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        co.Descripcion Comprobante,
                        cl.RazonSocial Cliente,
                        cl.CUIT,
                        ti.Descripcion TipoIVA,
                        cvt.Alicuota,
                        SUM(CASE WHEN cvt.Alicuota = 0 THEN 0 ELSE cvt.Neto END) AS Neto,
                        SUM(CASE WHEN cvt.Alicuota <> 0 THEN 0 ELSE cvt.Neto END) AS NoGravado,
                        SUM(cvt.ImporteAlicuota) ImporteAlicuota,
                        SUM(cvt.Neto + cvt.ImporteAlicuota) AS ImporteTotal
                    FROM ComprobantesVentas cv
                    INNER JOIN Clientes cl ON cv.IdCliente = cl.IdCliente
                    INNER JOIN Comprobantes co ON cv.IdComprobante = co.IdComprobante 
                    INNER JOIN TipoIVA ti ON cl.IdTipoIVA = ti.IdTipoIVA 
                    INNER JOIN ComprobantesVentasTotales cvt ON cv.IdComprobanteVenta = cvt.IdComprobanteVenta
                    /**where**/
                    GROUP BY cvt.IdComprobanteVenta, co.Descripcion, cl.RazonSocial, cl.CUIT, ti.Descripcion, cvt.Alicuota");

            builder.Where($"cv.IdEmpresa = {filters["IdEmpresa"]}");
            builder.Where($"cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"cvt.Neto <> {0}");

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"cv.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"cv.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("IdCliente"))
                builder.Where($"cv.IdCliente = {filters["IdCliente"]}");

            return (await builder.QueryAsync<Custom.ComprobanteVentaSubdiario>()).AsList();
        }

        public async Task<List<Custom.CitiVenta>> GetCITIVentas(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            //MIGRAR_A_MSSQL
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        cv.Fecha,
                        co.Codigo TipoComprobante,
                        cv.Sucursal PuntoVenta,
                        cv.Numero NroComprobanteDesde,
                        cv.Numero NroComprobanteHasta,
                        '80' CodDocumentoComprador,
                        REPLACE(cl.CUIT, '-', '') NroIdentificaiconComprador,
                        cl.RazonSocial ApellidoNombreComprador,
                        ROUND(cv.Total, 2) ImporteTotal,
                        '0' ImporteConceptos,
                        '0' ImportePercepcion,
                        '0' ImporteOperacioneExentas,
                        '0' PercepcionIVA,
                        '0' PercepcionIBrutos,
                        '0' PercepcionImpuestosMunicipales,
                        '0' ImporteImpuestosInternos,
                        cv.Moneda,
                        cv.Cotizacion TipoCambio,
                        COUNT(cvt.Alicuota) CantidadAlicuotas,
                        '0' CodigoOperacion,
                        '0' OtrosTributos,
                        ISNULL(cv.FechaVto, cv.Fecha) VtoPago
                    FROM ComprobantesVentas cv
                    INNER JOIN Clientes cl ON cv.IdCliente = cl.IdCliente
                    INNER JOIN Comprobantes co ON cv.IdComprobante = co.IdComprobante 
                    INNER JOIN TipoIVA ti ON cl.IdTipoIVA = ti.IdTipoIVA 
                    INNER JOIN ComprobantesVentasTotales cvt ON cv.IdComprobanteVenta = cvt.IdComprobanteVenta
                    /**where**/
                    GROUP BY cvt.IdComprobanteVenta");

            builder.Where($"cv.IdEmpresa = {idEmpresa}");
            builder.Where($"cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"cvt.Neto <> {0}");
            builder.Where($"cv.Fecha >= {fechaDesde}");
            builder.Where($"cv.Fecha <= {fechaHasta}");

            return (await builder.QueryAsync<Custom.CitiVenta>()).AsList();
        }

        public async Task<List<Custom.CitiVentaAlicuota>> GetCITIVentasAli(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        co.Codigo TipoComprobante,
                        cv.Sucursal PuntoVenta,
                        cv.Numero NroComprobante,
                        cvt.Alicuota,
                        ROUND(SUM(cvt.Neto), 2) ImporteNetoGravado,
                        ROUND(SUM(cvt.ImporteAlicuota), 2) ImpuestoLiquidado
                    FROM ComprobantesVentas cv
                    INNER JOIN Comprobantes co ON cv.IdComprobante = co.IdComprobante 
                    INNER JOIN ComprobantesVentasTotales cvt ON cv.IdComprobanteVenta = cvt.IdComprobanteVenta
                    /**where**/
                    GROUP BY cvt.IdComprobanteVenta, co.Codigo, cv.Sucursal, cv.Numero, cvt.Alicuota");

            builder.Where($"cv.IdEmpresa = {idEmpresa}");
            builder.Where($"cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"cvt.Neto <> {0}");
            builder.Where($"cv.Fecha >= {fechaDesde}");
            builder.Where($"cv.Fecha <= {fechaHasta}");

            return (await builder.QueryAsync<Custom.CitiVentaAlicuota>()).AsList();
        }

        public async Task<Domain.Entities.Comprobante> GetComprobanteCreditoBySucNro(int idComprobanteAnular, string sucursalAnular, string numeroComprobanteAnular, int idEmpresa, IDbTransaction tran)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT DISTINCT
                        co.*
                    FROM ComprobantesVentas cv
                        INNER JOIN Clientes c ON cv.IdCliente = c.IdCliente
                        INNER JOIN ComprobantesTipoIVA cti ON cti.IdComprobante = cv.IdComprobante AND cti.IdTipoIVA = c.IdTipoIVA
                        INNER JOIN Comprobantes co ON cti.IdComprobanteCredito = co.IdComprobante
                        WHERE
                            cv.IdComprobante = {idComprobanteAnular} AND
                            cv.Sucursal = {sucursalAnular} AND
                            cv.Numero = {numeroComprobanteAnular} AND
                            cv.IdEmpresa = {idEmpresa}
                ");

            return await builder.QueryFirstOrDefaultAsync<Domain.Entities.Comprobante>();
        }

        public async Task<List<Custom.ResumenCtaCteCli>> GetResumenCtaCteCli(Dictionary<string, object> filters)
        {
            var fechaDesde = DateTime.MinValue;
            if (filters.ContainsKey("FechaDesde"))
                fechaDesde = DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null);

            var fechaHasta = DateTime.MaxValue;
            if (filters.ContainsKey("FechaHasta"))
                fechaHasta = DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null);

            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        cl.IdCliente,
                        cl.RazonSocial,
                        cl.NombreFantasia,
                        cl.CUIT,
                        spc.Saldo SaldoPendiente
                    FROM Clientes cl
                    INNER JOIN (
                        SELECT IdCliente, SUM(Total) AS Saldo FROM (
                            SELECT cv.IdCliente, 
                                CASE
                                    WHEN c.EsCredito = 1 THEN cv.Total * -1
                                    ELSE cv.Total
                                END AS Total
                                FROM ComprobantesVentas cv
                                INNER JOIN Comprobantes c ON cv.IdComprobante = c.IdComprobante
                                WHERE
                                    cv.IdEmpresa = {filters["IdEmpresa"]} AND
                                    cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                    cv.Fecha BETWEEN {fechaDesde} AND {fechaHasta}
                             UNION ALL
                             SELECT IdCliente, (r.Importe - CASE WHEN ra.Importe IS NULL THEN  0 ELSE NULL END) * -1 AS Total
                                FROM Recibos r
                                LEFT JOIN (SELECT IdAnticipo, SUM(Importe) Importe FROM RecibosAnticipos GROUP BY IdAnticipo) ra ON r.IdRecibo = ra.IdAnticipo
                                WHERE
                                    r.IdEstadoRegistro NOT IN ({(int)EstadoRecibo.Anulado}, {(int)EstadoRegistro.Eliminado}) AND
                                    r.IdEmpresa = {filters["IdEmpresa"]} AND
                                    r.Fecha BETWEEN {fechaDesde} AND {fechaHasta}
                            ) cvr
                        GROUP BY IdCliente
                    ) spc ON cl.IdCliente = spc.IdCliente
                    /**where**/
                ");

            if (filters.ContainsKey("IdCliente"))
                builder.Where($"cl.IdCliente = {filters["IdCliente"]}");

            if (filters.ContainsKey("SaldosEnCero") && int.Parse(filters["SaldosEnCero"].ToString()) == 0)
                builder.Where($"spc.Saldo <> 0 ");

            return (await builder.QueryAsync<Custom.ResumenCtaCteCli>()).AsList();
        }

        public async Task<List<Custom.ResumenCtaCteCliDetalle>> GetCtaCteDetalle(int idCliente, DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT cv.IdComprobanteVenta AS IdDocumento, cv.IdTipoComprobante AS IdTipo, cv.Fecha, cv.IdEjercicio, cv.IdComprobante, c.Descripcion Comprobante, cv.Sucursal, cv.Numero, c.EsCredito, cv.Total, '0' Saldo FROM ComprobantesVentas cv
				        INNER JOIN Comprobantes c ON cv.IdComprobante = c.IdComprobante
				        WHERE 
                            cv.IdCliente = {idCliente} AND
                            cv.IdEmpresa = {idEmpresa} AND
                            cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
					        cv.Fecha BETWEEN {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)} AND {(fechaHasta.HasValue ? fechaHasta.Value : DateTime.MaxValue)}
				        UNION ALL
				        SELECT r.IdRecibo AS IdDocumento, '99' AS IdTipo, r.Fecha, r.IdEjercicio, '0' IdComprobante, CASE WHEN r.IdEstadoRecibo <> {(int)EstadoRecibo.Finalizado} THEN 'RECIBO SIN IMPUTAR' ELSE 'RECIBO' END Comprobante, '' Sucursal, r.IdRecibo Numero, 0 EsCredito, (r.Total - CASE WHEN ra.Importe IS NULL THEN  0 ELSE NULL END) Total, '0' Saldo FROM Recibos r
                            LEFT JOIN (SELECT IdAnticipo, SUM(Importe) Importe FROM RecibosAnticipos GROUP BY IdAnticipo) ra ON r.IdRecibo = ra.IdAnticipo
        		        WHERE 
                            r.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
        			        r.IdCliente = {idCliente} AND
					        r.Fecha BETWEEN {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)} AND {(fechaHasta.HasValue ? fechaHasta.Value : DateTime.MaxValue)}
				        ORDER BY Fecha, Numero
                ");

            return (await builder.QueryAsync<Custom.ResumenCtaCteCliDetalle>()).AsList();
        }

        public async Task<double> GetCtaCteDetalleSdoInicio(int idCliente, DateTime? fechaDesde, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT CASE WHEN SUM(Total) IS NULL THEN 0 ELSE NULL END SaldoInicio FROM (
					    SELECT CASE
                                    WHEN c.EsCredito = 1 THEN cv.Total * -1
                                    ELSE cv.Total
                                END Total 
                            FROM ComprobantesVentas cv
                                INNER JOIN Comprobantes c ON cv.IdComprobante = c.IdComprobante    
    					    WHERE
    						    cv.IdCliente = {idCliente} AND 
                                cv.IdEmpresa = {idEmpresa} AND
                                cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                cv.Fecha < {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)}
					    UNION ALL
					    SELECT (r.Total - CASE WHEN ra.Importe IS NULL THEN 0 ELSE NULL END) * -1 Total FROM Recibos r
                            LEFT JOIN (SELECT IdAnticipo, SUM(Importe) Importe FROM RecibosAnticipos GROUP BY IdAnticipo) ra ON r.IdRecibo = ra.IdAnticipo
						    WHERE
						   	    r.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                r.IdCliente = {idCliente} AND 
                                r.IdEmpresa = {idEmpresa} AND 
                                r.Fecha < {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)}
					    ) sdoInicio");

            var sdoInicio = await builder.QuerySingleOrDefaultAsync<double?>();

            return sdoInicio ?? 0D;
        }

        public async Task<List<Custom.ResumenCtaCteCliPendiente>> GetCtasCobrar(int idCliente, DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
			var builder = _context.Connection
				.QueryBuilder($@"
                    SELECT cv.IdComprobanteVenta, cv.IdTipoComprobante, cv.Fecha, cv.FechaVto, cv.IdEjercicio, cv.IdComprobante, c.Descripcion AS Comprobante, c.EsCredito, cv.Sucursal, cv.Numero, ROUND(cv.Total - CASE WHEN r.Importe IS NULL THEN  0 ELSE NULL END, 2) AS Saldo, 0 SaldoAcumulado FROM ComprobantesVentas cv
				        INNER JOIN Comprobantes c ON cv.IdComprobante = c.IdComprobante
                        LEFT JOIN (
                            SELECT rc.IdComprobanteVenta, SUM(ABS(rc.Importe)) AS Importe FROM RecibosComprobantes rc
                                INNER JOIN Recibos r ON r.IdRecibo = rc.IdRecibo
                            WHERE
                                r.IdEmpresa = {idEmpresa} AND
                                r.IdCliente = {idCliente} AND
                                r.Fecha BETWEEN {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)} AND {(fechaHasta.HasValue ? fechaHasta.Value : DateTime.MaxValue)} AND
                                r.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}
                            GROUP BY rc.IdComprobanteVenta
                        ) r ON cv.idComprobanteVenta = r.idComprobanteVenta
				        WHERE
                            cv.IdEmpresa = {idEmpresa} AND
                            cv.IdCliente = {idCliente} AND
                            cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                            ROUND(cv.Total - CASE WHEN r.Importe IS NULL THEN  0 ELSE NULL END, 2) <> 0 AND
					        cv.Fecha BETWEEN {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)} AND {(fechaHasta.HasValue ? fechaHasta.Value : DateTime.MaxValue)}
				        ORDER BY cv.Fecha, cv.Numero
            ");

			return (await builder.QueryAsync<Custom.ResumenCtaCteCliPendiente>()).AsList();
		}

        public async Task<double> GetCtaCteCliPendienteSdoInicio(int idCliente, DateTime? fechaDesde, int idEmpresa)
        {
            //MIGRAR_A_MSSQL
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT ROUND(cv.Total - CASE WHEN r.Importe IS NULL THEN  0 ELSE NULL END, 2) AS SaldoInicio FROM ComprobantesVentas cv
				        INNER JOIN Comprobantes c ON cv.IdComprobante = c.IdComprobante
                        LEFT JOIN (
                            SELECT rc.IdComprobanteVenta, SUM(ABS(rc.Importe)) AS Importe FROM RecibosComprobantes rc
                                INNER JOIN Recibos r ON rc.IdRecibo = r.IdRecibo
                            WHERE
                                r.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                r.IdEmpresa = {idEmpresa} AND
                                r.IdCliente = {idCliente} AND
                                r.Fecha < {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)}
                            GROUP BY rc.IdComprobanteVenta
                        ) r ON cv.IdComprobanteVenta = r.IdComprobanteVenta
				        WHERE
                            cv.IdCliente = {idCliente} AND 
                            cv.IdEmpresa = {idEmpresa} AND
                            cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
					        cv.Fecha < {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)}");

            var sdoInicio = await builder.QuerySingleOrDefaultAsync<double?>();

            return sdoInicio ?? 0D;
        }

        public async Task<ComprobanteVenta> GetComprobanteBySucNro(int idComprobanteAnular, string sucursalAnular, string numeroComprobanteAnular, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        cv.*
                    FROM ComprobantesVentas cv
                    WHERE
                        cv.IdComprobante = {idComprobanteAnular} AND
                        cv.Sucursal = {sucursalAnular} AND
                        cv.Numero = {numeroComprobanteAnular} AND
                        cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                        cv.IdEmpresa = {idEmpresa}");

            return await builder.QuerySingleOrDefaultAsync<Custom.ComprobanteVenta>(transaction);
        }

        public async Task<double> GetResumenCtaCteCliDashboard(int idEmpresa)
        {
            //MIGRAR_A_MSSQL
            var builder = _context.Connection.QueryBuilder($@"
                    SELECT SUM(spc.Saldo) Importe
                    FROM Clientes cl
                    INNER JOIN (
                        SELECT IdCliente, Fecha, SUM(Total) AS Saldo FROM (
                            SELECT cv.IdCliente, cv.Fecha, 
                                CASE
                                    WHEN c.EsCredito = 1 THEN cv.Total * -1
                                    ELSE cv.Total
                                END AS Total
                                FROM ComprobantesVentas cv
                                INNER JOIN Comprobantes c ON cv.IdComprobante = c.IdComprobante
                                WHERE
                                    cv.IdEmpresa = {idEmpresa} AND
                                    cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}
                             UNION ALL
                             SELECT IdCliente, Fecha, (r.Importe - CASE WHEN ra.Importe IS NULL THEN  0 ELSE NULL END) * -1 AS Total
                                FROM Recibos r
                                LEFT JOIN (SELECT IdAnticipo, SUM(Importe) Importe FROM RecibosAnticipos GROUP BY IdAnticipo) ra ON r.IdRecibo = ra.IdAnticipo
                                WHERE
                                    r.IdEstadoRegistro NOT IN ({(int)EstadoRecibo.Anulado}, {(int)EstadoRegistro.Eliminado}) AND
                                    r.IdEmpresa = {idEmpresa} 
                            ) cvr
                        GROUP BY idCliente
                    ) spc ON cl.IdCliente = spc.IdCliente ");

            var result = await builder.ExecuteScalarAsync<double>();

            return result;
        }
        public async Task<bool> RemoveAllByIdSdoCtaCteCli(int idSdoCtaCteCli, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesVentas
                    WHERE IdComprobanteVenta IN (
	                    SELECT IdComprobanteVenta 
	                    FROM SaldoCtaCteCliComprobantesVentas
	                    WHERE IdSaldoCtaCteCli = {idSdoCtaCteCli}
                    )");

            var results = await builder.ExecuteAsync();

            return (results == 1);
        }

        public async Task<List<Custom.SubdiarioVentas>> GetSubdiarioVentas(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT cv.IdComprobanteVenta, cv.Fecha, tc.Descripcion comprobante, cv.Sucursal, cv.Numero, c.NombreFantasia Cliente, c.CUIT as cuit, ta.Descripcion tipoIVA, cv.Total
                                    FROM ComprobantesVentas cv
                                    INNER JOIN Comprobantes tc ON (tc.IdComprobante = cv.IdComprobante)
                                    INNER JOIN Clientes c ON (cv.IdCliente = c.IdCliente)
                                    INNER JOIN TipoIVA ta ON (c.IdTipoIVA = ta.IdTipoIVA)
                ");

            builder.Where($"cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

            if (filters.ContainsKey("IdEmpresa"))
                builder.Where($"cv.IdEmpresa = {filters["IdEmpresa"]}");

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"cv.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"cv.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            return (await builder.QueryAsync<Custom.SubdiarioVentas>()).AsList();
        }

        public async Task<List<Custom.SubdiarioVentasDetalle>> GetSubdiarioVentasDetalle(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT 
                    cv.IdComprobanteVenta, 
                    cv.Fecha, 
                    tc.Descripcion Comprobante, 
                    cv.Sucursal, 
                    cv.Numero, 
                    c.RazonSocial Cliente, 
                    c.CUIT, 
                    ta.Descripcion TipoIVA, 
                    cv.Total,
                    cc.CodigoCuenta, 
                    cc.Descripcion NombreCuenta, 
                    CASE WHEN ad.Debitos = 0 THEN ad.Creditos ELSE ad.Debitos END AS Importe
                FROM ComprobantesVentas cv
                    INNER JOIN Comprobantes tc ON (tc.IdComprobante = cv.IdComprobante)
                    INNER JOIN Clientes c ON (cv.IdCliente = c.IdCliente)
                    INNER JOIN TipoIVA ta ON (c.IdTipoIVA = ta.IdTipoIVA)
                    INNER JOIN ComprobantesVentasAsientos cva ON (cva.IdComprobanteVenta = cv.IdComprobanteVenta)
                    INNER JOIN Asientos a ON (a.IdAsiento = cva.IdAsiento)
                    INNER JOIN AsientosDetalle ad ON (a.IdAsiento = ad.IdAsiento)
                    INNER JOIN CuentasContables cc ON (cc.IdCuentaContable = ad.IdCuentaContable)
                ");

            builder.Where($"cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

            if (filters.ContainsKey("IdEmpresa"))
                builder.Where($"cv.IdEmpresa = {filters["IdEmpresa"]}");

            if (filters.ContainsKey("IdComprobanteVenta"))
                builder.Where($"cv.IdComprobanteVenta = {filters["IdComprobanteVenta"]}");

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"cv.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"cv.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            return (await builder.QueryAsync<Custom.SubdiarioVentasDetalle>()).AsList();
        }

        public async Task<DateTime?> GetMinFechaComprobanteVenta(int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                                    MIN(Fecha) Fecha
                                FROM ComprobantesVentas cv
                                WHERE
                                    cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                    cv.IdEmpresa = {idEmpresa}");

            return await builder.QuerySingleOrDefaultAsync<DateTime?>();
        }

        public async Task<DateTime?> GetMaxFechaComprobanteVenta(int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                                    MAX(Fecha) Fecha
                                FROM ComprobantesVentas cv
                                WHERE
                                    cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                    cv.IdEmpresa = {idEmpresa}");

            return await builder.QuerySingleOrDefaultAsync<DateTime?>();
        }

        public async Task<List<Custom.DetalleAlicuota>> GetDetalleAlicuotas(DateTime? fechaDesde, DateTime? fechaHasta, int? idCliente, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT 
                    a.Descripcion Alicuota,
                    SUM(CASE WHEN cvt.Alicuota <> 0 THEN cvt.Neto ELSE 0 END) NetoGravado, 
                    SUM(CASE WHEN cvt.Alicuota = 0 THEN cvt.Neto ELSE 0 END) NoGravado, 
                    SUM(cvt.ImporteAlicuota) ImporteAlicuota, 
                    SUM(cvt.Neto + cvt.ImporteAlicuota) Total
                FROM ComprobantesVentasTotales cvt
                    INNER JOIN Alicuotas a ON cvt.Alicuota = a.Valor
                    INNER JOIN ComprobantesVentas cv ON cv.IdComprobanteVenta = cvt.IdComprobanteVenta
                /**where**/
                GROUP BY cvt.Alicuota, a.Descripcion
                ");

            builder.Where($"cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"cv.IdEmpresa = {idEmpresa}");

            if (idCliente.HasValue)
                builder.Where($"cv.IdCliente = {idCliente}");

            if (fechaDesde.HasValue)
                builder.Where($"cv.Fecha >= {fechaDesde}");

            if (fechaHasta.HasValue)
                builder.Where($"cv.Fecha <= {fechaHasta}");

            return (await builder.QueryAsync<Custom.DetalleAlicuota>()).AsList();
        }

        public async Task<List<Custom.SubdiarioImputaciones>> GetSubdiarioImputaciones(DateTime? fechaDesde, DateTime? fechaHasta, int? idCliente, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT 
                    cc.CodigoCuenta, 
                    cc.Descripcion NombreCuenta, 
                    SUM(ad.Debitos) AS Debitos,
                    SUM(ad.Creditos) AS Creditos
                FROM ComprobantesVentas cv
                    INNER JOIN ComprobantesVentasAsientos cva ON (cva.IdComprobanteVenta = cv.IdComprobanteVenta)
                    INNER JOIN Asientos a ON (a.IdAsiento = cva.IdAsiento)
                    INNER JOIN AsientosDetalle ad ON (a.IdAsiento = ad.IdAsiento)
                    INNER JOIN CuentasContables cc ON (cc.IdCuentaContable = ad.IdCuentaContable)
                    /**where**/
                    GROUP BY ad.IdCuentaContable, cc.CodigoCuenta, cc.Descripcion
                ");

            builder.Where($"cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"cv.IdEmpresa = {idEmpresa}");

            if (idCliente.HasValue)
                builder.Where($"cv.IdCliente = {idCliente}");

            if (fechaDesde.HasValue)
                builder.Where($"cv.Fecha >= {fechaDesde}");

            if (fechaHasta.HasValue)
                builder.Where($"cv.Fecha <= {fechaHasta}");

            return (await builder.QueryAsync<Custom.SubdiarioImputaciones>()).AsList();
        }

		public async Task<List<Custom.ResumenCtaCteCliPendienteGeneral>> GetDetalleGeneralCtasCobrar(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
		{
			var builder = _context.Connection
				.QueryBuilder($@"
                    SELECT cl.RazonSocial Cliente, cl.CUIT, cv.Fecha, cv.FechaVto, cv.IdComprobante, c.Descripcion AS Comprobante, c.EsCredito, cv.Sucursal, cv.Numero, ROUND(cv.Total - CASE WHEN r.Importe IS NULL THEN  0 ELSE NULL END, 2) AS Saldo, 0 SaldoAcumulado FROM ComprobantesVentas cv
				        INNER JOIN Comprobantes c ON cv.IdComprobante = c.IdComprobante
                        INNER JOIN Clientes cl ON cv.IdCliente = cl.IdCliente
                        LEFT JOIN (
                            SELECT rc.IdComprobanteVenta, SUM(ABS(rc.Importe)) AS Importe FROM RecibosComprobantes rc
                                INNER JOIN Recibos r ON r.IdRecibo = rc.IdRecibo
                            WHERE
                                r.IdEmpresa = {idEmpresa} AND
                                r.Fecha BETWEEN {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)} AND {(fechaHasta.HasValue ? fechaHasta.Value : DateTime.MaxValue)} AND
                                r.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}
                            GROUP BY rc.IdComprobanteVenta
                        ) r ON cv.idComprobanteVenta = r.idComprobanteVenta
				        WHERE
                            cv.IdEmpresa = {idEmpresa} AND
                            cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                            ROUND(cv.Total - CASE WHEN r.Importe IS NULL THEN  0 ELSE NULL END, 2) <> 0 AND
					        cv.Fecha BETWEEN {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)} AND {(fechaHasta.HasValue ? fechaHasta.Value : DateTime.MaxValue)}
				        ORDER BY cv.Fecha, cv.Numero
            ");

			return (await builder.QueryAsync<Custom.ResumenCtaCteCliPendienteGeneral>()).AsList();
		}
	}
}
