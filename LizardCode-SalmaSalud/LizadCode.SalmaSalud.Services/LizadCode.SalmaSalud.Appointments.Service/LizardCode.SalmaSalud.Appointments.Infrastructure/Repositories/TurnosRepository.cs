using Dapper;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Appointments.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Appointments.Domain.Entities;
using LizardCode.SalmaSalud.Appointments.Domain.Enums;

namespace LizardCode.SalmaSalud.Appointments.Infrastructure.Repositories
{
    public class TurnosRepository : BaseRepository, ITurnosRepository
    {
        public TurnosRepository(IDbContext context) : base(context)
        {

        }

        public async Task<IList<Turno>> GetTurnosAusentes()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT t.*
                    FROM Turnos t
                    WHERE (t.IdEstadoTurno = {(int)EstadoTurno.Agendado} 
                            OR t.IdEstadoTurno = {(int)EstadoTurno.ReAgendado} 
                            OR t.IdEstadoTurno = {(int)EstadoTurno.Confirmado}
                            OR t.IdEstadoTurno = {(int)EstadoTurno.Recepcionado})
                        AND t.fechaInicio < {DateTime.Now.Date}
                    
                ");

            var result = await query.QueryAsync<Turno>();

            return result.AsList();
        }
    }
}
