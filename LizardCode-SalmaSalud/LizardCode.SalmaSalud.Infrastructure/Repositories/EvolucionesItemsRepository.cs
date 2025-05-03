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
    public class EvolucionesItemsRepository : BaseRepository, IEvolucionesItemsRepository
    {
        public EvolucionesItemsRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<EvolucionPrestacion>> GetAllByIdEvolucion(int idEvolucion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ei.*
                    FROM EvolucionesItems ei
                    WHERE
                        ei.idEvolucion = {idEvolucion}" );

            var results = await builder.QueryAsync<EvolucionPrestacion>(transaction);

            return results.AsList();
        }

        public async Task<bool> DeleteByIdEvolucion(int idEvolucion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE ei FROM EvolucionesItems ei                        
                    WHERE 
                        ei.idEvolucion = {idEvolucion}" );

            var results = await builder.ExecuteAsync(transaction);

            return (results > 1);
        }
    }
}