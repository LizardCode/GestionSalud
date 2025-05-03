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
    public class PlanillaGastosItemsRepository : IPlanillaGastosItemsRepository
    {
        private readonly IDbContext _context;

        public PlanillaGastosItemsRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM PlanillaGastosItems
                    WHERE IdPlanillaGastos = {idPlanillaGastos}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> DeleteByIdPlanillaGastosAndItem(int idPlanillaGastos, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM PlanillaGastosItems
                    WHERE IdPlanillaGastos = {idPlanillaGastos} AND Item = {item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<List<Domain.EntitiesCustom.PlanillaGastoItem>> GetByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
                        pgi.IdPlanillaGastos,
		                pgi.Item,
		                pgi.CUIT,
		                pgi.Proveedor,
		                pgi.Fecha,
		                pgi.IdComprobante,
                        c.Descripcion Comprobante,
		                pgi.NumeroComprobante,
                        pgi.Detalle,
                        pgi.SubtotalNoGravado,
		                pgi.Subtotal,
                        pgi.IdAlicuota,
		                pgi.Alicuota,
                        pgi.Subtotal2,
                        pgi.IdAlicuota2,
		                pgi.Alicuota2,
		                pgi.IdCuentaContablePercepcion,
		                pgi.Percepcion,
                        pgi.IdCuentaContablePercepcion2,
                        pgi.Percepcion2,
                        pgi.ImpuestosInternos,
		                pgi.Total,
                        pgi.CAE,
                        pgi.VencimientoCAE
                    FROM PlanillaGastosItems pgi
                        INNER JOIN Comprobantes c ON pgi.IdComprobante = c.IdComprobante
                    WHERE
	                    pgi.IdPlanillaGastos = {idPlanillaGastos}"
                );

            var result = await builder.QueryAsync<Domain.EntitiesCustom.PlanillaGastoItem>(transaction);

            return result.AsList();
        }

        public async Task<PlanillaGastoItem> GetByIdPlanillaGastosAndItem(int idPlanillaGastos, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM PlanillaGastosItems
                    WHERE
	                    IdPlanillaGastos = {idPlanillaGastos}
	                    AND Item = {item}"
                );

            return await builder.QuerySingleOrDefaultAsync<PlanillaGastoItem>(transaction);
        }

        public async Task<double> GetImportePlanillaItem(int item, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT ISNULL(ROUND(SUM(pgi.Subtotal), 2), 0) Importe FROM PlanillaGastosItems pgi
                        INNER JOIN PlanillaGastos pg ON pgi.IdPlanillaGastos = pg.IdPlanillaGastos ")
                .Where($"pg.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"pg.Item = {item}");

            return await query.QueryFirstOrDefaultAsync<double>(transaction);
        }

        public async Task<bool> Insert(PlanillaGastoItem entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO PlanillaGastosItems
                    (
                        IdPlanillaGastos,
                        Item,
                        CUIT,
                        Proveedor,
                        Fecha,
                        IdComprobante,
                        NumeroComprobante,
                        Detalle,
                        SubtotalNoGravado,
                        Subtotal,
                        IdAlicuota,
                        Alicuota,
                        Subtotal2,
                        IdAlicuota2,
                        Alicuota2,
                        IdCuentaContablePercepcion,
                        Percepcion,
                        IdCuentaContablePercepcion2,
                        Percepcion2,
                        ImpuestosInternos,
                        Total,
                        CAE,
                        VencimientoCAE
                    )
                    VALUES
                    (
                        {entity.IdPlanillaGastos},
                        {entity.Item},
                        {entity.CUIT},
                        {entity.Proveedor},
                        {entity.Fecha},
                        {entity.IdComprobante},
                        {entity.NumeroComprobante},
                        {entity.Detalle},
                        {entity.SubtotalNoGravado},
                        {entity.Subtotal},
                        {entity.IdAlicuota},
                        {entity.Alicuota},
                        {entity.Subtotal2},
                        {entity.IdAlicuota2},
                        {entity.Alicuota2},
                        {entity.IdCuentaContablePercepcion},
                        {entity.Percepcion},
                        {entity.IdCuentaContablePercepcion2},
                        {entity.Percepcion2},
                        {entity.ImpuestosInternos},
                        {entity.Total},
                        {entity.CAE},
                        {entity.VencimientoCAE}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> Update(PlanillaGastoItem entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE PlanillaGastosItems SET
                        CUIT = {entity.CUIT},
                        Proveedor = {entity.Proveedor},
                        Fecha = {entity.Fecha},
                        IdComprobante = {entity.IdComprobante},
                        NumeroComprobante = {entity.NumeroComprobante},
                        Detalle = {entity.Detalle},
                        SubtotalNoGravado = {entity.SubtotalNoGravado},
                        Subtotal = {entity.Subtotal},
                        IdAlicuota = {entity.IdAlicuota},
                        Alicuota = {entity.Alicuota},
                        Subtotal2 = {entity.Subtotal2},
                        IdAlicuota2 = {entity.IdAlicuota2},
                        Alicuota2 = {entity.Alicuota2},
                        IdCuentaContablePercepcion = {entity.IdCuentaContablePercepcion},
                        Percepcion = {entity.Percepcion},
                        IdCuentaContablePercepcion2 = {entity.IdCuentaContablePercepcion2},
                        Percepcion2 = {entity.Percepcion2},
                        ImpuestosInternos = {entity.ImpuestosInternos},
                        Total = {entity.Total},
                        CAE = {entity.CAE},
                        VencimientoCAE = {entity.VencimientoCAE}
                     WHERE
	                    IdPlanillaGastos = {entity.IdPlanillaGastos} AND Item = {entity.Item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
    
}
