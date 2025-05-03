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
    public class LaboratoriosServiciosRepository : BaseRepository, ILaboratoriosServiciosRepository
    {
        public LaboratoriosServiciosRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<LaboratorioServicio>> GetAllByIdLaboratorio(long idLaboratorio, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ls.*
                    FROM LaboratoriosServicios ls
                    WHERE
                        ls.IdProveedor = {idLaboratorio}");

            var results = await builder.QueryAsync<LaboratorioServicio>(transaction);

            return results.AsList();
        }

        public async Task<bool> RemoveByIdLaboratorio(long idLaboratorio, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM LaboratoriosServicios
                    WHERE IdProveedor = {idLaboratorio} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> RemoveById(long idLaboratorioServicio, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM LaboratoriosServicios
                    WHERE IdLaboratorioServicio = {idLaboratorioServicio} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}