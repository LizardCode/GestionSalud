using Dapper;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class CierreMesRepository : BaseRepository, ICierreMesRepository
    {
        public CierreMesRepository(IDbContext context) : base(context)
        {

        }

        public async Task<List<CierreMes>> GetAllByIdEjercicio(int id, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM CierreMes
                    WHERE
	                    IdEjercicio = {id} AND
                        IdEmpresa = {idEmpresa}"
                );

            var result = await builder.QueryAsync<CierreMes>();

            return result.AsList();
        }

        public async Task<Domain.Entities.CierreMes> GetByAnnoMesModulo(int idEjercicio, int anno, int mes, string modulo, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM CierreMes
                    WHERE
	                    IdEjercicio = {idEjercicio} AND
                        Anno = {anno} AND
                        Mes = {mes} AND
                        Modulo = {modulo} AND
                        IdEmpresa = {idEmpresa}"
                );

            return await builder.QueryFirstOrDefaultAsync<CierreMes>();
        }

        public async Task<bool> MesCerrado(int idEjercicio, DateTime fecha, string modulo, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM CierreMes
                    WHERE
	                    IdEjercicio = {idEjercicio} AND
                        Anno = {fecha.Year} AND
                        Mes = {fecha.Month} AND
                        Modulo = {modulo} AND
                        IdEmpresa = {idEmpresa}"
                );

            var result = await builder.QueryFirstOrDefaultAsync<CierreMes>();

            return result?.Cierre == Commons.Si.Description();
        }
    }
}
