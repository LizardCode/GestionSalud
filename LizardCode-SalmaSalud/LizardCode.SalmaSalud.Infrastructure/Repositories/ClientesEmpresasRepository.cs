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
    public class ClientesEmpresasRepository : BaseRepository, IClientesEmpresasRepository
    {
        public ClientesEmpresasRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<ClienteEmpresa>> GetAllByIdCliente(int idCliente, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ce.*
                    FROM ClientesEmpresas ce
                    WHERE
                        ce.IdCliente = {idCliente}");

            var results = await builder.QueryAsync<ClienteEmpresa>(transaction);

            return results.AsList();
        }

        public async Task<bool> RemoveByIdCliente(int idCliente, int idUsuario, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE ce FROM ClientesEmpresas ce
                        INNER JOIN UsuariosEmpresas ue ON ce.IdEmpresa = ue.IdEmpresa
                    WHERE 
                        ce.IdCliente = {idCliente} AND
                        ue.IdUsuario = {idUsuario}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}