using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class TiposAsientosRepository : BaseRepository, ITiposAsientosRepository
    {
        public TiposAsientosRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT * FROM TipoAsientos");

            return base.GetAllCustomQuery(query);
        }

        public async Task<IList<TipoAsiento>> GetTiposAsientoByIdEmpresa(int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ta.*
                    FROM TipoAsientos ta
                    WHERE ta.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} 
                        AND ta.IdEmpresa = {idEmpresa}");

            var results = await builder.QueryAsync<TipoAsiento>();

            return results.AsList();
        }

        public async Task<List<TipoAsientoCuenta>> GetItemsByIdTipoAsiento(int idTipoAsiento, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    tac.*
                    FROM TipoAsientosCuentas tac
                    WHERE
	                    tac.IdTipoAsiento = {idTipoAsiento} "
                );

            var result = await builder.QueryAsync<TipoAsientoCuenta>(transaction);

            return result.ToList();
        }
    }
}
