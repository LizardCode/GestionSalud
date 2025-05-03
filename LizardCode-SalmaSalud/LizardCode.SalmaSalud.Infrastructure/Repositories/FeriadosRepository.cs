using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class FeriadosRepository : BaseRepository, IFeriadosRepository
    {
        public FeriadosRepository(IDbContext context) : base(context)
        {

        }

        public async Task<List<Feriado>> GetAllByIdEmpresa(int idEmpresa)
        {
            var builder = _context.Connection
                .FluentQueryBuilder()
                .Select($"*")
                .From($"Feriados")
                .Where($"IdEmpresa = {idEmpresa}")
                .OrderBy($"IdFeriado DESC");


            var result = await builder.QueryAsync<Feriado>();

            return result.AsList();
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT F.*,
                                        TF.Descripcion as TipoFeriado
                                 FROM Feriados F
                                 INNER JOIN TipoFeriado TF ON F.IdTipoFeriado = TF.IdTipoFeriado ");

            return base.GetAllCustomQuery(query);
        }
    }
}
