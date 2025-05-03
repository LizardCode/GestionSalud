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
    public class UsuariosEmpresasRepository : BaseRepository, IUsuariosEmpresasRepository
    {
        public UsuariosEmpresasRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<UsuarioEmpresa>> GetAllByIdUsuario(int idUsuario, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ue.*
                    FROM UsuariosEmpresas ue
                    WHERE
                        ue.IdUsuario = {idUsuario}");

            var results = await builder.QueryAsync<UsuarioEmpresa>(transaction);

            return results.AsList();
        }

        public async Task<bool> RemoveByIdUsuario(int idUsuario, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM UsuariosEmpresas
                    WHERE IdUsuario = {idUsuario}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}