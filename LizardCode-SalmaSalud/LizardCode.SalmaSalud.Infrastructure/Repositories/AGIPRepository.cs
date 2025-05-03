using Dapper;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class AGIPRepository : IAGIPRepository
    {
        private readonly IDbContext _context;

        public AGIPRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<AGIP> GetByCUITFechaVig(string cuit, DateTime fecha, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM IngresosBrutos.AGIP
                    WHERE
	                    CUIT = {cuit} AND
                        {fecha} BETWEEN FechaVigDesde AND FechaVigHasta"
                );

            return await builder.QueryFirstOrDefaultAsync<AGIP>(transaction);
        }
    }
}
