using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ARBARepository : IARBARepository
    {
        private readonly IDbContext _context;

        public ARBARepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<ARBA> GetByCUITFechaVig(string regimen, string cuit, DateTime fecha, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM IngresosBrutos.ARBA
                    WHERE
                        Regimen = {regimen} AND
	                    CUIT = {cuit} AND
                        {fecha} BETWEEN FechaVigDesde AND FechaVigHasta"
                );

            return await builder.QueryFirstOrDefaultAsync<ARBA>(transaction);
        }
    }
}
