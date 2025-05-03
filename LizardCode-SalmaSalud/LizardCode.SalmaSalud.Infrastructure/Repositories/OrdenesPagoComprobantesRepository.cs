using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;


namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class OrdenesPagoComprobantesRepository : BaseRepository, IOrdenesPagoComprobantesRepository
    {
        public OrdenesPagoComprobantesRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> DeleteByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM OrdenesPagoComprobantes
                    WHERE 
                        IdOrdenPago = {idOrdenPago} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<Domain.Entities.OrdenPagoComprobante> GetByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null)
        {
            var query = _context.Connection
               .QueryBuilder($@"
                    SELECT 
                        IdOrdenPagoComprobante,
		                IdOrdenPago,
		                IdComprobanteCompra,
		                Importe
                    FROM OrdenesPagoComprobantes opc
                ")
               .Where($"opc.IdComprobanteCompra = {idComprobanteCompra}");

            return await query.QueryFirstOrDefaultAsync<Domain.Entities.OrdenPagoComprobante>();
        }

        public async Task<IList<Custom.OrdenPagoComprobante>> GetComprobantesByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null)
        {
            var query = _context.Connection
               .QueryBuilder($@"
                    SELECT 
                            1 Seleccionar, 
                            cc.IdComprobanteCompra, 
                            cc.Fecha,
                            c.Descripcion AS TipoComprobante, 
                            (cc.Sucursal + '-' + cc.Numero) NumeroComprobante, 
                            cc.Total, 
                            '0' Saldo, 
                            opc.Importe,
                            cc.Cotizacion
                        FROM ComprobantesCompras cc
                        INNER JOIN Comprobantes c ON cc.IdComprobante = c.IdComprobante
                        INNER JOIN OrdenesPagoComprobantes opc ON cc.IdComprobanteCompra = opc.IdComprobanteCompra
                ")
               .Where($"opc.IdOrdenPago = {idOrdenPago}");

            return (await query.QueryAsync<Custom.OrdenPagoComprobante>()).AsList();
        }

        public async Task<List<Custom.OrdenPagoComprobante>> GetComprobantesImputar(int idProveedor, string idModena, int idEmpresa, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                                SELECT 0 Seleccionar, cc.IdComprobanteCompra, cc.Fecha, c.Descripcion AS TipoComprobante, (cc.Sucursal + '-' + cc.Numero) NumeroComprobante, cc.Total, ROUND(cc.Total - ISNULL(ABS(sdo.Saldo), 0), 2) Saldo, '0' Importe, c.EsCredito, cc.Cotizacion
                                    FROM ComprobantesCompras cc
                                    INNER JOIN Comprobantes c ON cc.IdComprobante = c.IdComprobante
                                    LEFT JOIN (
                                        SELECT opc.IdComprobanteCompra, SUM(opc.Importe) AS Saldo FROM OrdenesPagoComprobantes opc
                                        INNER JOIN OrdenesPago op ON op.IdOrdenPago = opc.IdOrdenPago
                                        WHERE
                                            op.IdProveedor = {idProveedor}
                                        GROUP BY opc.IdComprobanteCompra
                                    ) sdo ON cc.IdComprobanteCompra = sdo.IdComprobanteCompra
                ")
                .Where($"cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"cc.IdProveedor = {idProveedor}")
                .Where($"ROUND(cc.Total - ISNULL(ABS(sdo.Saldo), 0), 2) > 0")
                .Where($"cc.IdEmpresa = {idEmpresa}")
                .Where($"cc.Moneda = {idModena}");

            return (await query.QueryAsync<Custom.OrdenPagoComprobante>(transaction)).AsList();
        }

        public async Task<List<Custom.OrdenPagoPlanillaGasto>> GetPlanillasImputar(string idModena, int idEmpresa, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                                SELECT 0 Seleccionar, pg.IdPlanillaGastos, pg.Fecha, pg.Descripcion, pg.ImporteTotal Importe
                                    FROM PlanillaGastos pg
                ")
                .Where($"pg.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"pg.IdEstadoPlanilla = {(int)EstadoPlanillaGastos.Ingresada}")
                .Where($"pg.IdEmpresa = {idEmpresa}")
                .Where($"pg.Moneda = {idModena}");

            return (await query.QueryAsync<Custom.OrdenPagoPlanillaGasto>(transaction)).AsList();
        }

        public async Task<List<Custom.OrdenPagoGrilla>> GetOrdenPagoByIdComprobanteCompra(int idComprobanteCpa)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                                SELECT op.IdOrdenPago, op.Fecha, op.Descripcion, opc.Importe FROM OrdenesPago op
                                    INNER JOIN OrdenesPagoComprobantes opc ON op.IdOrdenPago = opc.IdOrdenPago
                
                        ")
                .Where($"opc.IdComprobanteCompra = {idComprobanteCpa}");

            return (await query.QueryAsync<Custom.OrdenPagoGrilla>()).AsList();
        }

    }
}
