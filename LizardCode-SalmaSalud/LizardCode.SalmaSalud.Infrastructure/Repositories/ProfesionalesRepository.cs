using Dapper;
using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ProfesionalesRepository : BaseRepository, IProfesionalesRepository, IDataTablesCustomQuery
    {
        public ProfesionalesRepository(IDbContext context) : base(context)
        {
        }

        public async Task<List<Profesional>> GetAllProfesionalesByIdEmpresaLookup(int idEmpresa)
        {
            QueryBuilder query = null;

            if (idEmpresa == 0)
            {
                query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						p.*
                    FROM Profesionales p
                ")
                .Where($"p.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            }
            else
            {
                query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						p.*
                    FROM Profesionales p
                ")
                .Where($"p.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"p.IdProfesional IN (SELECT IdProfesional FROM ProfesionalesEmpresas WHERE IdEmpresa = {idEmpresa})");
            }

            var result = await query.QueryAsync<Profesional>();

            return result.AsList();
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						p.*,
						ti.Descripcion AS TipoIVA,
                        e.Descripcion AS Especialidad
                    FROM Profesionales p
					INNER JOIN TipoIVA ti
						ON ti.IdTipoIVA = p.IdTipoIVA
                    INNER JOIN Especialidades e
						ON e.IdEspecialidad = p.IdEspecialidad
                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<Profesional> GetProfesionalByCUIT(string cuit, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT p.* FROM Profesionales p ")
                .Where($"p.CUIT = {cuit}");

            return await query.QueryFirstOrDefaultAsync<Profesional>(transaction);
        }

        public async Task<bool> ValidarCUITExistente(string cuit, int? id, int idEmpresa, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT p.* FROM Profesionales p
                        INNER JOIN ProfesionalesEmpresas ce ON p.IdProfesional = ce.IdProfesional
                ")
                .Where($"p.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"ce.IdEmpresa = {idEmpresa}")
                .Where($"p.CUIT = {cuit}");

            if (id != null)
            {
                query.Where($"p.IdProfesional <> {id}");
            }

            var result = await query.QueryAsync<Profesional>(transaction);

            return result.AsList().Count > 0;
        }
    }
}
