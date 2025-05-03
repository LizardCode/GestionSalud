using Dapper;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    internal class PresupuestosPrestacionesRepository : BaseRepository, IPresupuestosPrestacionesRepository
    {
        public PresupuestosPrestacionesRepository(IDbContext context) : base(context)
        {
        }
        public async Task<PresupuestoPrestacion> GetPresupuestoPrestacionById(long idPresupuestoPrestacion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    fp.*
                    FROM PresupuestosPrestaciones fp
                    WHERE
                        fp.IdPresupuestoPrestacion = {idPresupuestoPrestacion}");

            var results = await builder.QueryFirstOrDefaultAsync<PresupuestoPrestacion>(transaction);

            return results;
        }

        public async Task<IList<PresupuestoPrestacion>> GetAllByIdPresupuesto(long idPresupuesto, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    fp.*
                    FROM PresupuestosPrestaciones fp
                    WHERE
                        fp.idPresupuesto = {idPresupuesto}");

            var results = await builder.QueryAsync<PresupuestoPrestacion>(transaction);

            return results.AsList();
        }

        public async Task<bool> RemoveByIdPresupuesto(long idPresupuesto, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM PresupuestosPrestaciones
                    WHERE idPresupuesto = {idPresupuesto} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> RemoveById(long idPresupuestoPrestacion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM PresupuestosPrestaciones
                    WHERE idPresupuestoPrestacion = {idPresupuestoPrestacion} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}
