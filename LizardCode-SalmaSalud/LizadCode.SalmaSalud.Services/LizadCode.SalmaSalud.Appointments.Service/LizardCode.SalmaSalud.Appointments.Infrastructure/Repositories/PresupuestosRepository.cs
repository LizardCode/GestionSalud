using Dapper;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Appointments.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Appointments.Domain.Entities;
using LizardCode.SalmaSalud.Appointments.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Appointments.Infrastructure.Repositories
{
    public class PresupuestosRepository : BaseRepository, IPresupuestosRepository
    {
        public PresupuestosRepository(IDbContext context) : base(context)
        {

        }

        public async Task<IList<Presupuesto>> GetPresupuestosAVencer()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    c.*
                    FROM Presupuestos c
                    WHERE idEstadoPresupuesto = {(int)EstadoPresupuesto.Abierto} AND fechaVencimiento < GETDATE()
                    
                ");

            var result = await query.QueryAsync<Presupuesto>();

            return result.AsList();
        }
    }
}

