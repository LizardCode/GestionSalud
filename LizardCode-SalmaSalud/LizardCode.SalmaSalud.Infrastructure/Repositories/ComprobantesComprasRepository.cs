using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.Framework.Helpers.Utilities;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ComprobantesComprasRepository : BaseRepository, IComprobantesComprasRepository
    {
        public ComprobantesComprasRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> DeleteByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesCompras
                    WHERE IdComprobanteCompra = {idComprobanteCompra}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var builder = _context.Connection.QueryBuilder($@"
                    SELECT
                        cc.*,
                        p.RazonSocial Proveedor,
                        co.Descripcion Comprobante
                    FROM ComprobantesCompras cc
                    INNER JOIN Proveedores p ON cc.IdProveedor = p.IdProveedor
                    INNER JOIN Comprobantes co ON cc.IdComprobante = co.IdComprobante
                    INNER JOIN Ejercicios ej ON cc.IdEjercicio = ej.IdEjercicio
                    ");

            return base.GetAllCustomQuery(builder);
        }

        public async Task<List<ComprobanteCompraSubdiario>> GetAllSubdiarioCustomQuery(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        cc.*,
                        pr.RazonSocial Proveedor,
                        pr.CUIT,
                        co.Descripcion Comprobante,
                        ti.descripcion TipoIVA,
                        cct.Alicuota,
                        CASE 
                            WHEN co.EsCredito = 1 THEN 
                                SUM(CASE WHEN cct.Alicuota = 0 THEN 0 ELSE cct.Neto END) * -1
                            ELSE
                                SUM(CASE WHEN cct.Alicuota = 0 THEN 0 ELSE cct.Neto END)
                        END AS Neto,
                        CASE 
                            WHEN co.EsCredito = 1 THEN 
                                SUM(CASE WHEN cct.Alicuota <> 0 THEN 0 ELSE cct.Neto END) * -1
                            ELSE
                                SUM(CASE WHEN cct.Alicuota <> 0 THEN 0 ELSE cct.Neto END)
                        END AS NoGravado,
                        CASE WHEN co.EsCredito = 1 THEN SUM(cct.ImporteAlicuota) * -1 ELSE SUM(cct.ImporteAlicuota) END AS ImporteAlicuota,
                        CASE WHEN ccpiva.perIVA IS NULL THEN  '0' ELSE NULL END PercepcionIVA, 
                        CASE WHEN ccpib.perIbr IS NULL THEN  '0' ELSE NULL END PercepcionIB,
                        CASE WHEN co.EsCredito = 1 THEN SUM(cct.Neto + cct.ImporteAlicuota + CASE WHEN ccpiva.perIVA IS NULL THEN  0 ELSE NULL END + CASE WHEN ccpib.perIbr IS NULL THEN  0 ELSE NULL END) * -1 ELSE SUM(cct.Neto + cct.ImporteAlicuota + CASE WHEN ccpiva.perIVA IS NULL THEN  0 ELSE NULL END + CASE WHEN ccpib.perIbr IS NULL THEN  0 ELSE NULL END) END AS ImporteTotal
                    FROM ComprobantesCompras cc
                    INNER JOIN Proveedores pr ON cc.IdProveedor = pr.IdProveedor
                    INNER JOIN Comprobantes co ON cc.IdComprobante = co.IdComprobante 
                    INNER JOIN TipoIVA ti ON pr.IdTipoIVA = ti.IdTipoIVA 
                    INNER JOIN ComprobantesComprasTotales cct ON cc.IdComprobanteCompra = cct.IdComprobanteCompra
                    LEFT JOIN (
                        SELECT IdComprobanteCompra, SUM(Importe) AS perIVA 
                            FROM ComprobantesComprasPercepciones 
                            WHERE 
                                IdCuentaContable IN (SELECT IdCuentaContable FROM CuentasContables WHERE IdCodigoObservacion = {CodigoObservacion.PERCEPCION_IVA}) 
                            GROUP BY IdComprobanteCompra) ccpiva ON cc.IdComprobanteCompra = ccpiva.IdComprobanteCompra
                    LEFT JOIN (
                        SELECT IdComprobanteCompra, SUM(importe) AS perIbr 
                            FROM ComprobantesComprasPercepciones 
                            WHERE 
                                IdCuentaContable IN (SELECT IdCuentaContable FROM CuentasContables WHERE IdCodigoObservacion = {CodigoObservacion.PERCEPCION_ING_BRUTOS}) 
                            GROUP BY IdComprobanteCompra) ccpib ON cc.IdComprobanteCompra = ccpib.IdComprobanteCompra
                    /**where**/
                    GROUP BY cct.IdComprobanteCompra, cc.IdComprobanteCompra, cc.IdComprobante, cc.Sucursal, cc.Numero, cc.IdEjercicio, cc.IdProveedor, cc.IdEmpresa, cc.Fecha, cc.FechaVto, cc.FechaReal, cc.Subtotal, cc.Total, cc.Percepciones, cc.IdUsuario, cc.FechaIngreso, cc.IdTipoComprobante, cc.Moneda, cc.Cotizacion, cc.IdEstadoRegistro, cc.IdEstadoAFIP, cc.CAE, cc.VencimientoCAE, cc.IdCentroCosto, cc.IdCondicion, pr.RazonSocial, pr.CUIT, co.Descripcion, ti.Descripcion, cct.Alicuota, co.EsCredito, ccpiva.PerIVA, ccpib.PerIbr");

            builder.Where($"cc.IdEmpresa = {filters["IdEmpresa"]}");
            builder.Where($"cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"cct.Neto <> {0}");

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"cc.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"cc.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("IdProveedor"))
                builder.Where($"cc.IdProveedor = {filters["IdProveedor"]}");


            return (await builder.QueryAsync<Custom.ComprobanteCompraSubdiario>()).AsList();
        }

        public async Task<Custom.ComprobanteCompra> GetByIdCustom(int idComprobanteCompra, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
                        cc.*,
                        p.RazonSocial Proveedor,
                        co.Descripcion Comprobante
                    FROM ComprobantesCompras cc
                    INNER JOIN Proveedores p ON cc.IdProveedor = p.IdProveedor
                    INNER JOIN Comprobantes co ON cc.IdComprobante = co.IdComprobante
                    WHERE
                        cc.IdComprobanteCompra = {idComprobanteCompra}");

            return await builder.QuerySingleAsync<Custom.ComprobanteCompra>(transaction);
        }

        public async Task<Custom.ComprobanteCompraManual> GetManualByIdCustom(int idComprobanteCompra, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
                        cc.*,
                        CONCAT(cc.Sucursal, '-', cc.Numero) NumeroComprobante,
                        p.RazonSocial Proveedor,
                        co.Descripcion Comprobante
                    FROM ComprobantesCompras cc
                    INNER JOIN Proveedores p ON cc.IdProveedor = p.IdProveedor
                    INNER JOIN Comprobantes co ON cc.IdComprobante = co.IdComprobante
                    WHERE
                        cc.IdComprobanteCompra = {idComprobanteCompra}");

            return await builder.QuerySingleAsync<Custom.ComprobanteCompraManual>(transaction);
        }

        public async Task<Custom.ComprobanteCompra> GetComprobanteBySucNro(int idComprobanteAnular, string sucursalAnular, string numeroComprobanteAnular, int idProveedor, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        cc.*
                    FROM ComprobantesCompras cc
                    WHERE
                        cc.IdComprobante = {idComprobanteAnular} AND
                        cc.Sucursal = {sucursalAnular} AND
                        cc.Numero = {numeroComprobanteAnular} AND
                        cc.IdProveedor = {idProveedor} AND
                        cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                        cc.IdEmpresa = {idEmpresa}");

            return await builder.QuerySingleOrDefaultAsync<Custom.ComprobanteCompra>(transaction);
        }

        public async Task<List<CitiCompra>> GetCITICompras(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        cc.Fecha,
                        co.Codigo TipoComprobante,
                        cc.Sucursal PuntoVenta,
                        cc.Numero NroComprobante,
                        '0' NroDespachoImportacion,
                        '80' CodDocumentoVendedor,
                        REPLACE(pr.CUIT, '-', '') NroIdentificacionVendedor,
                        pr.RazonSocial ApellidoNombreVendedor,
                        cc.Total ImporteTotal,
                        '0' ImporteConceptos,
                        SUM(
                            CASE
                                WHEN cct.Alicuota = 0 THEN cct.Neto
                                ELSE 0
                            END
                        ) ImporteOperacionesExentas,
                        CASE WHEN piva.Importe IS NULL THEN  0 ELSE NULL END PercepcionIVA,
                        '0' PercepcionImpuestosNacionales,
                        CASE WHEN pibr.Importe IS NULL THEN  0 ELSE NULL END PercepcionIBrutos,
                        '0' PercepcionImpuestosMunicipales,
                        '0' ImporteImpuestosInternos,
                        cc.Moneda,
                        cc.Cotizacion TipoCambio,
                        SUM(CASE WHEN cct.Alicuota = {0} THEN {0} ELSE {1} END) CantidadAlicuotas,
                        CASE
                            WHEN pr.IdTipoIVA = {TipoIVA.Monotributo} THEN 'A'
                            WHEN pr.IdTipoIVA = {TipoIVA.Exento} OR pr.IdTipoIVA = {TipoIVA.ExentoNoGravado} THEN 'E'
                            ELSE '0'
                        END CodigoOperacion,
                        SUM(cct.ImporteAlicuota) CreditoFiscal,
                        '0' OtrosTributos,
                        REPLACE(pr.CUIT, '-', '') CUITEmisorCorredor,
                        '' DenominacionEmisorCorredor,
                        '0' IVAComision
                    FROM ComprobantesCompras cc
                    INNER JOIN Proveedores pr ON cc.IdProveedor = pr.IdProveedor
                    INNER JOIN Comprobantes co ON cc.IdComprobante = co.IdComprobante 
                    INNER JOIN TipoIVA ti ON pr.IdTipoIVA = ti.IdTipoIVA 
                    INNER JOIN ComprobantesComprasTotales cct ON cc.IdComprobanteCompra = cct.IdComprobanteCompra
                    LEFT JOIN (
                      SELECT ccp.IdComprobanteCompra, SUM(ccp.Importe) Importe FROM comprobantes_compras_percepciones ccp
                        LEFT JOIN cuentas_contables cta ON ccp.IdCuentaContable = cta.IdCuentaContable
                        WHERE
                          cta.IdCodigoObservacion = {(int)CodigoObservacion.PERCEPCION_IVA} GROUP BY ccp.IdComprobanteCompra) piva ON cc.IdComprobanteCompra = piva.IdComprobanteCompra
                    LEFT JOIN (
                      SELECT ccp.IdComprobanteCompra, SUM(ccp.Importe) Importe FROM comprobantes_compras_percepciones ccp
                        LEFT JOIN cuentas_contables cta ON ccp.IdCuentaContable = cta.IdCuentaContable
                        WHERE
                          cta.IdCodigoObservacion = {(int)CodigoObservacion.PERCEPCION_ING_BRUTOS} GROUP BY ccp.IdComprobanteCompra) pibr ON cc.IdComprobanteCompra = pibr.IdComprobanteCompra
                    /**where**/
                    GROUP BY cct.IdComprobanteCompra, cc.Fecha, co.Codigo, cc.Sucursal, cc.Numero, pr.CUIT, pr.RazonSocial, pr.IdTipoIVA, cc.Total, piva.Importe, pibr.Importe, cc.Moneda, cc.Cotizacion
                    ORDER BY cct.IdComprobanteCompra");

            builder.Where($"cc.IdEmpresa = {idEmpresa}");
            builder.Where($"cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"cct.Neto <> {0}");
            builder.Where($"(cc.CAE <> {string.Empty} OR co.descripcion LIKE 'TICKET%')");
            builder.Where($"cc.Fecha >= {fechaDesde}");
            builder.Where($"cc.Fecha <= {fechaHasta}");

            return (await builder.QueryAsync<Custom.CitiCompra>()).AsList();
        }

        public async Task<List<CitiCompraAlicuota>> GetCITIComprasAli(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        co.Codigo TipoComprobante,
                        cc.Sucursal PuntoVenta,
                        cc.Numero NroComprobante,
                        '80' CodDocumentoVendedor,
                        REPLACE(pr.CUIT, '-', '') NroIdentificacionVendedor,
                        a.CodigoAFIP Alicuota,
                        SUM(cct.Neto) ImporteNetoGravado,
                        SUM(cct.ImporteAlicuota) ImpuestoLiquidado
                    FROM ComprobantesCompras cc
                    INNER JOIN Proveedores pr ON cc.IdProveedor = pr.IdProveedor 
                    INNER JOIN Comprobantes co ON cc.IdComprobante = co.IdComprobante 
                    INNER JOIN ComprobantesComprasTotales cct ON cc.IdComprobanteCompra = cct.IdComprobanteCompra
                    INNER JOIN Alicuotas a ON cct.Alicuota = a.Valor 
                    LEFT JOIN (
                      SELECT ccp.IdComprobanteCompra, SUM(ccp.Importe) Importe FROM comprobantes_compras_percepciones ccp
                        LEFT JOIN cuentas_contables cta ON ccp.IdCuentaContable = cta.IdCuentaContable
                        WHERE
                          cta.IdCodigoObservacion = {(int)CodigoObservacion.PERCEPCION_IVA} GROUP BY ccp.IdComprobanteCompra) piva ON cc.IdComprobanteCompra = piva.IdComprobanteCompra
                    LEFT JOIN (
                      SELECT ccp.IdComprobanteCompra, SUM(ccp.Importe) Importe FROM comprobantes_compras_percepciones ccp
                        LEFT JOIN cuentas_contables cta ON ccp.IdCuentaContable = cta.IdCuentaContable
                        WHERE
                          cta.IdCodigoObservacion = {(int)CodigoObservacion.PERCEPCION_ING_BRUTOS} GROUP BY ccp.IdComprobanteCompra) pibr ON cc.IdComprobanteCompra = pibr.IdComprobanteCompra
                    /**where**/
                    GROUP BY cct.IdComprobanteCompra, cct.Alicuota, co.Codigo, cc.Sucursal, cc.Numero, pr.CUIT, a.CodigoAFIP
                    ORDER BY cct.IdComprobanteCompra");

            builder.Where($"cc.IdEmpresa = {idEmpresa}");
            builder.Where($"cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"cct.Neto <> {0} AND cct.Alicuota <> {0}");
            builder.Where($"(cc.CAE <> {string.Empty} OR co.descripcion LIKE 'TICKET%')");
            builder.Where($"cc.Fecha >= {fechaDesde}");
            builder.Where($"cc.Fecha <= {fechaHasta}");
            builder.Where($"co.Letra NOT IN ({Letras.B.Description()}, {Letras.C.Description()})");

            var result = await builder.QueryAsync<CitiCompraAlicuota>();

            return result.AsList();
        }

        public async Task<Domain.Entities.Comprobante> GetComprobanteCreditoBySucNro(int idComprobanteAnular, string numeroComprobanteAnular, int idProveedor, int idEmpresa, IDbTransaction tran)
        {
            var sucursal = numeroComprobanteAnular.Substring(0, 5);
            var numero = numeroComprobanteAnular.Substring(6, 8);

            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT DISTINCT
                        co.*
                    FROM ComprobantesCompras cc
                        INNER JOIN Proveedores p ON cc.IdProveedor = p.IdProveedor
                        INNER JOIN ComprobantesTiposIVA cti ON cti.IdComprobante = cc.IdComprobante AND cti.IdTipoIVA = p.IdTipoIVA
                        INNER JOIN Comprobantes co ON cti.IdComprobanteCredito = co.IdComprobante
                        WHERE
                            cc.IdComprobante = {idComprobanteAnular} AND
                            cc.IdProveedor = {idProveedor} AND
                            cc.Sucursal = {sucursal} AND
                            cc.Numero = {numero} AND
                            cc.IdEmpresa = {idEmpresa}
                ");

            return await builder.QueryFirstOrDefaultAsync<Domain.Entities.Comprobante>();
        }

        public async Task<List<ResumenCtaCtePro>> GetResumenCtaCtePro(Dictionary<string, object> filters)
        {
            var fechaDesde = DateTime.MinValue;
            if (filters.ContainsKey("FechaDesde"))
                fechaDesde = DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null);

            var fechaHasta = DateTime.MaxValue;
            if (filters.ContainsKey("FechaHasta"))
                fechaHasta = DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null);

            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        pr.IdProveedor,
                        pr.RazonSocial,
                        pr.NombreFantasia,
                        pr.CUIT,
                        spp.Saldo SaldoPendiente
                    FROM Proveedores pr
                    INNER JOIN (
                        SELECT IdProveedor, SUM(Total) AS Saldo FROM (
                            SELECT cc.IdProveedor,
                                CASE
                                    WHEN c.EsCredito = 1 THEN cc.Total * -1
                                    ELSE cc.Total
                                END AS Total
                                FROM ComprobantesCompras cc
                                INNER JOIN Comprobantes c ON cc.IdComprobante = c.IdComprobante
                                WHERE
                                    cc.IdEmpresa = {filters["IdEmpresa"]} AND
                                    cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                    cc.Fecha BETWEEN {fechaDesde} AND {fechaHasta}
                             UNION ALL
                             SELECT IdProveedor, op.Total * -1 AS Total
                                FROM OrdenesPago op
                                WHERE
                                    op.IdEstadoRegistro <> ({(int)EstadoRegistro.Eliminado}) AND
                                    op.IdEmpresa = {filters["IdEmpresa"]} AND
                                    op.Fecha BETWEEN {fechaDesde} AND {fechaHasta}
                            ) ccr
                        GROUP BY IdProveedor
                    ) spp ON pr.IdProveedor = spp.IdProveedor
                    /**where**/
                ");

            if (filters.ContainsKey("IdProveedor"))
                builder.Where($"pr.IdProveedor = {filters["IdProveedor"]}");

			if (filters.ContainsKey("SaldosEnCero") && int.Parse(filters["SaldosEnCero"].ToString()) == 0)
				builder.Where($"ROUND(spp.Saldo) <> 0 ");

            return (await builder.QueryAsync<Custom.ResumenCtaCtePro>()).AsList();
        }

        public async Task<List<ResumenCtaCteProDetalle>> GetCtaCteDetalle(int idProveedor, DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                SELECT '0' AS IdDocumento, cc.Fecha, cc.IdComprobante, c.Descripcion AS Comprobante, cc.Sucursal, cc.Numero, c.EsCredito, cc.Total, '0' Saldo FROM ComprobantesCompras cc
				    INNER JOIN Comprobantes c ON cc.IdComprobante = c.IdComprobante
				    WHERE 
                        cc.IdProveedor = {idProveedor} AND
                        cc.IdEmpresa = {idEmpresa} AND
                        cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                        cc.Fecha BETWEEN {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)} AND {(fechaHasta.HasValue ? fechaHasta.Value : DateTime.MaxValue)}
				    UNION ALL
				    SELECT op.IdOrdenPago AS IdDocumento, op.Fecha, 0 AS IdComprobante, 'ORDEN DE PAGO' AS Comprobante, '' AS Sucursal, op.IdOrdenPago Numero, 0 EsCredito, op.Total, '0' Saldo FROM OrdenesPago op
        		    WHERE 
                        op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
        			    op.IdProveedor = {idProveedor} AND
                        op.Fecha BETWEEN {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)} AND {(fechaHasta.HasValue ? fechaHasta.Value : DateTime.MaxValue)}
				    ORDER BY Fecha, Numero
            ");

            return (await builder.QueryAsync<ResumenCtaCteProDetalle>()).AsList();
        }

        public async Task<double> GetCtaCteDetalleSdoInicio(int idProveedor, DateTime? fechaDesde, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT CASE WHEN SUM(Total) IS NULL THEN 0 ELSE NULL END AS SaldoInicio FROM (
						SELECT CASE
                                    WHEN c.EsCredito = 1THEN cc.Total * -1
                                    ELSE cc.Total
                                END AS Total 
                            FROM ComprobantesCompras cc
                                INNER JOIN Comprobantes c ON cc.IdComprobante = c.IdComprobante
    						WHERE
    							cc.IdProveedor = {idProveedor} AND 
                                cc.IdEmpresa = {idEmpresa} AND 
                                cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                cc.Fecha < {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)}
						UNION ALL
						SELECT op.Total * -1 AS Total FROM OrdenesPago op
						   	WHERE
                                op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
        			            op.IdProveedor = {idProveedor} AND
						   		op.Fecha < {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)}
						) sdoInicio");

            var sdoInicio = await builder.QuerySingleOrDefaultAsync<double?>();
            
            return sdoInicio ?? 0D;
        }

        public async Task<List<ResumenCtaCteProPendiente>> GetCtasPagar(int idProveedor, DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            //MIGRAR_A_MSSQL
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT cc.Fecha, cc.FechaVto, cc.IdComprobante, c.Descripcion AS Comprobante, c.EsCredito, cc.Sucursal, cc.Numero, ROUND(cc.Total - CASE WHEN op.Importe IS NULL THEN  0 ELSE NULL END, 2) AS Saldo FROM ComprobantesCompras cc
				        INNER JOIN Comprobantes c ON cc.IdComprobante = c.IdComprobante
                        LEFT JOIN (
                            SELECT opc.IdComprobanteCompra, SUM(ABS(opc.Importe)) AS Importe FROM OrdenesPagoComprobantes opc
                                INNER JOIN OrdenesPago op ON op.IdOrdenPago = opc.IdOrdenPago
                            WHERE
                                op.IdEmpresa = {idEmpresa} AND
                                op.IdProveedor = {idProveedor} AND
                                op.Fecha BETWEEN {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)} AND {(fechaHasta.HasValue ? fechaHasta.Value : DateTime.MaxValue)} AND
                                op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}
                            GROUP BY opc.IdComprobanteCompra
                        ) op ON cc.IdComprobanteCompra = op.IdComprobanteCompra
				        WHERE
                            cc.IdEmpresa = {idEmpresa} AND
                            cc.IdProveedor = {idProveedor} AND
                            cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
					        cc.Fecha BETWEEN {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)} AND {(fechaHasta.HasValue ? fechaHasta.Value : DateTime.MaxValue)} AND
                            ROUND(cc.Total - CASE WHEN op.Importe IS NULL THEN  0 ELSE NULL END, 2) <> 0
				        ORDER BY cc.Fecha, cc.Numero
            ");

            return (await builder.QueryAsync<Custom.ResumenCtaCteProPendiente>()).AsList();
        }

        public async Task<double> GetCtaCteProPendienteSdoInicio(int idProveedor, DateTime? fechaDesde, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT ROUND(cc.Total - CASE WHEN op.Importe IS NULL THEN  0 ELSE NULL END, 2) AS SaldoInicio FROM ComprobantesCompras cc
				        INNER JOIN Comprobantes c ON cc.IdComprobante = c.IdComprobante
                        LEFT JOIN (
                            SELECT opc.IdComprobanteCompra, SUM(ABS(opc.Importe)) AS Importe FROM OrdenesPagoComprobantes opc
                                INNER JOIN OrdenesPago op ON opc.IdOrdenPago = op.IdOrdenPago
                            WHERE
                                op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                op.IdEmpresa = {idEmpresa} AND
                                op.IdProveedor = {idProveedor} AND
                                op.Fecha < {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)}
                            GROUP BY opc.IdComprobanteCompra
                        ) op ON cc.idComprobanteCompra = op.idComprobanteCompra
				        WHERE
                            cc.IdProveedor = {idProveedor} AND 
					        cc.Fecha < {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)}");

            var sdoInicio = await builder.QuerySingleOrDefaultAsync<double?>();

            return sdoInicio ?? 0D;
        }

        public async Task<bool> RemoveAllByIdSdoCtaCtePrv(int idSdoCtaCtePrv, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesCompras
                    WHERE IdComprobanteCompra IN (
	                    SELECT IdComprobanteCompra
	                    FROM SaldoCtaCtePrvComprobantesCompras
	                    WHERE IdSaldoCtaCtePrv = {idSdoCtaCtePrv}
                    )");

            var results = await builder.ExecuteAsync();

            return (results == 1);
        }
        public async Task<List<Custom.SubdiarioCompras>> GetSubdiarioCompras(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT cc.IdComprobanteCompra, cc.Fecha, tc.Descripcion comprobante, cc.Sucursal, cc.Numero, p.RazonSocial Proveedor, p.CUIT as cuit, ta.Descripcion tipoIVA, cec.Descripcion CentroCostos, cc.Total
                                    FROM ComprobantesCompras cc
                                    INNER JOIN Comprobantes tc ON (tc.IdComprobante = cc.IdComprobante)
                                    INNER JOIN Proveedores p ON (cc.IdProveedor = p.IdProveedor)
                                    INNER JOIN TipoIVA ta ON (p.IdTipoIVA = ta.IdTipoIVA)
                                    INNER JOIN CentrosCosto cec ON (cec.IdCentroCosto = cc.IdCentroCosto)
                ");

            builder.Where($"cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

            if (filters.ContainsKey("IdEmpresa"))
                builder.Where($"cc.IdEmpresa = {filters["IdEmpresa"]}");

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"cc.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"cc.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            return (await builder.QueryAsync<SubdiarioCompras>()).AsList();
        }

        public async Task<List<Custom.SubdiarioComprasDetalle>> GetSubdiarioComprasDetalle(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT 
                    cc.IdComprobanteCompra, 
                    cc.Fecha, 
                    tc.Descripcion Comprobante, 
                    cc.Sucursal, 
                    cc.Numero, 
                    p.RazonSocial Proveedor, 
                    p.CUIT, 
                    ta.Descripcion TipoIVA, 
                    cec.Descripcion CentroCostos, 
                    cc.Total,
		            cuc.CodigoCuenta, 
                    cuc.Descripcion NombreCuenta, 
                    CASE WHEN ad.Debitos = 0 THEN ad.Creditos ELSE ad.Debitos END AS Importe
                FROM ComprobantesCompras cc
                    INNER JOIN Comprobantes tc ON (tc.IdComprobante = cc.IdComprobante)
                    INNER JOIN Proveedores p ON (cc.IdProveedor = p.IdProveedor)
                    INNER JOIN TipoIVA ta ON (p.IdTipoIVA = ta.IdTipoIVA)
                    INNER JOIN CentrosCosto cec ON (cec.IdCentroCosto = cc.IdCentroCosto)
                    INNER JOIN ComprobantesComprasAsientos cca ON (cca.IdComprobanteCompra = cc.IdComprobanteCompra)
                    INNER JOIN Asientos a ON (a.IdAsiento = cca.IdAsiento)
                    INNER JOIN AsientosDetalle ad ON (a.IdAsiento = ad.IdAsiento)
                    INNER JOIN CuentasContables cuc ON (cuc.IdCuentaContable = ad.IdCuentaContable)
                ");

            builder.Where($"cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

            if (filters.ContainsKey("IdEmpresa"))
                builder.Where($"cc.IdEmpresa = {filters["IdEmpresa"]}");

            if (filters.ContainsKey("IdComprobanteCompra"))
                builder.Where($"cc.IdComprobanteCompra = {filters["IdComprobanteCompra"]}");

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"cc.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"cc.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            return (await builder.QueryAsync<Custom.SubdiarioComprasDetalle>()).AsList();
        }

        public async Task<DateTime?> GetMinFechaComprobanteCompra(int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                                    MIN(Fecha) Fecha
                                FROM ComprobantesCompras cc
                                WHERE
                                    cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                    cc.IdEmpresa = {idEmpresa}");

            return await builder.QuerySingleOrDefaultAsync<DateTime?>();
        }

        public async Task<DateTime?> GetMaxFechaComprobanteCompra(int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                                    MAX(Fecha) Fecha
                                FROM ComprobantesCompras cc
                                WHERE
                                    cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                    cc.IdEmpresa = {idEmpresa}");

            return await builder.QuerySingleOrDefaultAsync<DateTime?>();
        }

        public async Task<Domain.Entities.ComprobanteCompra> GetComprobanteByProveedor(int idComprobante, string numeroComprobante, int idProveedor, IDbTransaction transaction = null)
        {
            var sucursal = numeroComprobante.Substring(0, 5);
            var numero = numeroComprobante.Substring(6, 8);


            var builder = _context.Connection
                .QueryBuilder($@"
                                SELECT
                                    cc.*
                                FROM ComprobantesCompras cc
                                WHERE
                                    cc.IdComprobante = {idComprobante} AND 
                                    cc.IdProveedor = {idProveedor} AND
                                    cc.Sucursal = {sucursal} AND
                                    cc.Numero = {numero} AND
                                    cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

            return await builder.QueryFirstOrDefaultAsync<Custom.ComprobanteCompra>(transaction);
        }

        public async Task<List<Custom.ComprobanteCompra>> GetComprobantesProveedor(Dictionary<string, object> filters)
        {
            //MIGRAR_A_MSSQL
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        cc.*,
                        co.Descripcion Comprobante,
                        EXISTS(SELECT 1 FROM OrdenesPagoComprobantes opc WHERE opc.IdComprobanteCompra = cc.IdComprobanteCompra) Pagada
                    FROM ComprobantesCompras cc
                    INNER JOIN Comprobantes co ON cc.IdComprobante = co.IdComprobante ");

            builder.Where($"cc.IdEmpresa = {filters["IdEmpresa"]}");
            builder.Where($"cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"cc.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"cc.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("IdProveedor"))
                builder.Where($"cc.IdProveedor = {filters["IdProveedor"]}");

            return (await builder.QueryAsync<Custom.ComprobanteCompra>()).AsList();
        }

        public async Task<List<DetalleAlicuota>> GetDetalleAlicuotas(DateTime? fechaDesde, DateTime? fechaHasta, int? idProveedor, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT 
                    a.Descripcion Alicuota,
                    SUM(CASE WHEN cct.Alicuota <> 0 THEN cct.Neto ELSE 0 END) NetoGravado, 
                    SUM(CASE WHEN cct.Alicuota = 0 THEN cct.Neto ELSE 0 END) NoGravado, 
                    SUM(cct.ImporteAlicuota) ImporteAlicuota, 
                    SUM(cct.Neto + cct.ImporteAlicuota) Total
                FROM ComprobantesComprasTotales cct
                    INNER JOIN Alicuotas a ON cct.Alicuota = a.Valor
                    INNER JOIN ComprobantesCompras cc ON cc.IdComprobanteCompra = cct.IdComprobanteCompra
                /**where**/
                GROUP BY cct.Alicuota, a.Descripcion
                ");

            builder.Where($"cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"cc.IdEmpresa = {idEmpresa}");

            if (idProveedor.HasValue)
                builder.Where($"cc.IdProveedor = {idProveedor}");

            if (fechaDesde.HasValue)
                builder.Where($"cc.Fecha >= {fechaDesde}");

            if (fechaHasta.HasValue)
                builder.Where($"cc.Fecha <= {fechaHasta}");

            return (await builder.QueryAsync<Custom.DetalleAlicuota>()).AsList();
        }

        public async Task<List<SubdiarioImputaciones>> GetSubdiarioImputaciones(DateTime? fechaDesde, DateTime? fechaHasta, int? idProveedor, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT 
		            cuc.CodigoCuenta, 
                    cuc.Descripcion NombreCuenta, 
                    SUM(ad.Debitos) AS Debitos,
                    SUM(ad.Creditos) AS Creditos
                FROM ComprobantesCompras cc
                    INNER JOIN ComprobantesComprasAsientos cca ON (cca.IdComprobanteCompra = cc.IdComprobanteCompra)
                    INNER JOIN Asientos a ON (a.IdAsiento = cca.IdAsiento)
                    INNER JOIN AsientosDetalle ad ON (a.IdAsiento = ad.IdAsiento)
                    INNER JOIN CuentasContables cuc ON (cuc.IdCuentaContable = ad.IdCuentaContable)
                /**where**/
                GROUP BY ad.IdCuentaContable, cuc.CodigoCuenta, cuc.Descripcion
                ");

            builder.Where($"cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"cc.IdEmpresa = {idEmpresa}");

            if (idProveedor.HasValue)
                builder.Where($"cc.IdProveedor = {idProveedor}");

            if (fechaDesde.HasValue)
                builder.Where($"cc.Fecha >= {fechaDesde}");

            if (fechaHasta.HasValue)
                builder.Where($"cc.Fecha <= {fechaHasta}");

            return (await builder.QueryAsync<SubdiarioImputaciones>()).AsList();
        }

		public async Task<List<ResumenCtaCteProPendienteGeneral>> GetDetalleGeneralCtasPagar(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
		{
			var builder = _context.Connection
				.QueryBuilder($@"
                    SELECT p.RazonSocial AS Proveedor, p.CUIT, cc.Fecha, cc.FechaVto, c.Descripcion AS Comprobante, cc.Sucursal, cc.Numero, ROUND(cc.Total - CASE WHEN op.Importe IS NULL THEN  0 ELSE NULL END, 2) AS Saldo FROM ComprobantesCompras cc
				        INNER JOIN comprobantes c ON cc.IdComprobante = c.IdComprobante
                        INNER JOIN proveedores p ON cc.IdProveedor = p.IdProveedor
                        LEFT JOIN (
                            SELECT opc.IdComprobanteCompra, SUM(ABS(opc.Importe)) AS Importe FROM ordenes_pago_comprobantes opc
                                INNER JOIN ordenes_pago op ON op.IdOrdenPago = opc.IdOrdenPago
                            WHERE
                                op.IdEmpresa = {idEmpresa} AND
                                op.Fecha BETWEEN {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)} AND {(fechaHasta.HasValue ? fechaHasta.Value : DateTime.MaxValue)} AND
                                op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}
                            GROUP BY opc.IdComprobanteCompra
                        ) op ON cc.idComprobanteCompra = op.idComprobanteCompra
				        WHERE
                            cc.IdEmpresa = {idEmpresa} AND
                            cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
					        cc.Fecha BETWEEN {(fechaDesde.HasValue ? fechaDesde.Value : DateTime.MinValue)} AND {(fechaHasta.HasValue ? fechaHasta.Value : DateTime.MaxValue)} AND
                            ROUND(cc.Total - CASE WHEN op.Importe IS NULL THEN  0 ELSE NULL END, 2) <> 0
				        ORDER BY cc.Fecha, cc.Numero
            ");

			return (await builder.QueryAsync<Custom.ResumenCtaCteProPendienteGeneral>()).AsList();
		}

        public async Task<List<Custom.ComprobanteCompraManualPercepcion>> GetPercepcionesByIdComprobanteCompra(int idComprobanteCpa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT ccp.IdComprobanteCompra, ROW_NUMBER() OVER(PARTITION BY ccp.IdComprobanteCompra ORDER BY ccp.IdComprobanteCompra) AS Item, cc.IdCuentaContable, cc.Descripcion AS CuentaContable, ccp.Importe FROM ComprobantesComprasPercepciones ccp
                        INNER JOIN CuentasContables cc ON ccp.IdCuentaContable = cc.IdCuentaContable
				        WHERE
                            ccp.IdComprobanteCompra = {idComprobanteCpa}
            ");

            return (await builder.QueryAsync<Custom.ComprobanteCompraManualPercepcion>()).AsList();
        }
    }
}
