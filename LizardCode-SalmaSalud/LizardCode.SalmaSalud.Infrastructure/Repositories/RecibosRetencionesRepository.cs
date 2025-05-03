using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class RecibosRetencionesRepository : BaseRepository, IRecibosRetencionesRepository
    {
        public RecibosRetencionesRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> DeleteByIdRecibo(int idRecibo, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM RecibosRetenciones
                    WHERE 
                        IdRecibo = {idRecibo} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<List<ReciboRetencion>> GetAllByIdRecibo(int idRecibo)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM RecibosRetenciones
                    WHERE
                        IdRecibo = {idRecibo}");

            var results = await builder.QueryAsync<ReciboRetencion>();

            return results.AsList();
        }
    }
}
