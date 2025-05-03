using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class AsientosDetalleRepository : IAsientosDetalleRepository
    {
        private readonly IDbContext _context;

        public AsientosDetalleRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdAsiento(int idAsiento, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM AsientosDetalle
                    WHERE IdAsiento = {idAsiento}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> DeleteByIdAsientoAndItem(int idAsiento, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM AsientosDetalle
                    WHERE IdAsiento = {idAsiento} AND Item = {item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<Domain.EntitiesCustom.AsientoDetalle>> GetAllByIdAsiento(int idAsiento, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ad.*,
                        a.Fecha,
                        cc.CodigoCuenta Codigo,
                        cc.Descripcion Cuenta
                    FROM AsientosDetalle ad
                    INNER JOIN Asientos a ON a.IdAsiento = ad.IdAsiento
                    INNER JOIN CuentasContables cc ON ad.IdCuentaContable = cc.IdCuentaContable
                    WHERE
                        ad.IdAsiento = {idAsiento}");

            var results = await builder.QueryAsync<Domain.EntitiesCustom.AsientoDetalle>(transaction);

            return results.AsList();
        }

        public async Task<AsientoDetalle> GetByIdAsientoAndItem(int idAsiento, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM AsientosDetalle
                    WHERE
	                    IdAsiento = {idAsiento}
	                    AND Item = {item}"
                );

            return await builder.QuerySingleAsync<AsientoDetalle>(transaction);
        }

        public async Task<List<Domain.EntitiesCustom.MayorCuentasDetalle>> GetMayorCuentasDetalle(int idCuentaContable, DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                        a.Fecha, 
                        a.Descripcion, 
                        ad.Debitos, 
                        ad.Creditos
                    FROM AsientosDetalle ad
                    INNER JOIN Asientos a ON ad.IdAsiento = a.IdAsiento
                    WHERE
                        ad.IdCuentaContable = {idCuentaContable} AND
                        a.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                        a.IdEmpresa = {idEmpresa} AND
                        a.Fecha >= {fechaDesde ?? DateTime.Now} AND
                        a.Fecha <= {fechaHasta ?? DateTime.Now}");

            return (await builder.QueryAsync<Domain.EntitiesCustom.MayorCuentasDetalle>()).AsList();
        }

        public async Task<double> GetMayorCuentasDetalleSdoInicio(int idCuentaContable, DateTime? fechaDesde, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT ISNULL(SUM(ad.Debitos - ad.Creditos), 0) AS SaldoInicio
                    FROM AsientosDetalle ad
                    INNER JOIN Asientos a ON ad.IdAsiento = a.IdAsiento
                    WHERE
                        ad.IdCuentaContable = {idCuentaContable} AND
                        a.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                        a.IdEmpresa = {idEmpresa} AND
                        a.Fecha < {fechaDesde ?? DateTime.Now}
            ");

            return await builder.QueryFirstOrDefaultAsync<double>();
        }

        public async Task<bool> Insert(AsientoDetalle entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO AsientosDetalle
                    (
                        IdAsiento,
                        Item,
                        IdCuentaContable,
                        Detalle,
                        Debitos,
                        Creditos
                    )
                    VALUES
                    (
                        {entity.IdAsiento},
                        {entity.Item},
                        {entity.IdCuentaContable},
                        {entity.Detalle},
                        {entity.Debitos},
                        {entity.Creditos}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> Update(AsientoDetalle entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE AsientosDetalle SET
	                    IdCuentaContable = {entity.IdCuentaContable},
	                    Detalle = {entity.Detalle},
                        Debitos = {entity.Debitos},
	                    Creditos = {entity.Creditos}
                     WHERE
	                    IdAsiento = {entity.IdAsiento} AND Item = {entity.Item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<List<Domain.EntitiesCustom.BalancePatrimonial>> GetBalanceCuentasByRubros(List<int> rubros, int idEmpresa, int idEjercicio, DateTime fechaHasta)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT  
                        '' Rubro,
                        cc.CodigoCuenta CodigoIntegracion,
                        cc.IdCuentaContable NumeroCuenta,
                        cc.Descripcion,
                        ISNULL(SUM(ad.Debitos - ad.Creditos), 0) AS Saldo
                    FROM CuentasContables cc
                        LEFT JOIN AsientosDetalle ad ON ad.IdCuentaContable = cc.IdCuentaContable
                        LEFT JOIN Asientos a ON ad.IdAsiento = a.IdAsiento
                    WHERE
                        cc.IdRubroContable IN {rubros} AND
                        a.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                        a.IdEjercicio = {idEjercicio} AND
                        a.IdEmpresa = {idEmpresa} AND
                        a.Fecha <= {fechaHasta}
                    GROUP BY cc.IdCuentaContable, cc.CodigoCuenta, cc.Descripcion
                    ORDER BY cc.CodigoCuenta
            ");

            var result = await builder.QueryAsync<Domain.EntitiesCustom.BalancePatrimonial>();

            return result.AsList();
        }

        public async Task<List<Domain.EntitiesCustom.EstadoResultado>> GetEstadoResultadosByRubros(List<int> rubros, int idEmpresa, int idEjercicio, DateTime fechaHasta)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT  
                        '' Rubro,
                        cc.CodigoCuenta CodigoIntegracion,
                        cc.IdCuentaContable NumeroCuenta,
                        cc.Descripcion,
                        ISNULL(SUM(ad.Debitos - ad.Creditos), 0) AS Saldo
                    FROM CuentasContables cc
                        LEFT JOIN AsientosDetalle ad ON ad.IdCuentaContable = cc.IdCuentaContable
                        LEFT JOIN Asientos a ON ad.IdAsiento = a.IdAsiento
                    WHERE
                        cc.IdRubroContable IN {rubros} AND
                        a.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                        a.IdEjercicio = {idEjercicio} AND
                        a.IdEmpresa = {idEmpresa} AND
                        a.Fecha <= {fechaHasta}
                    GROUP BY cc.IdCuentaContable, cc.CodigoCuenta, cc.Descripcion
                    ORDER BY cc.CodigoCuenta
            ");

            var result = await builder.QueryAsync<Domain.EntitiesCustom.EstadoResultado>();

            return result.AsList();
        }

        public async Task<List<Domain.EntitiesCustom.BalanceSumSdo>> GetBalanceSumSdoByIdEmpresa(int idEmpresa, int idEjercicio, DateTime fechaDesde, DateTime fechaHasta, int? idCuentaDesde, int? idCuentaHasta)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT  
                        cc.IdCuentaContable,
                        cc.CodigoCuenta CodigoIntegracion,
                        cc.Descripcion,
                        ISNULL(SUM(ad.Debitos), 0) AS Debe,
                        ISNULL(SUM(ad.Creditos), 0) AS Haber,
                        '0' AS Deudor,
                        '0' AS Acredor
                    FROM CuentasContables cc
                        LEFT JOIN AsientosDetalle ad ON ad.IdCuentaContable = cc.IdCuentaContable
                        LEFT JOIN Asientos a ON ad.IdAsiento = a.IdAsiento
                    /**where**/
                    GROUP BY cc.IdCuentaContable, cc.CodigoCuenta, cc.Descripcion
                    ORDER BY cc.CodigoCuenta
            ");

            builder.Where($"cc.IdEmpresa = {idEmpresa}");
            builder.Where($"cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"a.Fecha >= {fechaDesde}");
            builder.Where($"a.Fecha <= {fechaHasta}");

            if (idCuentaDesde.HasValue)
                builder.Where($"cc.IdCuentaContable >= {idCuentaDesde.Value}");

            if (idCuentaHasta.HasValue)
                builder.Where($"cc.IdCuentaContable <= {idCuentaHasta.Value}");

            var result = await builder.QueryAsync<Domain.EntitiesCustom.BalanceSumSdo>();

            return result.AsList();
        }
    }
}
