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
    public class FinanciadoresRepository : BaseRepository, IFinanciadoresRepository, IDataTablesCustomQuery
    {
        public FinanciadoresRepository(IDbContext context) : base(context)
        {
        }

        public async Task<List<Financiador>> GetAllFinanciadoresLookup()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						c.*
                    FROM Financiadores c
                ")
                .Where($"c.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

            var result = await query.QueryAsync<Financiador>();

            return result.AsList();
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						c.*,
						ti.Descripcion AS TipoIVA
                    FROM Financiadores c
					INNER JOIN TipoIVA ti
						ON ti.IdTipoIVA = c.IdTipoIVA
                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<Financiador> GetFinanciadorByCUIT(string cuit, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT c.* FROM Financiadores c ")
                .Where($"c.CUIT = {cuit}");

            return await query.QueryFirstOrDefaultAsync<Financiador>(transaction);
        }

        public async Task<bool> ValidarCUITExistente(string cuit, int? id, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT c.* FROM Financiadores c
                ")
                .Where($"c.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"c.CUIT = {cuit}");

            if (id != null)
            {
                query.Where($"c.IdFinanciador <> {id}");
            }

            var result = await query.QueryAsync<Financiador>(transaction);

            return result.AsList().Count > 0;
        }
    }
}
