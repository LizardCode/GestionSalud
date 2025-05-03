using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ComprobantesVentasItemRepository : IComprobantesVentasItemRepository
    {
        private readonly IDbContext _context;

        public ComprobantesVentasItemRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesVentasItems
                    WHERE IdComprobanteVenta = {idComprobanteVenta}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> DeleteByIdComprobanteVentaAndItem(int idComprobanteVenta, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesVentasItems
                    WHERE IdComprobanteVenta = {idComprobanteVenta} AND Item = {item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<ComprobanteVentaItem>> GetAllByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    cvi.*
                    FROM ComprobantesVentasItems cvi
                    WHERE
                        cvi.IdComprobanteVenta = {idComprobanteVenta}");

            var results = await builder.QueryAsync<ComprobanteVentaItem>(transaction);

            return results.AsList();
        }

        public async Task<ComprobanteVentaItem> GetByIdComprobanteVentaAndItem(int idComprobanteVenta, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM ComprobantesVentasItems
                    WHERE
	                    IdComprobanteVenta = {idComprobanteVenta}
	                    AND Item = {item}"
                );

            return await builder.QuerySingleOrDefaultAsync<ComprobanteVentaItem>(transaction);
        }

        public async Task<List<Domain.EntitiesCustom.ComprobanteVentaItem>> GetItemsNCAnulaByFactura(int idComprobante, string sucursal, string numero, int idEmpresa, IDbTransaction transaction = null)
        {
            //MIGRAR_A_MSSQL
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                            1 Seleccion,
                            1 Item,
                            CONCAT(cv.Sucursal, '-', cv.Numero) Descripcion,
                            cv.Moneda IdMoneda,
                            cv.Moneda,
                            cvi.Alicuota,
                            SUM(cvi.Importe) Importe
                        FROM comprobantes_ventas_items cvi
                        INNER JOIN comprobantes_ventas cv ON cvi.IdComprobanteVenta = cv.IdComprobanteVenta
                    WHERE
                        cv.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                        cv.IdComprobante = {idComprobante} AND
                        cv.Sucursal = {sucursal} AND
                        cv.Numero = {numero} AND
                        cv.IdEmpresa = {idEmpresa}
                    GROUP BY cvi.IdComprobanteVenta, cvi.Alicuota");

            var results = await builder.QueryAsync<Domain.EntitiesCustom.ComprobanteVentaItem>(transaction);

            return results.AsList();
        }

        public async Task<bool> Insert(ComprobanteVentaItem entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO ComprobantesVentasItems
                    (
                        IdComprobanteVenta,
                        Item,
                        IdArticulo,
                        IdEvolucionPrestacion,
                        IdEvolucionOtraPrestacion,
                        Descripcion,
                        Cantidad,
                        Precio,
                        Bonificacion,
                        Importe,
                        Impuestos,
                        Alicuota
                    )
                    VALUES
                    (
                        {entity.IdComprobanteVenta},
                        {entity.Item},
                        {entity.IdArticulo},
                        {entity.IdEvolucionPrestacion},
                        {entity.IdEvolucionOtraPrestacion},
                        {entity.Descripcion},
                        {entity.Cantidad},
                        {entity.Precio},
                        {entity.Bonificacion},
                        {entity.Importe},
                        {entity.Impuestos},
                        {entity.Alicuota}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> Update(ComprobanteVentaItem entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE ComprobantesVentasItems SET
                        IdArticulo = {entity.IdArticulo},
	                    Descripcion = {entity.Descripcion},
	                    Cantidad = {entity.Cantidad},
                        Precio = {entity.Precio},
                        Bonificacion = {entity.Bonificacion},
                        Importe = {entity.Importe},
                        Impuestos = {entity.Impuestos},
                        Alicuota = {entity.Alicuota}
                     WHERE
	                    IdComprobanteVenta = {entity.IdComprobanteVenta} AND Item = {entity.Item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
        public async Task<List<Domain.EntitiesCustom.ComprobanteVentaItem>> GetItemsByClientePaciente(int idCliente, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                            1 Seleccion,
                            eop.idEvolucionPrestacion Item,
                            eop.idEvolucionPrestacion IdEvolucionOtraPrestacion,
                            CONVERT(varchar(10), e.fecha, 103) + ' - ' + eop.codigo + ' - ' + eop.Descripcion Descripcion, 
                            {(int)Monedas.MonedaLocal} IdMoneda,
                            'PESOS ARGENTINOS' Moneda,
                            21 Alicuota, 
                            ROUND(eop.valor - ISNULL(cvif.importe, 0), 2) Importe
                        FROM EvolucionesOtrasPrestaciones eop
	                    INNER JOIN Evoluciones e ON (e.idEvolucion = eop.idEvolucion)
	                    INNER JOIN Clientes c ON (c.IdPaciente = e.IdPaciente)
                        LEFT JOIN (
                            SELECT cvi.IdEvolucionOtraPrestacion, SUM(cvi.Importe) Importe 
		                    FROM ComprobantesVentasItems cvi
		                    INNER JOIN ComprobantesVentas cv ON (cvi.IdComprobanteVenta = cv.IdComprobanteVenta)
                            WHERE cvi.IdEvolucionOtraPrestacion > 0
                                AND cv.IdEmpresa = {idEmpresa}
                                AND cv.IdCliente = {idCliente}
                            GROUP BY cvi.IdEvolucionOtraPrestacion
                        ) cvif ON cvif.IdEvolucionOtraPrestacion = eop.IdEvolucionPrestacion
                    WHERE
	                    c.IdCliente = {idCliente} AND
                        e.IdEmpresa = {idEmpresa} AND
                        ROUND(eop.valor - ISNULL(cvif.importe, 0), 2) > 0 
                    ORDER BY eop.idEvolucionPrestacion ASC ");

            var results = await builder.QueryAsync<Domain.EntitiesCustom.ComprobanteVentaItem>(transaction);

            return results.AsList();
        }

        public async Task<List<Domain.EntitiesCustom.ComprobanteVentaItem>> GetItemsCoPagoByClientePaciente(int idCliente, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                            1 Seleccion,
                            ep.idEvolucionPrestacion Item,
                            ep.idEvolucionPrestacion IdEvolucionPrestacion,
                            '(CO-PAGO) ' + ' - ' + CONVERT(varchar(10), e.fecha, 103) + ' - ' +  ep.codigo + ' - ' + ep.Descripcion Descripcion, 
                            {(int)Monedas.MonedaLocal} IdMoneda,
                            'PESOS ARGENTINOS' Moneda,
                            21 Alicuota, 
                            ROUND(ep.valor - ISNULL(cvif.importe, 0), 2) Importe
                        FROM EvolucionesPrestaciones ep
	                    INNER JOIN Evoluciones e ON (e.idEvolucion = ep.idEvolucion)
	                    INNER JOIN Clientes c ON (c.IdPaciente = e.IdPaciente)
                        LEFT JOIN (
                            SELECT cvi.IdEvolucionPrestacion, SUM(cvi.Importe) Importe 
		                    FROM ComprobantesVentasItems cvi
		                    INNER JOIN ComprobantesVentas cv ON (cvi.IdComprobanteVenta = cv.IdComprobanteVenta)
                            WHERE cvi.IdEvolucionPrestacion > 0
                                AND cv.IdEmpresa = {idEmpresa}
                                AND cv.IdCliente = {idCliente}
                            GROUP BY cvi.IdEvolucionPrestacion
                        ) cvif ON cvif.IdEvolucionPrestacion = ep.IdEvolucionPrestacion
                    WHERE
	                    c.IdCliente = {idCliente} AND
                        e.IdEmpresa = {idEmpresa} AND
                        ep.IdTipoPrestacion = {(int)TipoPrestacion.CoPago} AND
                        ROUND(ep.valor - ISNULL(cvif.importe, 0), 2) > 0 
                    ORDER BY ep.idEvolucionPrestacion ASC ");

            var results = await builder.QueryAsync<Domain.EntitiesCustom.ComprobanteVentaItem>(transaction);

            return results.AsList();
        }

        public async Task<List<Domain.EntitiesCustom.ComprobanteVentaItem>> GetItemsByClienteFinanciador(int idCliente, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                            1 Seleccion,
                            ep.idEvolucionPrestacion Item,
                            ep.idEvolucionPrestacion IdEvolucionPrestacion,
                            CONVERT(nvarchar(10), e.fecha, 105) + ' - ' + e.FinanciadorNro + ' - ' + ep.codigo + ' - ' + ep.Descripcion Descripcion, 
                            {(int)Monedas.MonedaLocal} IdMoneda,
                            'PESOS ARGENTINOS' Moneda,
                            21 Alicuota, 
                            ROUND(ep.valor - ISNULL(cvif.importe, 0), 2) Importe
                        FROM EvolucionesPrestaciones ep
	                    INNER JOIN Evoluciones e ON (e.idEvolucion = ep.idEvolucion)
	                    INNER JOIN Clientes c ON (c.IdFinanciador = e.IdFinanciador)
                        LEFT JOIN (
                            SELECT cvi.IdEvolucionPrestacion, SUM(cvi.Importe) Importe 
		                    FROM ComprobantesVentasItems cvi
		                    INNER JOIN ComprobantesVentas cv ON (cvi.IdComprobanteVenta = cv.IdComprobanteVenta)
                            WHERE cvi.IdEvolucionPrestacion > 0
                                --AND cv.IdEmpresa = {idEmpresa}
                                AND cv.IdCliente = {idCliente}
                            GROUP BY cvi.IdEvolucionPrestacion
                        ) cvif ON cvif.IdEvolucionPrestacion = ep.IdEvolucionPrestacion
                    WHERE e.idEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND 
	                    c.IdCliente = {idCliente} AND
                        --e.IdEmpresa = {idEmpresa} AND
                        ep.IdTipoPrestacion = {(int)TipoPrestacion.Prestacion} AND
                        ROUND(ep.valor - ISNULL(cvif.importe, 0), 2) > 0 
                    ORDER BY ep.idEvolucionPrestacion ASC ");

            var results = await builder.QueryAsync<Domain.EntitiesCustom.ComprobanteVentaItem>(transaction);

            return results.AsList();
        }

        public async Task<double> GetSaldoPrestacion(int idEvolucionPrestacion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT ROUND(ep.valor - ISNULL(cvif.importe, 0), 2) Saldo 
                        FROM EvolucionesPrestaciones ep
                        LEFT JOIN (
                            SELECT cvi.IdEvolucionPrestacion, SUM(cvi.Importe) Importe 
		                    FROM ComprobantesVentasItems cvi
                            WHERE cvi.IdEvolucionPrestacion = {idEvolucionPrestacion}
                            GROUP BY cvi.IdEvolucionPrestacion
                        ) cvif ON cvif.IdEvolucionPrestacion = ep.IdEvolucionPrestacion
                    WHERE
	                    ep.IdEvolucionPrestacion = {idEvolucionPrestacion} ");

            var result = await builder.QuerySingleOrDefaultAsync<double>(transaction);

            return result;
        }

        public async Task<double> GetSaldoOtraPrestacion(int idEvolucionOtraPrestacion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT ROUND(ep.valor - ISNULL(cvif.importe, 0), 2) Saldo 
                        FROM EvolucionesOtrasPrestaciones ep
                        LEFT JOIN (
                            SELECT cvi.IdEvolucionOtraPrestacion, SUM(cvi.Importe) Importe 
		                    FROM ComprobantesVentasItems cvi
                            WHERE cvi.IdEvolucionOtraPrestacion = {idEvolucionOtraPrestacion}
                            GROUP BY cvi.IdEvolucionOtraPrestacion
                        ) cvif ON cvif.IdEvolucionOtraPrestacion = ep.IdEvolucionPrestacion
                    WHERE
	                    ep.IdEvolucionPrestacion = {idEvolucionOtraPrestacion} ");

            var result = await builder.QuerySingleOrDefaultAsync<double>(transaction);

            return result;
        }
    }
}
