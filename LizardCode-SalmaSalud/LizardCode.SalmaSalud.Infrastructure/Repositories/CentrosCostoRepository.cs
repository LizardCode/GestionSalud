using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class CentrosCostoRepository : BaseRepository, ICentrosCostoRepository
    {
        public CentrosCostoRepository(IDbContext context) : base(context)
        {

        }

        public async Task<IList<CentroCosto>> GetAllByIdEmpresa(int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT c.* FROM CentrosCosto c
                    WHERE
                        c.IdEmpresa = {idEmpresa} AND
                        c.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

            var results = await builder.QueryAsync<CentroCosto>();

            return results.AsList();
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT cc.* FROM CentrosCosto cc");

            return base.GetAllCustomQuery(query);
        }

    }
}
