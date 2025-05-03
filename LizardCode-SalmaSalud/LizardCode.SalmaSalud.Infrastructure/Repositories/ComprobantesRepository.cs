using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ComprobantesRepository : BaseRepository, IComprobantesRepository
    {
        public ComprobantesRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT c.* FROM Comprobantes c");

            return base.GetAllCustomQuery(query);
        }

        public async Task<IList<Comprobante>> GetComprobantesByCliente(int idCliente)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT DISTINCT
	                    c.*
                    FROM Comprobantes c
                    INNER JOIN ComprobantesTiposIVA cti ON c.IdComprobante = cti.IdComprobante
                    INNER JOIN Clientes cl ON cl.IdTipoIVA = cti.IdTipoIVA
                    WHERE
                        cl.IdCliente = {idCliente}");

            var results = await builder.QueryAsync<Comprobante>();

            return results.AsList();
        }

        public async Task<IList<Comprobante>> GetComprobantesByProveedor(int idProveedor)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT DISTINCT
	                    c.*
                    FROM Comprobantes c
                    INNER JOIN ComprobantesTiposIVA cti ON c.IdComprobante = cti.IdComprobante
                    INNER JOIN Proveedores p ON p.IdTipoIVA = cti.IdTipoIVA
                    WHERE
                        p.IdProveedor = {idProveedor}");

            var results = await builder.QueryAsync<Comprobante>();

            return results.AsList();
        }

        public async Task<IList<Comprobante>> GetComprobantesByTipoIVA(int idTipoIVA)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT DISTINCT
	                    c.*
                    FROM Comprobantes c
                    INNER JOIN ComprobantesTiposIVA cti ON c.IdComprobante = cti.IdComprobante
                    WHERE
                        cti.IdTipoIVA = {idTipoIVA}");

            var results = await builder.QueryAsync<Comprobante>();

            return results.AsList();
        }

        public async Task<IList<Comprobante>> GetComprobantesParaCredito()
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT DISTINCT
	                    c.*
                    FROM Comprobantes c
                    INNER JOIN ComprobantesTiposIVA cti ON c.IdComprobante = cti.IdComprobante
                    WHERE
                        cti.IdComprobanteCredito IS NOT NULL");

            var results = await builder.QueryAsync<Comprobante>();

            return results.AsList();
        }

        public async Task<IList<Comprobante>> GetComprobantes()
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT DISTINCT
	                    c.*
                    FROM Comprobantes c");

            var results = await builder.QueryAsync<Comprobante>();

            return results.AsList();
        }

        public async Task<int> GetCantidadFacturasCompras(DateTime desde, DateTime hasta, int idEmpresa)
        {
            FormattableString sql = $@"SELECT COUNT(*)
                                        FROM ComprobantesCompras cc
                                        WHERE 
                                            cc.IdEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND
                                            cc.IdEmpresa = {idEmpresa} AND
                                            cc.Fecha BETWEEN {desde.ToString("yyyy-MM-dd") + "T00:00:00.000"} AND {hasta.ToString("yyyy-MM-dd") + "T23:59:00.000"} ";

            var builder = _context.Connection.QueryBuilder(sql);
            var result = await builder.ExecuteScalarAsync<int>();

            return result;
        }

        public async Task<int> GetCantidadFacturasVentas(DateTime desde, DateTime hasta, int idEmpresa)
        {
            FormattableString sql = $@"SELECT COUNT(*)
                                        FROM ComprobantesVentas cv
                                        WHERE 
                                            cv.IdEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND
                                            cv.IdEmpresa = {idEmpresa} AND
                                            cv.Fecha BETWEEN {desde.ToString("yyyy-MM-dd") + "T00:00:00.000"} AND {hasta.ToString("yyyy-MM-dd") + "T23:59:00.000"} ";

            var builder = _context.Connection.QueryBuilder(sql);
            var result = await builder.ExecuteScalarAsync<int>();

            return result;
        }

        public async Task<decimal> GetIVACompras(DateTime desde, DateTime hasta, int idEmpresa)
        {
            FormattableString sql = $@"SELECT SUM(ImporteAlicuota) AS Importe
                                        FROM ComprobantesComprasTotales cct
                                            INNER JOIN ComprobantesCompras cc ON cct.IdComprobanteCompra = cc.IdComprobanteCompra
                                        WHERE 
                                            cc.IdEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND
                                            cc.IdEmpresa = {idEmpresa} AND
                                            cc.Fecha BETWEEN {desde.ToString("yyyy-MM-dd") + "T00:00:00.000"} AND {hasta.ToString("yyyy-MM-dd") + "T23:59:00.000"} ";

            var builder = _context.Connection.QueryBuilder(sql);
            var result = await builder.ExecuteScalarAsync<decimal>();

            return result;
        }

        public async Task<decimal> GetIVAVentas(DateTime desde, DateTime hasta, int idEmpresa)
        {
            FormattableString sql = $@"SELECT SUM(ImporteAlicuota) AS Importe
                                        FROM ComprobantesVentasTotales cvt
                                            INNER JOIN ComprobantesVentas cv ON cvt.IdComprobanteVenta = cv.IdComprobanteVenta
                                        WHERE 
                                            cv.idEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND
                                            cv.IdEmpresa = {idEmpresa} AND
                                            cv.fecha BETWEEN {desde.ToString("yyyy-MM-dd") + "T00:00:00.000"} AND {hasta.ToString("yyyy-MM-dd") + "T23:59:00.000"} ";

            var builder = _context.Connection.QueryBuilder(sql);
            var result = await builder.ExecuteScalarAsync<decimal>();

            return result;
        }


        public async Task<int> GetCantidadFacturasComprasProveedor(int idProveedor, int idEmpresa)
        {
            FormattableString sql = $@"SELECT COUNT(*)
                                        FROM ComprobantesCompras cc
                                        WHERE 
                                            cc.IdEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND
                                            cc.IdEmpresa = {idEmpresa} AND
                                            cc.IdProveedor = {idProveedor} AND
                                            cc.Fecha BETWEEN DATE_ADD(NOW(), INTERVAL -60 DAY) AND NOW() ";

            var builder = _context.Connection.QueryBuilder(sql);
            var result = await builder.ExecuteScalarAsync<int>();

            return result;
        }


        public async Task<int> GetCantidadFacturasComprasPagasProveedor(int idProveedor, int idEmpresa)
        {
            FormattableString sql = $@"SELECT COUNT(*) 
                                        FROM ComprobantesCompras cc
                                        WHERE
                                            cc.IdEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND
                                            cc.IdComprobanteCompra IN (SELECT IdComprobanteCompra FROM OrdenesPagoComprobantes) AND
                                            cc.IdProveedor = {idProveedor} AND
                                            cc.IdEmpresa = {idEmpresa} AND
                                            cc.Fecha BETWEEN DATE_ADD(NOW(), INTERVAL -60 DAY) AND NOW() ";


            var builder = _context.Connection.QueryBuilder(sql);
            var result = await builder.ExecuteScalarAsync<int>();

            return result;
        }
        public DataTablesCustomQuery GetUltimasFacturasProveedor(int idProveedor, int idEmpresa)
        {
            FormattableString sql = $@"SELECT cc.fecha, CONCAT(c.Letra, ' ', cc.sucursal, '-', cc.Numero) as Comprobante, cc.Total as Importe
                                        FROM ComprobantesCompras cc
                                        INNER JOIN Comprobantes c ON (cc.IdComprobante = c.IdComprobante)
                                        WHERE cc.idEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND 
                                                cc.IdEmpresa = {idEmpresa} AND 
                                                cc.IdProveedor = {idProveedor}  
                                        ORDER BY cc.IdComprobanteCompra DESC 
                                        LIMIT 10 ";

            var builder = _context.Connection.QueryBuilder(sql);

            return base.GetAllCustomQuery(builder);
        }

    }
}
