using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using Dapper;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class PrestacionesProfesionalesRepository : BaseRepository, IPrestacionesProfesionalesRepository
    {
        public PrestacionesProfesionalesRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<PrestacionProfesional>> GetAllByIdPrestacion(long idPrestacion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    fp.*
                    FROM PrestacionesProfesionales fp
                    WHERE
                        fp.idPrestacion = {idPrestacion}");

            var results = await builder.QueryAsync<PrestacionProfesional>(transaction);

            return results.AsList();
        }

        public async Task<bool> RemoveByIdPrestacion(long idPrestacion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM PrestacionesProfesionales
                    WHERE idPrestacion = {idPrestacion} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<PrestacionProfesional> GetByIdPrestacionAndProfesional(int idPrestacion, int idProfesional, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT p.* FROM PrestacionesProfesionales p ")
                .Where($"p.idPrestacion = {idPrestacion}")
                .Where($"p.idProfesional = {idProfesional}");

            return await query.QueryFirstOrDefaultAsync<PrestacionProfesional>(transaction);
        }
    }
}