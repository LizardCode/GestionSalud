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
    public class ComprobantesComprasItemRepository : IComprobantesComprasItemRepository
    {
        private readonly IDbContext _context;

        public ComprobantesComprasItemRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesComprasItems
                    WHERE IdComprobanteCompra = {idComprobanteCompra}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> DeleteByIdComprobanteCompraAndItem(int idComprobanteCompra, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ComprobantesComprasItems
                    WHERE IdComprobanteCompra = {idComprobanteCompra} AND Item = {item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<Custom.ComprobanteCompraItem>> GetAllByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    cci.*
                    FROM ComprobantesComprasItems cci
                    WHERE
                        cci.IdComprobanteCompra = {idComprobanteCompra}");

            var results = await builder.QueryAsync<Custom.ComprobanteCompraItem>(transaction);

            return results.AsList();
        }

        public async Task<IList<Custom.ComprobanteCompraManualItem>> GetAllByIdComprobanteCompraManual(int idComprobanteCompra, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    cci.*,
                        a.IdAlicuota,
                        cc.Descripcion CuentaContable
                    FROM ComprobantesComprasItems cci
                        INNER JOIN Alicuotas a ON cci.Alicuota = a.Valor
                        INNER JOIN CuentasContables cc ON cci.IdCuentaContable = cc.IdCuentaContable
                    WHERE
                        cci.IdComprobanteCompra = {idComprobanteCompra}");

            var results = await builder.QueryAsync<Custom.ComprobanteCompraManualItem>(transaction);

            return results.AsList();
        }

        public async Task<ComprobanteCompraItem> GetByIdComprobanteCompraAndItem(int idComprobanteCompra, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM ComprobantesComprasItems
                    WHERE
	                    IdComprobanteCompra = {idComprobanteCompra}
	                    AND Item = {item}"
                );

            return await builder.QuerySingleOrDefaultAsync<ComprobanteCompraItem>(transaction);
        }

        public async Task<List<Domain.EntitiesCustom.ComprobanteCompraItem>> GetItemsNCAnulaByFactura(int idComprobante, string numeroComprobante, int idProveedor, int idEmpresa, IDbTransaction transaction = null)
        {
            var sucursal = numeroComprobante.Substring(0, 5);
            var numero = numeroComprobante.Substring(6, 8);

            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                            1 Seleccion,
                            1 Item,
                            (cc.Sucursal + '-' + cc.Numero) Descripcion,
                            cc.Moneda IdMoneda,
                            cc.Moneda,
                            cci.Alicuota,
                            SUM(cci.Importe) Importe
                        FROM ComprobantesComprasItems cci
                        INNER JOIN ComprobantesCompras cc ON cci.IdComprobanteCompra = cc.IdComprobanteCompra
                    WHERE
                        cc.IdComprobante = {idComprobante} AND
                        cc.Sucursal = {sucursal} AND
                        cc.Numero = {numero} AND
                        cc.IdEmpresa = {idEmpresa}
                    GROUP BY cci.IdComprobanteCompra, cci.Alicuota, cc.Sucursal, cc.Numero, cc.Moneda");

            var results = await builder.QueryAsync<Custom.ComprobanteCompraItem>(transaction);

            return results.AsList();
        }

        public async Task<bool> Insert(ComprobanteCompraItem entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO ComprobantesComprasItems
                    (
                        IdComprobanteCompra,
                        Item,
                        IdArticulo,
                        IdCuentaContable,
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
                        {entity.IdComprobanteCompra},
                        {entity.Item},
                        {entity.IdArticulo},
                        {entity.IdCuentaContable},
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

        public async Task<bool> Update(ComprobanteCompraItem entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE ComprobantesComprasItems SET
                        IdArticulo = {entity.IdArticulo},
                        IdCuentaContable = {entity.IdCuentaContable},
	                    Descripcion = {entity.Descripcion},
	                    Cantidad = {entity.Cantidad},
                        Precio = {entity.Precio},
                        Bonificacion = {entity.Bonificacion},
                        Importe = {entity.Importe},
                        Impuestos = {entity.Impuestos},
                        Alicuota = {entity.Alicuota}
                     WHERE
	                    IdComprobanteCompra = {entity.IdComprobanteCompra} AND Item = {entity.Item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}
