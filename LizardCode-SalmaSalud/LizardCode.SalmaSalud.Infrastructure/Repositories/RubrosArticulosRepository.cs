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
    public class RubrosArticulosRepository : BaseRepository, IRubrosArticulosRepository
    {
        public RubrosArticulosRepository(IDbContext context) : base(context)
        {

        }

        public async Task<IList<RubroArticulo>> GetAllByIdEmpresa(int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT ra.* FROM RubrosArticulos ra 
                                WHERE ra.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                    ra.IdEmpresa = {idEmpresa} ");

            return (await builder.QueryAsync<RubroArticulo>()).AsList();
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT ra.* FROM RubrosArticulos ra");

            return base.GetAllCustomQuery(query);
        }

    }
}
