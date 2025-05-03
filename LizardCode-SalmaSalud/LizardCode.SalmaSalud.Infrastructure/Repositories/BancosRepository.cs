using Dapper;
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
    public class BancosRepository : BaseRepository, IBancosRepository
    {
        public BancosRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT b.* FROM Bancos b");

            return base.GetAllCustomQuery(query);
        }

        public async Task<IList<Banco>> GetBancosByIdEmpresa(int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                                SELECT
	                                ba.*
                                FROM Bancos ba
                                WHERE
                                    ba.IdEmpresa = {idEmpresa}");

            var results = await builder.QueryAsync<Banco>();

            return results.AsList();
        }

        public async Task<bool> UpdateEsDefault(bool esDefault, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE Bancos SET
	                    EsDefault = {esDefault}
                     WHERE
	                    IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}
