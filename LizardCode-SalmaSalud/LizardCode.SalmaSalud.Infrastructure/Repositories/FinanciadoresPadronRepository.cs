using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class FinanciadoresPadronRepository : BaseRepository, IFinanciadoresPadronRepository
    {
        public FinanciadoresPadronRepository(IDbContext context) : base(context)
        {
        }

        public async Task<FinanciadorPadron> GetByIdFinanciadorAndDocumento(long idFinanciador, string documento, IDbTransaction transaction = null)
        {
            var exists = _context.Connection.ExecuteScalar<bool>($"SELECT COUNT(1) FROM FinanciadoresPadron WHERE idFinanciador = { idFinanciador } ");

            if (exists)
            {
                var builder = _context.Connection
                    .QueryBuilder($@"
                        SELECT
	                        fp.*
                        FROM FinanciadoresPadron fp
                        WHERE                        
                            fp.IdFinanciador = {idFinanciador}
                            AND fp.documento = {documento}");

                return await builder.QueryFirstOrDefaultAsync<FinanciadorPadron>(transaction);
            }
            else return new FinanciadorPadron();
        }

        public async Task<bool> RemoveByIdFinanciador(long idFinanciador, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM FinanciadoresPadron                    
                    WHERE IdFinanciador = {idFinanciador} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results >= 0);
        }

        public async Task<bool> RemoveById(long idFinanciadorPadron, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM FinanciadoresPadron
                    WHERE idFinanciadorPadron = {idFinanciadorPadron} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT P.*
                                 FROM FinanciadoresPadron P ");

            return base.GetAllCustomQuery(query);
        }
    }
}