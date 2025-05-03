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
    public class ProfesionalesEmpresasRepository : BaseRepository, IProfesionalesEmpresasRepository
    {
        public ProfesionalesEmpresasRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<ProfesionalEmpresa>> GetAllByIdProfesional(int IdProfesional, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ce.*
                    FROM ProfesionalesEmpresas ce
                    WHERE
                        ce.IdProfesional = {IdProfesional}");

            var results = await builder.QueryAsync<ProfesionalEmpresa>(transaction);

            return results.AsList();
        }

        public async Task<bool> RemoveByIdProfesional(int IdProfesional, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE ce FROM ProfesionalesEmpresas ce                        
                    WHERE 
                        ce.IdProfesional = {IdProfesional}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}