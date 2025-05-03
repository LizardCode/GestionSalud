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
    public class FinanciadoresPrestacionesRepository : BaseRepository, IFinanciadoresPrestacionesRepository
    {
        public FinanciadoresPrestacionesRepository(IDbContext context) : base(context)
        {
        }

        public async Task<FinanciadorPrestacion> GetFinanciadorPrestacionById(long idFinanciadorPrestacion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    fp.*
                    FROM FinanciadoresPrestaciones fp
                    WHERE
                        fp.IdFinanciadorPrestacion = {idFinanciadorPrestacion}");

            var results = await builder.QueryFirstOrDefaultAsync<FinanciadorPrestacion>(transaction);

            return results;
        }

        public async Task<IList<FinanciadorPrestacion>> GetAllByIdFinanciador(long idFinanciador, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    fp.*
                    FROM FinanciadoresPrestaciones fp
                    WHERE
                        fp.IdEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND
                        fp.IdFinanciador = {idFinanciador}");

            var results = await builder.QueryAsync<FinanciadorPrestacion>(transaction);

            return results.AsList();
        }

        public async Task<IList<FinanciadorPrestacion>> GetAllByIdFinanciadorAndIdPlan(long idFinanciador, long idFinanciadorPlan, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    fp.*,
                        fp.Codigo + ' - ' + fp.Descripcion as Descripcion
                    FROM FinanciadoresPrestaciones fp
                    WHERE
                        fp.IdEstadoRegistro != {(int)EstadoRegistro.Eliminado} 
                        AND ({idFinanciador} = 0 OR fp.IdFinanciador = {idFinanciador}) 
                        AND fp.IdFinanciadorPlan = {idFinanciadorPlan} ");

            var results = await builder.QueryAsync<FinanciadorPrestacion>(transaction);

            return results.AsList();
        }

        public async Task<bool> RemoveByIdFinanciador(long idFinanciador, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE FinanciadoresPrestaciones
                    SET IdEstadoRegistro = {(int)EstadoRegistro.Eliminado}
                    WHERE IdFinanciador = {idFinanciador} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> RemoveById(long idFinanciadorPlan, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM FinanciadoresPrestaciones
                    WHERE idFinanciadorPlan = {idFinanciadorPlan} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT FPR.*,
		                                FPL.Nombre AS FinanciadorPlan,
		                                FPL.Codigo AS FinanciadorPlanCodigo,
		                                P.Descripcion AS Prestacion,
		                                P.Codigo AS PrestacionCodigo
                                FROM FinanciadoresPrestaciones FPR 
                                INNER JOIN FinanciadoresPlanes FPL ON (FPL.IdFinanciadorPlan = FPR.IdFinanciadorPlan)
                                LEFT  JOIN Prestaciones P ON (P.IdPrestacion = FPR.IdPrestacion) ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<FinanciadorPrestacion> GetByCodigo(string codigo, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        p.*
                    FROM FinanciadoresPrestaciones p 
                        WHERE
                            p.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND                            
                            p.Codigo = {codigo}
                ");

            return await builder.QueryFirstOrDefaultAsync<FinanciadorPrestacion>(transaction);
        }
    }
}
