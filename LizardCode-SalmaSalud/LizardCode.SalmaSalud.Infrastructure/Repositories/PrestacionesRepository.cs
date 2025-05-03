using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class PrestacionesRepository : BaseRepository, IPrestacionesRepository
    {
        public PrestacionesRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> Update(Prestacion prestacion, IDbTransaction transaction = null)
        {
            var bResult = await base.Update(prestacion, transaction);

            var builder = _context.Connection
                    .CommandBuilder($@"
                        UPDATE FinanciadoresPrestaciones 
                        SET codigoPrestacion = '{prestacion.Codigo}' 
                        WHERE IdPrestacion = {prestacion.IdPrestacion} ");

            await builder.ExecuteAsync(transaction);

            return bResult;
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT P.*
                                 FROM Prestaciones P ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<Prestacion> GetByCodigo(string codigo, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        p.*
                    FROM Prestaciones p 
                        WHERE
                            p.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                            p.Codigo = {codigo}
                ");

            return await builder.QueryFirstOrDefaultAsync<Prestacion>(transaction);
        }

        public async Task<IList<Prestacion>> GetAllPrestaciones(IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    p.*,
                        p.Codigo + ' - ' + p.Descripcion as Descripcion
                    FROM Prestaciones p
                    WHERE
                        p.IdEstadoRegistro != {(int)EstadoRegistro.Eliminado} ");

            var results = await builder.QueryAsync<Prestacion>(transaction);

            return results.AsList();
        }
    }
}
