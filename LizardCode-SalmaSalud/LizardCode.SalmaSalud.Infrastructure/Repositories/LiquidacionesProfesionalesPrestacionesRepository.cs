using Dapper;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class LiquidacionesProfesionalesPrestacionesRepository : BaseRepository, ILiquidacionesProfesionalesPrestacionesRepository
    {
        public LiquidacionesProfesionalesPrestacionesRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<LiquidacionProfesionalPrestacion>> GetAllByIdLiquidacionProfesional(long idLiquidacionProfesional, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    lpp.*
                    FROM LiquidacionesProfesionalesPrestaciones lpp
                    WHERE
                        lpp.idLiquidacion = {idLiquidacionProfesional}");

            var results = await builder.QueryAsync<LiquidacionProfesionalPrestacion>(transaction);

            return results.AsList();
        }

        public async Task<bool> RemoveByIdLiquidacionProfesional(long idLiquidacionProfesional, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM LiquidacionesProfesionalesPrestaciones
                    WHERE idLiquidacion = {idLiquidacionProfesional} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> RemoveById(long idLiquidacionProfesionalPrestacion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM LiquidacionesProfesionalesPrestaciones
                    WHERE idLiquidacionProfesionalPrestacion = {idLiquidacionProfesionalPrestacion} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}