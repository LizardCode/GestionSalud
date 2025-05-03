using Dapper;
using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
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
    public class ProveedoresRepository : BaseRepository, IProveedoresRepository, IDataTablesCustomQuery
    {
        public ProveedoresRepository(IDbContext context) : base(context)
        {
            
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						p.*,
						ti.Descripcion AS TipoIVA
                    FROM Proveedores p
					INNER JOIN TipoIVA ti
						ON ti.IdTipoIVA = p.IdTipoIVA
                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<List<Proveedor>> GetAllProveedoresByIdEmpresaLookup(int idEmpresa)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						p.*
                    FROM Proveedores p
                ")
                .Where($"p.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"(p.IdProveedor IN (SELECT IdProveedor FROM ProveedoresEmpresas WHERE IdEmpresa = {idEmpresa}) OR p.IdProfesional > 0)");

            var result = await query.QueryAsync<Proveedor>();

            return result.AsList();
        }

        public async Task<Proveedor> GetByCUIT(string cuit, int idEmpresa, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                        SELECT
						    p.*
                        FROM Proveedores p
                            INNER JOIN ProveedoresEmpresas pe ON p.IdProveedor = pe.IdProveedor
                        WHERE
                            p.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                            pe.IdEmpresa = {idEmpresa} AND
                            p.CUIT = {cuit}");

            return await query.QuerySingleOrDefaultAsync<Proveedor>(transaction);
        }

        public async Task<Proveedor> GetProveedorByCUIT(string cuit, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT p.* FROM Proveedores p ")
                .Where($"p.CUIT = {cuit}");

            return await query.QueryFirstOrDefaultAsync<Proveedor>(transaction);
        }

        public async Task<List<CodigosRetencion>> GetCodigosRetencionByIdProveedor(int idProveedor, IDbTransaction tran)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						cr.*
                    FROM CodigosRetencion cr
                    INNER JOIN ProveedoresCodigosRetencion pcr ON pcr.IdCodigoRetencion = cr.IdCodigoRetencion
                ")
                .Where($"cr.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"pcr.IdProveedor = {idProveedor}");

            var result = await query.QueryAsync<CodigosRetencion>(tran);

            return result.AsList();
        }

        public async Task<bool> ValidarCUITExistente(string cuit, int? id, int idEmpresa, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT p.* FROM Proveedores p
                        INNER JOIN ProveedoresEmpresas pe ON p.IdProveedor = pe.IdProveedor
                ")
                .Where($"p.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"pe.IdEmpresa = {idEmpresa}")
                .Where($"p.CUIT = {cuit}");

            if (id != null)
            {
                query.Where($"p.IdProveedor <> {id}");
            }

            var result = await query.QueryAsync<Proveedor>(transaction);

            return result.AsList().Count > 0;
        }

        public async Task<Proveedor> GetProveedorByIdProfesional(int idProfesional, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT p.* FROM Proveedores p ")
                .Where($"p.IdProfesional = {idProfesional}");

            return await query.QueryFirstOrDefaultAsync<Proveedor>(transaction);
        }
    }
}
