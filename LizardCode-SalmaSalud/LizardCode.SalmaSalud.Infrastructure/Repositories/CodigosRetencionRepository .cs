using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class CodigosRetencionRepository : BaseRepository, ICodigosRetencionRepository
    {
        public CodigosRetencionRepository(IDbContext context) : base(context)
        {
        }

        public async Task<Domain.EntitiesCustom.CodigosRetencion> GetByIdCustom(int idCodigoRetencion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        cr.*, 
                        tr.Descripcion TipoRetencion 
                    FROM CodigosRetencion cr 
                        INNER JOIN TipoRetencion tr ON cr.IdTipoRetencion = tr.IdTipoRetencion
                    WHERE
                        cr.IdCodigoRetencion = {idCodigoRetencion}");

            return await builder.QuerySingleAsync<Domain.EntitiesCustom.CodigosRetencion>(transaction);
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        cr.*, 
                        tr.Descripcion TipoRetencion 
                    FROM CodigosRetencion cr 
                        INNER JOIN TipoRetencion tr ON cr.IdTipoRetencion = tr.IdTipoRetencion");

            return base.GetAllCustomQuery(query);
        }

        public async Task<IList<CodigosRetencion>> GetAllByTipo<CodigosRetencion>(int? idTipoRetencion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    cr.*
                    FROM CodigosRetencion cr");

            if(idTipoRetencion != null)
                builder.Where($"cr.IdTipoRetencion = {idTipoRetencion}");

            var results = await builder.QueryAsync<CodigosRetencion>(transaction);

            return results.AsList();
        }
    }
}
