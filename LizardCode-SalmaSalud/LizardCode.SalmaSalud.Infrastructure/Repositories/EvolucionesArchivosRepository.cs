using Dapper;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class EvolucionesArchivosRepository : BaseRepository, IEvolucionesArchivosRepository
    {
        public EvolucionesArchivosRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<EvolucionArchivo>> GetAllByIdEvolucion(int idEvolucion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ei.*
                    FROM EvolucionesArchivos ei
                    WHERE
                        ei.idEvolucion = {idEvolucion}");

            var results = await builder.QueryAsync<EvolucionArchivo>(transaction);

            return results.AsList();
        }

        public async Task<bool> DeleteByIdEvolucion(int idEvolucion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE ei FROM EvolucionesArchivos ei                        
                    WHERE 
                        ei.idEvolucion = {idEvolucion}");

            var results = await builder.ExecuteAsync(transaction);

            return (results > 1);
        }
    }
}