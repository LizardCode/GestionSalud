using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class RecibosComprobantesRepository : BaseRepository, IRecibosComprobantesRepository
    {
        public RecibosComprobantesRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> DeleteByIdRecibo(int idRecibo, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM RecibosComprobantes
                    WHERE 
                        IdRecibo = {idRecibo} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<Custom.ReciboComprobante>> GetComprobantesImputar(int idCliente, string Moneda, int idEmpresa, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT 0 Seleccionar, cv.IdComprobanteVenta, cv.Fecha, c.Descripcion AS TipoComprobante, (cv.Sucursal + '-' + cv.Numero) NumeroComprobante, cv.Total, ROUND(cv.Total - ISNULL(ABS(sdo.Saldo), 0), 2) Saldo, '0' Importe, c.EsCredito, cv.Cotizacion
                        FROM ComprobantesVentas cv
                        INNER JOIN Comprobantes c ON cv.IdComprobante = c.IdComprobante
                        LEFT JOIN (
                            SELECT rc.IdComprobanteVenta, SUM(rc.Importe) AS Saldo FROM RecibosComprobantes rc
                            INNER JOIN Recibos r ON r.IdRecibo = rc.IdRecibo
                            WHERE
                                r.IdCliente = {idCliente}
                            GROUP BY IdComprobanteVenta
                        ) sdo ON cv.IdComprobanteVenta = sdo.idComprobanteVenta
                ")
                .Where($"cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"cv.Moneda = {Moneda}")
                .Where($"cv.IdCliente = {idCliente}")
                .Where($"ROUND(cv.Total - ISNULL(ABS(sdo.Saldo), 0), 2) > 0")
                .Where($"cv.IdEmpresa = {idEmpresa}");

            return (await query.QueryAsync<Custom.ReciboComprobante>(transaction)).AsList();
        }

        public async Task<IList<Custom.ReciboComprobante>> GetComprobantesByIdRecibo(int idRecibo, IDbTransaction transaction = null)
        {
            var query = _context.Connection
               .QueryBuilder($@"
                    SELECT 
                            1 Seleccionar, 
                            cv.IdComprobanteVenta, 
                            cv.Fecha,
                            c.Descripcion AS TipoComprobante, 
                            (cv.Sucursal + '-' + cv.Numero) NumeroComprobante,
                            cv.Total, 
                            '0' Saldo, 
                            rc.Importe,
                            cv.Cotizacion
                        FROM ComprobantesVentas cv
                        INNER JOIN Comprobantes c ON cv.IdComprobante = c.IdComprobante
                        INNER JOIN RecibosComprobantes rc ON cv.IdComprobanteVenta = rc.IdComprobanteVenta
                ")
               .Where($"rc.IdRecibo = {idRecibo}");

            return (await query.QueryAsync<Custom.ReciboComprobante>()).AsList();
        }

        public async Task<ReciboComprobante> GetByIdComprobanteVenta(int idComprobanteVta, IDbTransaction transaction = null)
        {
            var query = _context.Connection
               .QueryBuilder($@"
                    SELECT 
                        IdReciboComprobante,
		                IdRecibo,
		                IdComprobanteVenta,
		                Importe,
                        Cotizacion
                    FROM RecibosComprobantes rc
            ")
               .Where($"rc.IdComprobanteVenta = {idComprobanteVta}");

            return await query.QueryFirstOrDefaultAsync<ReciboComprobante>();
        }
    }
}
