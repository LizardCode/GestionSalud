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
                                    Dias,
                                    Rangos,
	                                ets.descripcion as Estado,
	                                ets.clase as EstadoClase
                                FROM TurnosSolicitud ts
                                INNER JOIN Pacientes p ON p.IdPaciente = ts.idPaciente
                                INNER JOIN Especialidades e ON e.IdEspecialidad = ts.idEspecialidad
                                INNER JOIN EstadoTurnoSolicitud ets ON ets.idEstadoTurnoSolicitud = TS.idEstadoTurnoSolicitud 
                                LEFT JOIN (
                                    SELECT idTurnoSolicitud, 
		                                    STRING_AGG(CASE Dia WHEN 1 THEN 'LU' WHEN 2 THEN 'MA' WHEN 3 THEN 'MI' WHEN 4 THEN 'JU' WHEN 5 THEN 'VI' WHEN 6 THEN 'SA' ELSE 'DO' END, ', ') AS Dias
                                    FROM TurnosSolicitudDia
                                    GROUP BY idTurnoSolicitud
                                ) AS vwTSD ON (vwTSD.idTurnoSolicitud = ts.idTurnoSolicitud)
                                LEFT JOIN (
                                    SELECT TSRH.idTurnoSolicitud, 
		                                    STRING_AGG(TRH.descripcion, ', ') AS Rangos
                                    FROM TurnosSolicitudRangoHorario TSRH
                                    INNER JOIN TipoRangoHorario TRH ON (TSRH.idRangoHorario = TRH.idRangoHorario) 
                                    GROUP BY TSRH.idTurnoSolicitud
                                ) AS vwTSRH ON (vwTSRH.idTurnoSolicitud = ts.idTurnoSolicitud) 
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
                    Dias,
                    Rangos,
	                ets.descripcion as Estado,
	                ets.clase as EstadoClase
                FROM TurnosSolicitud ts
                INNER JOIN Pacientes p ON p.IdPaciente = ts.idPaciente
                INNER JOIN Especialidades e ON e.IdEspecialidad = ts.idEspecialidad
                INNER JOIN EstadoTurnoSolicitud ets ON ets.idEstadoTurnoSolicitud = TS.idEstadoTurnoSolicitud 
                LEFT JOIN (
                    SELECT idTurnoSolicitud, 
		                    STRING_AGG(CASE Dia WHEN 1 THEN 'LU' WHEN 2 THEN 'MA' WHEN 3 THEN 'MI' WHEN 4 THEN 'JU' WHEN 5 THEN 'VI' WHEN 6 THEN 'SA' ELSE 'DO' END, ', ') AS Dias
                    FROM TurnosSolicitudDia
                    GROUP BY idTurnoSolicitud
                ) AS vwTSD ON (vwTSD.idTurnoSolicitud = ts.idTurnoSolicitud)
                LEFT JOIN (
                    SELECT TSRH.idTurnoSolicitud, 
		                    STRING_AGG(TRH.descripcion, ', ') AS Rangos
                    FROM TurnosSolicitudRangoHorario TSRH
                    INNER JOIN TipoRangoHorario TRH ON (TSRH.idRangoHorario = TRH.idRangoHorario) 
                    GROUP BY TSRH.idTurnoSolicitud
                ) AS vwTSRH ON (vwTSRH.idTurnoSolicitud = ts.idTurnoSolicitud) ";

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

        public async Task<Custom.TurnoSolicitudTotales> GetTotalesDashboard()
        {
            FormattableString sql = $@"SELECT * FROM (
                                        SELECT COUNT(*) as Total,
		                                        SUM(CASE WHEN [idEstadoTurnoSolicitud] = 1 THEN 1 ELSE 0 END) Solicitados, 
                                                SUM(CASE WHEN [idEstadoTurnoSolicitud] = 2 THEN 1 ELSE 0 END) Asignados,
		                                        SUM(CASE WHEN [idEstadoTurnoSolicitud] = 3 THEN 1 ELSE 0 END) Cancelados
                                        FROM TurnosSolicitud
                                    ) AS vwTotales ";

            var builder = _context.Connection
                .QueryBuilder(sql);

            return await builder.QuerySingleAsync<Custom.TurnoSolicitudTotales>();
        }
    }
}
