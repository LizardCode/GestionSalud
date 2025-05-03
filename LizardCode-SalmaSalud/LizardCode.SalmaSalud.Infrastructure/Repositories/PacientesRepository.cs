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
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class PacientesRepository : BaseRepository, IPacientesRepository, IDataTablesCustomQuery
    {
        public PacientesRepository(IDbContext context) : base(context)
        {
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						p.*,
						ti.Descripcion AS TipoIVA,
                        ISNULL(f.Nombre, '') AS Financiador,
                        ISNULL(fp.Nombre, '') AS FinanciadorPlan,
                        CASE WHEN ISNULL(p.IdTipoIva, 0) = 0 THEN 1 ELSE 0 END as FichaIncompleta
                    FROM Pacientes p
					LEFT JOIN TipoIVA ti
						ON ti.IdTipoIVA = p.IdTipoIVA
                    LEFT JOIN Financiadores f
						ON f.IdFinanciador = p.IdFinanciador
                    LEFT JOIN FinanciadoresPlanes fp
						ON fp.IdFinanciadorPlan = p.IdFinanciadorPlan
                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<Paciente> GetPacienteByCUIT(string cuit, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT p.* FROM Pacientes p ")
                .Where($"p.CUIT = {cuit}");

            return await query.QueryFirstOrDefaultAsync<Paciente>(transaction);
        }

        public async Task<Paciente> GetPacienteByDocumento(string documento, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT p.* FROM Pacientes p ")
                .Where($"p.Documento = {documento}");

            return await query.QueryFirstOrDefaultAsync<Paciente>(transaction);
        }

        public async Task<bool> ValidarCUITExistente(string cuit, int? id, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT p.* FROM Pacientes p
                ")
                .Where($"p.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"p.CUIT = {cuit}");

            if (id != null)
            {
                query.Where($"p.IdPaciente <> {id}");
            }

            var result = await query.QueryAsync<Paciente>(transaction);

            return result.AsList().Count > 0;
        }

        public async Task<bool> ValidarDocumentoExistente(string documento, int? id, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT p.* FROM Pacientes p
                ")
                .Where($"p.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"p.Documento = {documento}");

            if (id != null)
            {
                query.Where($"p.IdPaciente <> {id}");
            }

            var result = await query.QueryAsync<Paciente>(transaction);

            return result.AsList().Count > 0;
        }

        public async Task<bool> ValidarNroFinanciadorExistente(string financiadorNro, int? id, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT p.* FROM Pacientes p
                ")
                .Where($"p.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"p.FinanciadorNro = {financiadorNro}");

            if (id != null)
            {
                query.Where($"p.IdPaciente <> {id}");
            }

            var result = await query.QueryAsync<Paciente>(transaction);

            return result.AsList().Count > 0;
        }

        public async Task<Custom.Paciente> GetLikeDocumentoCustom(string documento, IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
                .QueryBuilder(sql)
                .Where($@"
                    p.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}
                    AND p.Documento LIKE '{$"%{documento}%"}'"
                );

            var result = await builder.QueryFirstOrDefaultAsync<Custom.Paciente>(transaction);

            return result;
        }
        public async Task<Custom.Paciente> GetLikePhoneCustom(string phone, IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
                .QueryBuilder(sql)
                .Where($@"
                    p.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}
                    AND p.Telefono LIKE '{$"%{phone}%"}'"
                );

            var result = await builder.QueryFirstOrDefaultAsync<Custom.Paciente>(transaction);

            return result;
        }

        public async Task<Custom.Paciente> GetCustomById(int idPaciente, IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
                .QueryBuilder(sql)
                .Where($@"p.idPaciente = {idPaciente} "
                );

            var result = await builder.QueryFirstOrDefaultAsync<Custom.Paciente>(transaction);

            return result;
        }
        private FormattableString BuildCustomQuery() =>
            $@"SELECT
	            p.*,
	            f.Nombre as Financiador,
	            fp.Nombre as FinanciadorPlan
            FROM Pacientes p
            LEFT JOIN Financiadores f ON (F.IdFinanciador = P.IdFinanciador)
            LEFT JOIN FinanciadoresPlanes fp ON (Fp.IdFinanciadorPlan = P.IdFinanciadorPlan) ";
    }
}
