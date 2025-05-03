using Dapper;
using Dapper.DataTables.Models;
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
    public class CuentasContablesRepository : BaseRepository, ICuentasContablesRepository
    {
        public CuentasContablesRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        cc.*, 
                        rc.CodigoRubro,
                        rc.Descripcion Rubro 
                    FROM CuentasContables cc 
                        INNER JOIN RubrosContables rc ON rc.idRubroContable = cc.idRubroContable
                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<IList<CuentaContable>> GetCuentasContablesByIdEmpresa(int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        cc.*
                    FROM CuentasContables cc 
                        WHERE
                            cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                            cc.IdEmpresa = {idEmpresa}
                    ORDER BY cc.CodigoCuenta
                ");

            return (await builder.QueryAsync<CuentaContable>(transaction)).AsList();
        }

        public async Task<CuentaContable> GetCuentaContablesByIdEmpresaAndCodObservacion(int idEmpresa, int idCodigoObservacion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        cc.*
                    FROM CuentasContables cc 
                        WHERE
                            cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                            cc.IdEmpresa = {idEmpresa} AND
                            cc.IdCodigoObservacion = {idCodigoObservacion}
                ");

            return await builder.QueryFirstOrDefaultAsync<CuentaContable>(transaction);
        }

        public async Task<CuentaContable> GetByIdBanco(int idBanco, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        cc.*
                    FROM CuentasContables cc 
                        WHERE
                            cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                            cc.IdEmpresa = {idEmpresa} AND
                            cc.IdCuentaContable IN (SELECT IdCuentaContable FROM bancos WHERE IdBanco = {idBanco})
                ");

            return await builder.QueryFirstOrDefaultAsync<CuentaContable>(transaction);
        }

        public async Task<List<Domain.EntitiesCustom.MayorCuentas>> GetMayorCuentas(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT DISTINCT
                        cc.IdCuentaContable,
                        cc.CodigoCuenta,
                        cc.Descripcion
                    FROM CuentasContables cc
                    INNER JOIN AsientosDetalle ad ON cc.IdCuentaContable = ad.IdCuentaContable
                    INNER JOIN Asientos a ON ad.IdAsiento = a.IdAsiento 
                    /**where**/
                ");

            builder.Where($"cc.IdEmpresa = {filters["IdEmpresa"]}");
            builder.Where($"cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

            if (filters.ContainsKey("FechaDesde"))
                builder.Where($"a.Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta"))
                builder.Where($"a.Fecha <= {DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("IdEjercicio"))
                builder.Where($"a.IdEjercicio = {filters["IdEjercicio"]}");

            if (filters.ContainsKey("IdCuentaDesde"))
                builder.Where($"ad.IdCuentaContable >= {filters["IdCuentaDesde"]}");

            if (filters.ContainsKey("IdCuentaHasta"))
                builder.Where($"ad.IdCuentaContable <= {filters["IdCuentaHasta"]}");

            return (await builder.QueryAsync<Domain.EntitiesCustom.MayorCuentas>()).AsList();
        }

        public async Task<CuentaContable> GetCuentaContablesByIdEmpresaAndDescripcion(int idEmpresa, string descripcionCuentaContable, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        cc.*
                    FROM cuentas_contables cc 
                        WHERE
                            cc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                            cc.IdEmpresa = {idEmpresa} AND
                            cc.Descripcion = {descripcionCuentaContable}
                ");

            return await builder.QueryFirstOrDefaultAsync<CuentaContable>(transaction);
        }
    }
}
