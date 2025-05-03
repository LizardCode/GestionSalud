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
	public class EvolucionesOdontogramasPiezasRepository : BaseRepository, IEvolucionesOdontogramasPiezasRepository
	{
		public EvolucionesOdontogramasPiezasRepository(IDbContext context) : base(context)
		{
		}

		public async Task<IList<EvolucionOdontogramaPieza>> GetAllByIdEvolucion(int idEvolucion, IDbTransaction transaction = null)
		{
			var builder = _context.Connection
				.QueryBuilder($@"
                    SELECT
	                    ei.*
                    FROM EvolucionesOdontogramasPiezas ei
                    WHERE
                        ei.idEvolucion = {idEvolucion}");

			var results = await builder.QueryAsync<EvolucionOdontogramaPieza>(transaction);

			return results.AsList();
		}
	}
}