using Dapper;
using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class RubrosContablesRepository : BaseRepository, IRubrosContablesRepository, IDataTablesCustomQuery
    {
        public RubrosContablesRepository(IDbContext context) : base(context)
        {
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						rc.*,
						rcp.Descripcion AS RubroPadre
                    FROM RubrosContables rc
					LEFT JOIN RubrosContables rcp
						ON rc.IdRubroPadre = rcp.IdRubroContable
                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<IList<RubroContable>> GetRubrosContablesByIdEmpresa(int idEmpresa)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						rc.*
                    FROM RubrosContables rc
                    WHERE 
                        rc.IdEmpresa = {idEmpresa} AND
                        rc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}
                    ORDER BY rc.CodigoRubro
                ");

            var result = await query.QueryAsync<RubroContable>();

            return result.AsList();
        }

        public async Task<List<Domain.EntitiesCustom.Select2Custom>> GetRubrosContablesByIdEmpresaAndTerm(int idEmpresa, string term)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"r.IdRubroContable Id")
                .Select($"(r.CodigoRubro + ' - ' + r.Descripcion) Text")
                .From($"RubrosContables r")
                .Where($"r.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"r.Descripcion LIKE {string.Concat("%", term, "%")}")
                .Where($"r.IdEmpresa = {idEmpresa}");


            var result = await query.QueryAsync<Domain.EntitiesCustom.Select2Custom>();

            return result.AsList();
        }

        public async Task<IList<RubroContable>> GetRubrosContablesByIdEmpresa(int idEmpresa, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						rc.*
                    FROM rubros_contables rc
                    WHERE 
                        rc.IdEmpresa = {idEmpresa} AND
                        rc.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}
                    ORDER BY rc.CodigoRubro
                ");

            var result = await query.QueryAsync<RubroContable>(transaction);

            return result.AsList();
        }
    }
}
