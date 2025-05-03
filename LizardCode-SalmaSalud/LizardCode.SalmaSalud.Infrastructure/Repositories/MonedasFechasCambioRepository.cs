using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class MonedasFechasCambioRepository : BaseRepository, IMonedasFechasCambioRepository
    {
        public MonedasFechasCambioRepository(IDbContext context) : base(context)
        {

        }

        public async Task<MonedaFechasCambio> GetByIdAndFecha(int idMoneda, DateTime fecha, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    mfc.*
                    FROM MonedasFechasCambio mfc
                    WHERE
                        mfc.IdMoneda = {idMoneda} AND
                        mfc.Fecha = {fecha} AND
                        mfc.IdEmpresa = {idEmpresa}
                    ");

            return await builder.QueryFirstOrDefaultAsync<MonedaFechasCambio>();
        }

        public async Task<IList<MonedaFechasCambio>> GetCotizaciones(int idMoneda, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    mfc.*
                    FROM MonedasFechasCambio mfc
                    WHERE
                        mfc.IdMoneda = {idMoneda} AND
                        mfc.IdEmpresa = {idEmpresa}
                    ORDER BY Fecha DESC
                    ");

            var result = await builder.QueryAsync<MonedaFechasCambio>();

            return result.AsList();
        }

        public async Task<double?> GetFechaCambio(int idMoneda, DateTime fecha, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    mfc.Cotizacion
                    FROM MonedasFechasCambio mfc
                    WHERE
                        mfc.IdMoneda = {idMoneda} AND
                        mfc.Fecha = {fecha} AND
                        mfc.IdEmpresa = {idEmpresa}
                    ");

            return await builder.QueryFirstOrDefaultAsync<double?>();
        }

        public async Task<double?> GetFechaCambioByCodigo(string idMoneda, DateTime fecha, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    mfc.Cotizacion
                    FROM MonedasFechasCambio mfc
                    INNER JOIN Monedas m ON mfc.IdMoneda = m.IdMoneda
                    WHERE
                        m.Codigo = {idMoneda} AND
                        mfc.Fecha = {fecha} AND
                        mfc.IdEmpresa = {idEmpresa}
                    ");

            return await builder.QueryFirstOrDefaultAsync<double?>();
        }

        public async Task<double?> GetFechaCambioByCodigo(string idMoneda, DateTime fecha, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    mfc.Cotizacion
                    FROM MonedasFechasCambio mfc
                    INNER JOIN Monedas m ON mfc.IdMoneda = m.IdMoneda
                    WHERE
                        m.Codigo = {idMoneda} AND
                        mfc.Fecha = {fecha} AND
                        mfc.IdEmpresa = {idEmpresa}
                    ");

            return await builder.QueryFirstOrDefaultAsync<double?>();
        }
    }
}
