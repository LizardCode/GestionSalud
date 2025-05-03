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
    public class ProveedoresEmpresasRepository : BaseRepository, IProveedoresEmpresasRepository
    {
        public ProveedoresEmpresasRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<ProveedorEmpresa>> GetAllByIdProveedor(int idProveedor, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    pe.*
                    FROM ProveedoresEmpresas pe
                    WHERE
                        pe.IdProveedor = {idProveedor}");

            var results = await builder.QueryAsync<ProveedorEmpresa>(transaction);

            return results.AsList();
        }

        public async Task<ProveedorEmpresa> GetByIdProveedorAndEmpresa(int idProveedor, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    pe.*
                    FROM ProveedoresEmpresas pe
                    WHERE
                        pe.IdProveedor = {idProveedor} AND
                        pe.IdEmpresa = {idEmpresa}");

            return await builder.QueryFirstOrDefaultAsync<ProveedorEmpresa>(transaction);
        }

        public async Task<bool> RemoveByIdProveedor(int idProveedor, int idUsuario, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE pe FROM ProveedoresEmpresas pe
                        INNER JOIN UsuariosEmpresas ue ON pe.IdEmpresa = ue.IdEmpresa
                    WHERE 
                        pe.IdProveedor = {idProveedor} AND
                        ue.IdUsuario = {idUsuario}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}