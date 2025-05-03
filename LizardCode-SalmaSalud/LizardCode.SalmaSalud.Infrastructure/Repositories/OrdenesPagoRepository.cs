using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class OrdenesPagoRepository : BaseRepository, IOrdenesPagoRepository
    {
        public OrdenesPagoRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT op.*, p.RazonSocial Proveedor FROM OrdenesPago op LEFT JOIN Proveedores p ON op.IdProveedor = p.IdProveedor");

            return base.GetAllCustomQuery(query);
        }

        public async Task<Custom.OrdenPago> GetByIdCustom(int idOrdenPago, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                                    op.*,
                                    p.RazonSocial Proveedor
                                FROM OrdenesPago op
                                LEFT JOIN Proveedores p ON op.IdProveedor = p.IdProveedor ")
                .Where($"op.IdOrdenPago = {idOrdenPago}");

            return await builder.QuerySingleAsync<Custom.OrdenPago>(transaction);
        }

		public async Task<double?> GetTotalRetenido(int idTipoRetencion, DateTime fecha, int idProveedor, int idEmpresa, IDbTransaction transaction = null)
        {
            var fechaDesde = new DateTime(fecha.Year, fecha.Month, 1);
            var fechaHasta = fechaDesde.AddMonths(1).AddDays(-1);

            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                                    SUM(opr.Importe * CASE WHEN op.MonedaPago = {Monedas.MonedaLocal.Description()} THEN 1 ELSE op.Cotizacion END) Retencion
                                FROM OrdenesPago op
                                LEFT JOIN OrdenesPagoRetenciones opr ON op.IdOrdenPago = opr.IdOrdenPago ")
                .Where($"opr.IdTipoRetencion = {idTipoRetencion}")
                .Where($"op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"op.IdProveedor = {idProveedor}")
                .Where($"op.IdEmpresa = {idEmpresa}")
                .Where($"op.Fecha BETWEEN {fechaDesde} AND {fechaHasta}");
            
            return await builder.ExecuteScalarAsync<double?>(transaction);
        }

		public async Task<List<Custom.SubdiarioPagos>> GetSubdiarioPagos(Dictionary<string, object> filters)
		{
			var builder = _context.Connection
				.QueryBuilder($@"SELECT 
                        op.IdOrdenPago, op.Fecha, 'ORDEN DE PAGO' AS Comprobante, op.IdOrdenPago AS Numero, p.RazonSocial Proveedor, p.CUIT, op.Importe AS Total
                    FROM OrdenesPago op
                    INNER JOIN Proveedores p ON op.IdProveedor = p.IdProveedor
                ");

			builder.Where($"op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

			if (filters.ContainsKey("IdEmpresa"))
				builder.Where($"op.IdEmpresa = {filters["IdEmpresa"]}");

			if (filters.ContainsKey("FechaDesde"))
				builder.Where($"op.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

			if (filters.ContainsKey("FechaHasta"))
				builder.Where($"op.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

			return (await builder.QueryAsync<SubdiarioPagos>()).AsList();
		}

		public async Task<List<Custom.SubdiarioPagosDetalle>> GetSubdiarioPagosDetalle(Dictionary<string, object> filters)
		{
			var builder = _context.Connection
				.QueryBuilder($@"SELECT 
                    op.IdOrdenPago, 
                    op.Fecha, 
                    'ORDEN DE PAGO' AS Comprobante, 
                    op.IdOrdenPago AS Numero, 
                    p.RazonSocial Proveedor, 
                    op.Importe AS Total,
		            cuc.CodigoCuenta, 
                    cuc.Descripcion NombreCuenta, 
                    CASE WHEN ad.Debitos = 0 THEN ad.Creditos ELSE ad.Debitos END AS Importe
                FROM OrdenesPago op
                    INNER JOIN Proveedores p ON (op.IdProveedor = p.IdProveedor)
                    INNER JOIN OrdenesPagoAsientos opa ON (opa.IdOrdenPago = op.IdOrdenPago)
                    INNER JOIN Asientos a ON (a.IdAsiento = opa.IdAsiento)
                    INNER JOIN AsientosDetalle ad ON (a.IdAsiento = ad.IdAsiento)
                    INNER JOIN CuentasContables cuc ON (cuc.IdCuentaContable = ad.IdCuentaContable)
                ");

			builder.Where($"op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

			if (filters.ContainsKey("IdEmpresa"))
				builder.Where($"op.IdEmpresa = {filters["IdEmpresa"]}");

			if (filters.ContainsKey("IdOrdenPago"))
				builder.Where($"op.IdOrdenPago = {filters["IdOrdenPago"]}");

			if (filters.ContainsKey("FechaDesde"))
				builder.Where($"op.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

			if (filters.ContainsKey("FechaHasta"))
				builder.Where($"op.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

			return (await builder.QueryAsync<Custom.SubdiarioPagosDetalle>()).AsList();
		}

        public async Task<int> GetCantidadOrdenesPagoProveedor(int idProveedor, int idEmpresa)
        {
            FormattableString sql = $@"SELECT COUNT(*)
                                        FROM OrdenesPago op
                                        WHERE 
                                            op.IdEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND
                                            op.IdEmpresa = {idEmpresa} AND
                                            op.IdProveedor = {idProveedor}  AND
                                            op.Fecha BETWEEN DATE_ADD(NOW(), INTERVAL -60 DAY) AND NOW() ";

            var builder = _context.Connection.QueryBuilder(sql);
            var result = await builder.ExecuteScalarAsync<int>();

            return result;
        }
        public DataTablesCustomQuery GetUltimasOrdenesPagoProveedor(int idProveedor, int idEmpresa)
        {
            FormattableString sql = $@"SELECT TOP 10 op.Fecha, op.Importe 
                                        FROM OrdenesPago op 
                                        WHERE op.idEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND 
                                                op.IdEmpresa = {idEmpresa} AND 
                                                op.IdProveedor = {idProveedor}  
                                        ORDER BY op.IdOrdenPago DESC 
                                        LIMIT 10 ";

            var builder = _context.Connection.QueryBuilder(sql);

            return base.GetAllCustomQuery(builder);
        }

        public async Task<List<Custom.OrdenPago>> GetOrdenesPagoProveedor(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT op.* FROM OrdenesPago op ");

            builder.Where($"op.IdEmpresa = {filters["IdEmpresa"]}");
            builder.Where($"op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"op.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"op.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("IdProveedor"))
                builder.Where($"op.IdProveedor = {filters["IdProveedor"]}");

            return (await builder.QueryAsync<Custom.OrdenPago>()).AsList();
        }

        public async Task<List<Custom.SubdiarioImputaciones>> GetSubdiarioImputaciones(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT 
		            cuc.CodigoCuenta, 
                    cuc.Descripcion NombreCuenta, 
                    SUM(ad.Debitos) AS Debitos,
                    SUM(ad.Creditos) AS Creditos
                FROM OrdenesPago op
                    INNER JOIN OrdenesPagoAsientos opa ON (opa.IdOrdenPago = op.IdOrdenPago)
                    INNER JOIN Asientos a ON (a.IdAsiento = opa.IdAsiento)
                    INNER JOIN AsientosDetalle ad ON (a.IdAsiento = ad.IdAsiento)
                    INNER JOIN CuentasContables cuc ON (cuc.IdCuentaContable = ad.IdCuentaContable)
                    /**where**/
                    GROUP BY ad.IdCuentaContable, cuc.CodigoCuenta, cuc.Descripcion
                ");

            builder.Where($"op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"op.IdEmpresa = {idEmpresa}");

            if (fechaDesde.HasValue)
                builder.Where($"op.Fecha >= {fechaDesde}");

            if (fechaHasta.HasValue)
                builder.Where($"op.Fecha <= {fechaHasta}");

            return (await builder.QueryAsync<Custom.SubdiarioImputaciones>()).AsList();
        }

        public async Task<List<dynamic>> GetOrdenesPagoByCheque(int idCheque)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
		                op.IdOrdenPago, 
                        p.RazonSocial Proveedor, 
                        p.CUIT
                    FROM OrdenesPagoDetalle opd
                        INNER JOIN OrdenesPago op ON opd.IdOrdenPago = op.IdOrdenPago
                        INNER JOIN Proveedores p ON op.IdProveedor = p.IdProveedor
                    ");

            builder.Where($"op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"opd.IdCheque = {idCheque}");

            return (await builder.QueryAsync<dynamic>()).AsList();
        }

		public async Task<DateTime?> GetMinFechaOrdenPago(int idEmpresa)
		{
			var builder = _context.Connection
				.QueryBuilder($@"SELECT
                                    MIN(Fecha) Fecha
                                FROM OrdenesPago op
                                WHERE
                                    op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                    op.IdEmpresa = {idEmpresa}");

			return await builder.QuerySingleOrDefaultAsync<DateTime?>();
		}

		public async Task<DateTime?> GetMaxFechaOrdenPago(int idEmpresa)
		{
			var builder = _context.Connection
				.QueryBuilder($@"SELECT
                                    MAX(Fecha) Fecha
                                FROM OrdenesPago op
                                WHERE
                                    op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                    op.IdEmpresa = {idEmpresa}");

			return await builder.QuerySingleOrDefaultAsync<DateTime?>();
		}
	}
}