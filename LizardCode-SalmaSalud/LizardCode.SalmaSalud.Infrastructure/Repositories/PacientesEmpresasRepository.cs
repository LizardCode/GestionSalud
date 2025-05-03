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
    public class PacientesEmpresasRepository : BaseRepository, IPacientesEmpresasRepository
    {
        public PacientesEmpresasRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<PacienteEmpresa>> GetAllByIdPaciente(int IdPaciente, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ce.*
                    FROM PacientesEmpresas ce
                    WHERE
                        ce.IdPaciente = {IdPaciente}");

            var results = await builder.QueryAsync<PacienteEmpresa>(transaction);

            return results.AsList();
        }

        public async Task<bool> RemoveByIdPaciente(int IdPaciente, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM PacientesEmpresas                      
                    WHERE IdPaciente = {IdPaciente}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}