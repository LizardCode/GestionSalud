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
    public class FinanciadoresPrestacionesProfesionalesRepository : BaseRepository, IFinanciadoresPrestacionesProfesionalesRepository
    {
        public FinanciadoresPrestacionesProfesionalesRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<FinanciadorPrestacionProfesional>> GetAllByIdFinanciadorPrestacion(long idFinanciadorPrestacion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    fpp.*
                    FROM FinanciadoresPrestacionesProfesionales fpp
                    WHERE
                        fpp.IdFinanciadorPrestacion = {idFinanciadorPrestacion}");

            var results = await builder.QueryAsync<FinanciadorPrestacionProfesional>(transaction);

            return results.AsList();
        }

        public async Task<bool> RemoveByIdFinanciadorPrestacion(long idFinanciadorPrestacion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM FinanciadoresPrestacionesProfesionales
                    WHERE IdFinanciadorPrestacion = {idFinanciadorPrestacion} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<FinanciadorPrestacionProfesional> GetByIdPrestacionAndProfesional(int idFinanciadorPrestacion, int idProfesional, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT p.* FROM FinanciadoresPrestacionesProfesionales p ")
                .Where($"p.idFinanciadorPrestacion = {idFinanciadorPrestacion}")
                .Where($"p.idProfesional = {idProfesional}");

            return await query.QueryFirstOrDefaultAsync<FinanciadorPrestacionProfesional>(transaction);
        }
    }
}