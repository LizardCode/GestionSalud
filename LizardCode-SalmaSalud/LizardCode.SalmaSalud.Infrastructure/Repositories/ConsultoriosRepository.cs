using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ConsultoriosRepository : BaseRepository, IConsultoriosRepository
    {
        public ConsultoriosRepository(IDbContext context) : base(context)
        {

        }

        public async Task<List<Consultorio>> GetAllByIdEmpresa(int idEmpresa)
        {
            var builder = _context.Connection
                .FluentQueryBuilder()
                .Select($"*")
                .From($"Consultorios")
                .Where($"IdEmpresa = {idEmpresa}")
                .OrderBy($"IdConsultorio DESC");


            var result = await builder.QueryAsync<Consultorio>();

            return result.AsList();
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT C.*
                                 FROM Consultorios C ");

            return base.GetAllCustomQuery(query);
        }
    }
}