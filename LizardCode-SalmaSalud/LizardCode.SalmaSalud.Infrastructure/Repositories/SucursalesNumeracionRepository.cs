using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class SucursalesNumeracionRepository : ISucursalesNumeracionRepository
    {
        private readonly IDbContext _context;

        public SucursalesNumeracionRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdSucursal(int idSucursal, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM SucursalesNumeracion
                    WHERE IdSucursal = {idSucursal}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> DeleteByIdSucursalAndComprobante(int idSucursal, int idComprobante, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM SucursalesNumeracion
                    WHERE IdSucursal = {idSucursal} AND IdComprobante = {idComprobante}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<Domain.EntitiesCustom.SucursalNumeracion>> GetAllCustomSucursalesNumByIdSucursal(int idSucursal, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT
                                    sn.*,
                                    c.descripcion Comprobante
                                FROM SucursalesNumeracion sn
                                INNER JOIN Comprobantes c ON sn.IdComprobante = c.IdComprobante
                                INNER JOIN Sucursales s ON s.IdSucursal = sn.IdSucursal
                                    WHERE 
                                        sn.IdSucursal = {idSucursal} AND
                                        s.IdEmpresa = {idEmpresa}");

            return (await builder.QueryAsync<Domain.EntitiesCustom.SucursalNumeracion>()).AsList();
        }

        public async Task<SucursalNumeracion> GetByIdSucursalAndComprobante(int idSucursal, int idComprobante, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM SucursalesNumeracion
                    WHERE
	                    IdSucursal = {idSucursal}
	                    AND IdComprobante = {idComprobante}"
                );

            return await builder.QuerySingleAsync<SucursalNumeracion>(transaction);
        }

        public async Task<SucursalNumeracion> GetLastNumeroByComprobante(int idComprobante, int idSucursal, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT sn.* FROM SucursalesNumeracion sn
                        INNER JOIN Sucursales s ON sn.IdSucursal = s.IdSucursal
                    WHERE 
	                    sn.IdSucursal = {idSucursal} AND
	                    sn.IdComprobante = {idComprobante} AND
                        s.IdEmpresa = {idEmpresa}"
                );

            return await builder.QuerySingleOrDefaultAsync<SucursalNumeracion>(transaction);
        }

        public async Task<bool> Insert(SucursalNumeracion entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO SucursalesNumeracion
                    (
                        IdSucursal,
                        IdComprobante,
                        Numerador
                    )
                    VALUES
                    (
                        {entity.IdSucursal},
                        {entity.IdComprobante},
                        {entity.Numerador}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> Update(SucursalNumeracion entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE SucursalesNumeracion SET
	                    Numerador = {entity.Numerador}
                     WHERE
	                    IdSucursal = {entity.IdSucursal} AND IdComprobante = {entity.IdComprobante}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}
