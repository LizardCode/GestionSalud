using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
	public class RecibosRepository : BaseRepository, IRecibosRepository
    {
        public RecibosRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT r.*, c.RazonSocial Cliente FROM Recibos r LEFT JOIN Clientes c ON r.IdCliente = c.IdCliente");

            return base.GetAllCustomQuery(query);
        }

        public async Task<Custom.Recibo> GetByIdCustom(int idRecibo, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                                    r.*,
                                    c.RazonSocial Cliente
                                FROM Recibos r
                                LEFT JOIN Clientes c ON r.IdCliente = c.IdCliente
                                WHERE 
                                    r.IdRecibo = {idRecibo}");

            return await builder.QuerySingleAsync<Custom.Recibo>(transaction);
        }

		public async Task<DateTime?> GetMaxFechaRecibo(int idEmpresa)
		{
			var builder = _context.Connection
	            .QueryBuilder($@"SELECT
                                    MAX(Fecha) Fecha
                                FROM recibos r
                                WHERE
                                    r.IdEstadoRegistro NOT IN ({(int)EstadoRegistro.Eliminado}, {(int)EstadoRecibo.Anulado}) AND
                                    r.IdEmpresa = {idEmpresa}");

			return await builder.QuerySingleOrDefaultAsync<DateTime?>();
		}

		public async Task<DateTime?> GetMinFechaRecibo(int idEmpresa)
		{
			var builder = _context.Connection
	            .QueryBuilder($@"SELECT
                                    MIN(Fecha) Fecha
                                FROM recibos r
                                WHERE
                                    r.IdEstadoRegistro NOT IN ({(int)EstadoRegistro.Eliminado}, {(int)EstadoRecibo.Anulado}) AND
                                    r.IdEmpresa = {idEmpresa}");

			return await builder.QuerySingleOrDefaultAsync<DateTime?>();
		}

		public async Task<List<Custom.SubdiarioCobros>> GetSubdiarioCobros(Dictionary<string, object> filters)
		{
			var builder = _context.Connection
				.QueryBuilder($@"SELECT r.IdRecibo, r.Fecha, 'RECIBO' AS Comprobante, r.IdRecibo AS Numero, c.NombreFantasia Cliente, c.CUIT, r.Importe AS Total
                    FROM Recibos r
                    INNER JOIN Clientes c ON r.IdCliente = c.IdCliente
                ");

			builder.Where($"r.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

			if (filters.ContainsKey("IdEmpresa"))
				builder.Where($"r.IdEmpresa = {filters["IdEmpresa"]}");

			if (filters.ContainsKey("FechaDesde"))
				builder.Where($"r.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

			if (filters.ContainsKey("FechaHasta"))
				builder.Where($"r.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

			return (await builder.QueryAsync<Custom.SubdiarioCobros>()).AsList();
		}

		public async Task<List<Custom.SubdiarioCobrosDetalle>> GetSubdiarioCobrosDetalle(Dictionary<string, object> filters)
		{
			var builder = _context.Connection
				.QueryBuilder($@"SELECT 
                    r.IdRecibo, 
                    r.Fecha, 
                    'RECIBO' AS Comprobante, 
                    r.IdRecibo AS Numero, 
                    c.NombreFantasia Cliente, 
                    c.CUIT, 
                    r.Importe AS Total,
                    cc.CodigoCuenta, 
                    cc.Descripcion NombreCuenta, 
                    CASE WHEN ad.Debitos = 0 THEN ad.Creditos ELSE ad.Debitos END AS Importe
                FROM recibos r
                    INNER JOIN Clientes c ON (r.IdCliente = c.IdCliente)
                    INNER JOIN RecibosAsientos ra ON (ra.IdRecibo = r.IdRecibo)
                    INNER JOIN Asientos a ON (a.IdAsiento = ra.IdAsiento)
                    INNER JOIN AsientosDetalle ad ON (a.IdAsiento = ad.IdAsiento)
                    INNER JOIN CuentasContables cc ON (cc.IdCuentaContable = ad.IdCuentaContable)
                ");

			builder.Where($"r.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

			if (filters.ContainsKey("IdEmpresa"))
				builder.Where($"r.IdEmpresa = {filters["IdEmpresa"]}");

			if (filters.ContainsKey("IdRecibo"))
				builder.Where($"r.IdRecibo = {filters["IdRecibo"]}");

			if (filters.ContainsKey("FechaDesde"))
				builder.Where($"r.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

			if (filters.ContainsKey("FechaHasta"))
				builder.Where($"r.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

			return (await builder.QueryAsync<Custom.SubdiarioCobrosDetalle>()).AsList();
		}

        public async Task<List<Custom.SubdiarioImputaciones>> GetSubdiarioImputaciones(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT 
                    cc.CodigoCuenta, 
                    cc.Descripcion NombreCuenta, 
                    SUM(ad.Debitos) AS Debitos,
                    SUM(ad.Creditos) AS Creditos
                FROM recibos r
                    INNER JOIN RecibosAsientos ra ON (ra.IdRecibo = r.IdRecibo)
                    INNER JOIN Asientos a ON (a.IdAsiento = ra.IdAsiento)
                    INNER JOIN AsientosDetalle ad ON (a.IdAsiento = ad.IdAsiento)
                    INNER JOIN CuentasContables cc ON (cc.IdCuentaContable = ad.IdCuentaContable)
                    /**where**/
                    GROUP BY ad.IdCuentaContable, cc.CodigoCuenta, cc.Descripcion
                ");

            builder.Where($"r.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"r.IdEmpresa = {idEmpresa}");

            if (fechaDesde.HasValue)
                builder.Where($"r.Fecha >= {fechaDesde}");

            if (fechaHasta.HasValue)
                builder.Where($"r.Fecha <= {fechaHasta}");

            return (await builder.QueryAsync<Custom.SubdiarioImputaciones>()).AsList();
        }
    }
}
