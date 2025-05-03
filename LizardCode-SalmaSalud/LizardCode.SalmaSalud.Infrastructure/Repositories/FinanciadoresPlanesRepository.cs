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
    public class FinanciadoresPlanesRepository : BaseRepository, IFinanciadoresPlanesRepository
    {
        public FinanciadoresPlanesRepository(IDbContext context) : base(context)
        {
        }

        public async Task<bool> Update(FinanciadorPlan financiadorPlan, IDbTransaction transaction = null)
        {
            var bResult = await base.Update(financiadorPlan, transaction);

            var builder = _context.Connection
                    .CommandBuilder($@"
                        UPDATE FinanciadoresPrestaciones 
                        SET codigoFinanciadorPlan = '{financiadorPlan.Codigo}' 
                        WHERE idFinanciadorPlan = {financiadorPlan.IdFinanciadorPlan} ");

            await builder.ExecuteAsync(transaction);

            return bResult;
        }

        public async Task<IList<FinanciadorPlan>> GetAllByIdFinanciador(long idFinanciador, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ce.*
                    FROM FinanciadoresPlanes ce
                    WHERE
                        ce.IdEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND
                        ce.IdFinanciador = {idFinanciador}");

            var results = await builder.QueryAsync<FinanciadorPlan>(transaction);

            return results.AsList();
        }

        public async Task<FinanciadorPlan> GetByCodigo(string codigo, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ce.*
                    FROM FinanciadoresPlanes ce
                    WHERE
                        ce.IdEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND
                        ce.codigo = {codigo}");

            var results = await builder.QueryFirstOrDefaultAsync<FinanciadorPlan>(transaction);

            return results;
        }

        public async Task<bool> RemoveByIdFinanciador(long idFinanciador, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE FinanciadoresPlanes
                    SET IdEstadoRegistro = {(int)EstadoRegistro.Eliminado}
                    WHERE IdFinanciador = {idFinanciador} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> RemoveById(long idFinanciadorPlan, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM FinanciadoresPlanes
                    WHERE idFinanciadorPlan = {idFinanciadorPlan} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> ExistePacienteAsociado(long idFinanciadorPlan, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM Pacientes
                    WHERE idFinanciadorPlan = {idFinanciadorPlan} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results > 0);
        }
    }
}