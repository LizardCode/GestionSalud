using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class TurnosSolicitudRepository : BaseRepository, ITurnosSolicitudRepository, IDataTablesCustomQuery
    {
        public TurnosSolicitudRepository(IDbContext context) : base(context)
        {
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ts.*,
	                    P.Nombre AS Paciente,
                        P.Documento,
                        P.Email,
                        P.telefono,
                        e.Descripcion as Especialidad,
                        trh.descripcion as RangoHorario,
	                    ets.descripcion as Estado,
	                    ets.clase as EstadoClase
                    FROM TurnosSolicitud ts
                    INNER JOIN Pacientes p ON p.IdPaciente = ts.idPaciente
                    INNER JOIN Especialidades e ON e.IdEspecialidad = ts.idEspecialidad
                    INNER JOIN TipoRangoHorario trh ON trh.idRangoHorario = TS.idRangoHorario
                    INNER JOIN EstadoTurnoSolicitud ets ON ets.idEstadoTurnoSolicitud = TS.idEstadoTurnoSolicitud
                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<Custom.TurnoSolicitud> GetCustomById(int idTurnoSolicitud, IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
                .QueryBuilder(sql)
                .Where($@"ts.idTurnoSolicitud = {idTurnoSolicitud} "
                );

            var result = await builder.QueryFirstOrDefaultAsync<Custom.TurnoSolicitud>(transaction);

            return result;
        }

        private FormattableString BuildCustomQuery() =>
            $@" SELECT
                    ts.*,
                    P.Nombre AS Paciente,
                    P.Documento,
                    P.Email,
                    P.telefono,
                    e.Descripcion as Especialidad,
                    trh.descripcion as RangoHorario,
	                ets.descripcion as Estado,
	                ets.clase as EstadoClase
                FROM TurnosSolicitud ts
                INNER JOIN Pacientes p ON p.IdPaciente = ts.idPaciente
                INNER JOIN Especialidades e ON e.IdEspecialidad = ts.idEspecialidad
                INNER JOIN TipoRangoHorario trh ON trh.idRangoHorario = TS.idRangoHorario
                INNER JOIN EstadoTurnoSolicitud ets ON ets.idEstadoTurnoSolicitud = TS.idEstadoTurnoSolicitud ";

        public async Task<List<Custom.TurnoSolicitud>> GetTurnosByIdPaciente(int idPaciente, IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
                .QueryBuilder(sql)
                .Where($@"
                    ts.IdPaciente = {idPaciente} " //AND ts.idEstadoTurnoSolicitud = {(int)EstadoTurnoSolicitud.Solicitado} "
                );

            var result = await builder.QueryAsync<Custom.TurnoSolicitud>(transaction);

            return result.ToList();
        }

        public async Task<List<Custom.TurnoSolicitud>> GetTurnosByIdPacienteFinalizados(int idPaciente, IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
                .QueryBuilder(sql)
                .Where($@"
                    ts.IdPaciente = {idPaciente} AND ts.idEstadoTurnoSolicitud != {(int)EstadoTurnoSolicitud.Solicitado} "
                ); ;

            var result = await builder.QueryAsync<Custom.TurnoSolicitud>(transaction);

            return result.ToList();
        }
    }
}
