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
    public class ArticulosRepository : BaseRepository, IArticulosRepository
    {
        public ArticulosRepository(IDbContext context) : base(context)
        {

        }

        public async Task<IList<Articulo>> GetAllByIdEmpresa(int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT a.* FROM Articulos a 
                        WHERE 
                            a.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                            a.IdEmpresa = {idEmpresa} ");

            return (await builder.QueryAsync<Articulo>()).AsList();
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT a.* FROM articulos a");

            return base.GetAllCustomQuery(query);
        }
    }
}
