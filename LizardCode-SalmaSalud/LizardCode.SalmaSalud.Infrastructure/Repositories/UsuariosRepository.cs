using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Threading.Tasks;
using LizardCode.Framework.Application.Common.Enums;
using Dapper.DataTables.Models;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class UsuariosRepository : BaseRepository, IUsuariosRepository
    {
        public UsuariosRepository(IDbContext context) : base(context)
        {
        }
        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						u.*
                    FROM Usuarios u
                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<List<Usuario>> GetAllUsuariosByIdEmpresaLookup(int idEmpresa)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						u.*
                    FROM Usuarios u
                ")
                .Where($"u.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"u.IdTipoUsurio <> {(int)TipoUsuario.Paciente}")
                .Where($"u.IdUsuario IN (SELECT IdUsuario FROM UsuariosEmpresas WHERE IdEmpresa = {idEmpresa})");

            var result = await query.QueryAsync<Usuario>();

            return result.AsList();
        }
        public async Task<Usuario> GetByDocumento(string documento)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						u.*
                    FROM Usuarios u
                    INNER JOIN Pacientes p ON (p.idPaciente = u.idPaciente)
                ")
                .Where($"u.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"p.Documento = {documento}");

            var result = await query.QuerySingleOrDefaultAsync<Usuario>();

            return result;
        }
    }
}
