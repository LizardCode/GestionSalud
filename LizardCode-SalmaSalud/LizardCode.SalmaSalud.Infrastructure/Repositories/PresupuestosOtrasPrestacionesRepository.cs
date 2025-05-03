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
    public class PresupuestosOtrasPrestacionesRepository : BaseRepository, IPresupuestosOtrasPrestacionesRepository
    {
        public PresupuestosOtrasPrestacionesRepository(IDbContext context) : base(context)
        {
        }
        public async Task<PresupuestoOtraPrestacion> GetPresupuestoOtraPrestacionById(long idPresupuestoOtraPrestacion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    fp.*
                    FROM PresupuestosOtrasPrestaciones fp
                    WHERE
                        fp.idPresupuestoOtraPrestacion = {idPresupuestoOtraPrestacion}");

            var results = await builder.QueryFirstOrDefaultAsync<PresupuestoOtraPrestacion>(transaction);

            return results;
        }

        public async Task<IList<PresupuestoOtraPrestacion>> GetAllByIdPresupuesto(long idPresupuesto, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    fp.*
                    FROM PresupuestosOtrasPrestaciones fp
                    WHERE
                        fp.idPresupuesto = {idPresupuesto}");

            var results = await builder.QueryAsync<PresupuestoOtraPrestacion>(transaction);

            return results.AsList();
        }

        public async Task<bool> RemoveByIdPresupuesto(long idPresupuesto, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM PresupuestosOtrasPrestaciones
                    WHERE idPresupuesto = {idPresupuesto} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> RemoveById(long idPresupuestoOtraPrestacion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM PresupuestosOtrasPrestaciones
                    WHERE idPresupuestoOtraPrestacion = {idPresupuestoOtraPrestacion} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}
