using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class OrdenesPagoRetencionesRepository : BaseRepository, IOrdenesPagoRetencionesRepository
    {
        public OrdenesPagoRetencionesRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> DeleteByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM OrdenesPagoRetenciones
                    WHERE 
                        IdOrdenPago = {idOrdenPago} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<List<OrdenPagoRetencion>> GetAllByIdOrdenPago(int idOrdenPago, IDbTransaction tran = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM OrdenesPagoRetenciones
                    WHERE
                        IdOrdenPago = {idOrdenPago}");

            var results = await builder.QueryAsync<OrdenPagoRetencion>();

            return results.AsList();
        }

        public async Task<List<Custom.OrdenPagoRetencion>> GetCustomByIdOrdenPago(int idOrdenPago, IDbTransaction tran = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        opr.*,
                        IdOrdenPagoRetencion NroRetencion,
                        tr.Descripcion TipoRetencion
                    FROM OrdenesPagoRetenciones opr
                    INNER JOIN TipoRetencion tr ON opr.IdTipoRetencion = tr.IdTipoRetencion
                    WHERE
                        opr.IdOrdenPago = {idOrdenPago}");

            var results = await builder.QueryAsync<Custom.OrdenPagoRetencion>();

            return results.AsList();
        }
    }
}
