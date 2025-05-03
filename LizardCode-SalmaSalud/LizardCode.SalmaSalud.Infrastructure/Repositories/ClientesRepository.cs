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
    public class ClientesRepository : BaseRepository, IClientesRepository, IDataTablesCustomQuery
    {
        public ClientesRepository(IDbContext context) : base(context)
        {
        }

        public async Task<List<Cliente>> GetAllClientesLookup()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						c.*
                    FROM Clientes c
                ")
                .Where($"c.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

            var result = await query.QueryAsync<Cliente>();

            return result.AsList();
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						c.*,
                        CASE WHEN ISNULL(c.CUIT, '') = '' THEN c.Documento ELSE c.CUIT END as DocumentoCUIT,
						ti.Descripcion AS TipoIVA
                    FROM Clientes c
					INNER JOIN TipoIVA ti
						ON ti.IdTipoIVA = c.IdTipoIVA
                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<Cliente> GetClienteByCUIT(string cuit, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT c.* FROM Clientes c ")
                .Where($"c.CUIT = {cuit}");

            return await query.QueryFirstOrDefaultAsync<Cliente>(transaction);
        }

        public async Task<bool> ValidarCUITExistente(string cuit, int? id, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT c.* FROM Clientes c
                ")
                .Where($"c.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"c.CUIT = {cuit}");

            if (id != null)
            {
                query.Where($"c.IdCliente <> {id}");
            }

            var result = await query.QueryAsync<Cliente>(transaction);

            return result.AsList().Count > 0;
        }
        public async Task<Cliente> GetClienteByDocumento(string documento, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT c.* FROM Clientes c ")
                .Where($"c.Documento = {documento}");

            return await query.QueryFirstOrDefaultAsync<Cliente>(transaction);
        }

        public async Task<bool> ValidarDocumentoExistente(string documento, int? id, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT c.* FROM Clientes c
                ")
                .Where($"c.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"c.Documento = {documento}");

            if (id != null)
            {
                query.Where($"c.IdCliente <> {id}");
            }

            var result = await query.QueryAsync<Cliente>(transaction);

            return result.AsList().Count > 0;
        }

        public async Task<Cliente> GetClienteByIdPaciente(int idPaciente, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT c.* FROM Clientes c ")
                .Where($"c.IdPaciente = {idPaciente}");

            return await query.QueryFirstOrDefaultAsync<Cliente>(transaction);
        }

        public async Task<Cliente> GetClienteByIdFinanciador(int idFinanciador, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT c.* FROM Clientes c ")
                .Where($"c.IdFinanciador = {idFinanciador}");

            return await query.QueryFirstOrDefaultAsync<Cliente>(transaction);
        }
    }
}
