using Dapper;
using DapperQueryBuilder;
using Dawa.Framework.Application.Interfaces.Context;
using LizadCode.SalmaSalud.Notifications.Application.Interfaces.Repositories;
using LizadCode.SalmaSalud.Notifications.Domain.Entities;
using LizadCode.SalmaSalud.Notifications.Domain.Enums;
using System.Data;
using Custom = LizadCode.SalmaSalud.Notifications.Domain.EntitiesCustom;

namespace LizadCode.SalmaSalud.Notifications.Infrastructure.Repositories
{
    public class TurnosRepository : BaseRepository, ITurnosRepository
    {
        public TurnosRepository(IDbContext context) : base(context)
        {

        }
        public async Task<Custom.Turno> GetByIdCustom(int idTurno, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT TUR.*,
                        ESP.Descripcion as Especialidad,
		                PRO.Nombre as Profesional,
		                PAC.Nombre as Paciente,
		                Convert(varchar, TUR.fechaInicio, 23) as Fecha, 
                        Convert(varchar, TUR.fechaInicio, 8) as Hora, 
                        PAC.telefono as Telefono,
		                ETU.Descripcion as Estado,
                        ETU.Clase as EstadoClase,
                        CASE WHEN ISNULL(PAC.IdTipoIva, 0) = 0 THEN 1 ELSE 0 END as FichaIncompleta
                FROM Turnos TUR
                INNER JOIN Especialidades ESP ON (TUR.IdEspecialidad = ESP.IdEspecialidad)
                INNER JOIN Profesionales PRO ON (TUR.IdProfesional = PRO.IdProfesional)
                INNER JOIN Pacientes PAC ON (TUR.IdPaciente = PAC.IdPaciente)
                INNER JOIN EstadoTurno ETU ON (TUR.IdEstadoTurno = ETU.IdEstadoTurno)")
                .Where($"TUR.IdTurno = {idTurno}");

            return await builder.QuerySingleAsync<Custom.Turno>(transaction);
        }

        public async Task<IList<Turno>> GetTurnosAConfirmar()
        {
            var fecha = DateTime.Now.Date.AddDays(1);

            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT t.*
                    FROM Turnos t
                    WHERE (t.IdEstadoTurno = {(int)EstadoTurno.Agendado} 
                            OR t.IdEstadoTurno = {(int)EstadoTurno.ReAgendado}
                            OR t.IdEstadoTurno = {(int)EstadoTurno.Confirmado})
                        AND convert(varchar, t.fechaInicio, 23) = {fecha.ToString("yyyy-MM-dd")}
                    
                ");

            var result = await query.QueryAsync<Turno>();

            return result.AsList();
        }
    }
}
