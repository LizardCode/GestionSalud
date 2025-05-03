using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using System.Data;
using System.Threading.Tasks;
using Dapper.DataTables.Models;
using System;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Collections.Generic;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Linq;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Models.Permisos;
using System.Collections;
using NPOI.SS.Formula.Functions;
using System.Reflection.PortableExecutable;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
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
                        CASE WHEN ISNULL(PAC.IdTipoIva, 0) = 0 THEN 1 ELSE 0 END as FichaIncompleta,
                        TTU.Siglas as TipoTurno
                FROM Turnos TUR
                LEFT JOIN Especialidades ESP ON (TUR.IdEspecialidad = ESP.IdEspecialidad)
                LEFT JOIN Profesionales PRO ON (TUR.IdProfesional = PRO.IdProfesional)
                INNER JOIN Pacientes PAC ON (TUR.IdPaciente = PAC.IdPaciente)
                INNER JOIN EstadoTurno ETU ON (TUR.IdEstadoTurno = ETU.IdEstadoTurno)
                INNER JOIN TipoTurnos TTU ON (TTU.IdTipoTurno = TUR.IdTipoTurno) ")
                .Where($"TUR.IdTurno = {idTurno}");

            return await builder.QuerySingleAsync<Custom.Turno>(transaction);
        }
        public DataTablesCustomQuery GetProfesionalesDisponiblesByEspecialidad(string desde, string hasta, int idEmpresa, int idEspecialidad, int idProfesional)
        {
            var query = _context.Connection
                    .QueryBuilder($@"
                                    SELECT P.IdProfesional, P.Nombre as Profesional, MIN(PT.fechaInicio) as PrimeroDisponible, COUNT(*) as TurnosDisponibles
                                    FROM ProfesionalesTurnos PT
                                    INNER JOIN Profesionales P ON (PT.IdProfesional = P.IdProfesional)
                                    WHERE ({idProfesional} = 0 OR PT.IdProfesional = {idProfesional})
                                        AND PT.IdEmpresa = {idEmpresa} 
                                        AND P.IdEspecialidad = {idEspecialidad} 
                                        AND PT.fechaInicio >= {desde}  
                                        AND PT.fechaFin <= {hasta}
                                    GROUP BY P.IdProfesional, P.Nombre
                                ");

            return base.GetAllCustomQuery(query);
        }

        public DataTablesCustomQuery GetTuenosDisponiblesByProfesional(DateTime desde, DateTime hasta, int idEmpresa, int idProfesional)
        {
            var query = _context.Connection
                    .QueryBuilder($@"
                                    SELECT P.IdProfesional, P.Nombre, MIN(PT.fechaInicio) as PrimeroDisponible, COUNT(*) as TurnosDisponibles
                                    FROM ProfesionalesTurnos PT
                                    INNER JOIN Profesionales P ON (PT.IdProfesional = P.IdProfesional)
                                    GROUP BY P.IdProfesional, P.Nombre
                                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<IList<Custom.TurnoDisponible>> GetTurnosDisponiblesPorDia(int idEmpresa, DateTime desde, DateTime hasta, int idEspecialidad, int idProfesional)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT Convert(varchar, fechaInicio, 23) as Fecha, COUNT(*) as Cantidad
                    FROM ProfesionalesTurnos PT
                    INNER JOIN Profesionales P ON (PT.IdProfesional = P.IdProfesional)
                    WHERE PT.IdEstadoProfesionalTurno = {(int)EstadoProfesionalTurno.Disponible} AND
                        PT.fechaInicio >= {desde}  AND
                        PT.fechaFin <= {hasta} AND
                        PT.IdEmpresa = {idEmpresa} AND
                        ({idProfesional} = 0 OR PT.IdProfesional = {idProfesional}) AND
                        ({idEspecialidad} = 0 OR P.IdEspecialidad = {idEspecialidad})
                    GROUP BY Convert(varchar, fechaInicio, 23)
                    
                ");

            var result = await query.QueryAsync<Custom.TurnoDisponible>();

            return result.AsList();
        }

        public async Task<IList<Custom.Turno>> GetTurnosDisponibles(int idEmpresa, DateTime fecha, int idEspecialidad, int idProfesional)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT PT.IdProfesionalTurno, PT.IdProfesional, P.Nombre as Profesional, 
                            Convert(varchar, fechaInicio, 23) as Fecha, 
                            Convert(varchar, fechaInicio, 8) as Hora, 
                            Convert(varchar, fechaFin, 8) as Fin
                    FROM ProfesionalesTurnos PT
                    INNER JOIN Profesionales P ON (PT.IdProfesional = P.IdProfesional)
                    WHERE PT.IdEstadoProfesionalTurno = {(int)EstadoProfesionalTurno.Disponible} AND
                        PT.fechaInicio >= {fecha}  AND
                        PT.fechaFin <= {fecha.Date.AddHours(23).AddMinutes(59)} AND
                        PT.IdEmpresa = {idEmpresa} AND
                        ({idProfesional} = 0 OR PT.IdProfesional = {idProfesional}) AND
                        ({idEspecialidad} = 0 OR P.IdEspecialidad = {idEspecialidad})
                    ORDER BY fechaInicio
                    
                ");

            var result = await query.QueryAsync<Custom.Turno>();

            return result.AsList();
        }

        public async Task<IList<Custom.Turno>> GetPrimerosTurnosDisponibles(int idEmpresa, DateTime desde, int idEspecialidad, int idProfesional)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT TOP 5 PT.IdProfesionalTurno, PT.IdProfesional, P.Nombre as Profesional, 
                            Convert(varchar, fechaInicio, 23) as Fecha, 
                            Convert(varchar, fechaInicio, 8) as Hora, 
                            Convert(varchar, fechaFin, 8) as Fin,
                            E.Descripcion as Especialidad, fechaInicio
                    FROM ProfesionalesTurnos PT
                    INNER JOIN Profesionales P ON (PT.IdProfesional = P.IdProfesional)
                    INNER JOIN Especialidades E ON (E.IdEspecialidad = P.IdEspecialidad)
                    WHERE PT.IdEstadoProfesionalTurno = {(int)EstadoProfesionalTurno.Disponible} AND
                        PT.fechaInicio >= {desde}  AND
                        PT.IdEmpresa = {idEmpresa} AND
                        ({idProfesional} = 0 OR PT.IdProfesional = {idProfesional}) AND
                        ({idEspecialidad} = 0 OR P.IdEspecialidad = {idEspecialidad})
                    ORDER BY fechaInicio ASC                                        
                ");

            var result = await query.QueryAsync<Custom.Turno>();

            return result.AsList();
        }

        //public async Task<List<Custom.Turno>> GetTurnosHoy(int idEmpresa, IDbTransaction transaction = null)
        //{
        //    var sql = BuildCustomQuery();
        //    var builder = _context.Connection
        //        .QueryBuilder(sql)
        //        .Where($@"
        //            TUR.IdEmpresa = {idEmpresa} "
        //        );

        //    var result = await builder.QueryAsync<Custom.Turno>(transaction);

        //    return result.ToList();
        //}

        private FormattableString BuildCustomQuery() =>
            $@"SELECT TUR.*,
                        ISNULL(ESP.Descripcion, 'GUARDIA') as Especialidad,
		                ISNULL(PRO.Nombre, pev.Nombre) as Profesional,
		                PAC.Nombre as Paciente,
		                Convert(varchar, TUR.fechaInicio, 23) as Fecha, 
                        Convert(varchar, TUR.fechaInicio, 8) as Hora, 
                        PAC.telefono as Telefono,
		                ETU.Descripcion as Estado,
                        ETU.Clase as EstadoClase,
                        CASE WHEN ISNULL(PAC.IdTipoIva, 0) = 0 THEN 1 ELSE 0 END as FichaIncompleta,
                        TTU.Siglas as TipoTurno,
                        TTU.Descripcion as TipoTurnoDescripcion,
                        Emp.NombreFantasia as Empresa,
                        Emp.Direccion as EmpresaDireccion,
                        Emp.Telefono as EmpresaTelefono,
                        Emp.Email as EmpresaEmail,
                        ISNULL(f.Nombre, '') AS Financiador,
                        ISNULL(fp.Nombre, '') AS FinanciadorPlan,
                        PAC.Email as Email,
                        PAC.Documento as Documento
                FROM Turnos TUR
                LEFT JOIN Especialidades ESP ON (TUR.IdEspecialidad = ESP.IdEspecialidad)
                LEFT JOIN Profesionales PRO ON (TUR.IdProfesional = PRO.IdProfesional)
                INNER JOIN Pacientes PAC ON (TUR.IdPaciente = PAC.IdPaciente)
                INNER JOIN EstadoTurno ETU ON (TUR.IdEstadoTurno = ETU.IdEstadoTurno)
                INNER JOIN TipoTurnos TTU ON (TTU.IdTipoTurno = TUR.IdTipoTurno) 
                INNER JOIN Empresas Emp ON (Emp.IdEmpresa = TUR.IdEmpresa)
                LEFT JOIN Financiadores f
						ON (f.IdFinanciador = PAC.IdFinanciador)
                    LEFT JOIN FinanciadoresPlanes fp
						ON (fp.IdFinanciadorPlan = PAC.IdFinanciadorPlan) 
                LEFT JOIN Evoluciones ev ON (ev.idTurno = TUR.idTurno)
                LEFT JOIN Profesionales pev ON (pev.idProfesional = ev.idProfesional)";

        public async Task<List<Custom.Turno>> GetTurnosByIdPaciente(int idPaciente, IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
                .QueryBuilder(sql)
                .Where($@"
                    TUR.IdPaciente = {idPaciente} "
                );

            var result = await builder.QueryAsync<Custom.Turno>(transaction);

            return result.ToList();
        }

        public DataTablesCustomQuery GetTurnos(IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
                .QueryBuilder(sql);

            //.Where($@"
            //    TUR.IdEmpresa = {idEmpresa} 
            //    AND TUR.IdEstadoTurno != {(int)EstadoTurno.Recepcionado} "
            //);

            return base.GetAllCustomQuery(builder);
        }

        public DataTablesCustomQuery GetSalaEspera(IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
                .QueryBuilder(sql);

            return base.GetAllCustomQuery(builder);
        }

        public DataTablesCustomQuery GetGuardia(IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
                .QueryBuilder(sql);

            return base.GetAllCustomQuery(builder);
        }

        public DataTablesCustomQuery GetTurnosReagendar(int idEmpresa, int idEspecialidad, int idProfesional, IDbTransaction transaction = null)
        {
            FormattableString sql = $@"SELECT PT.fechaInicio, PT.IdProfesionalTurno, PT.IdProfesional, P.Nombre as Profesional, 
                                                Convert(varchar, fechaInicio, 23) as Fecha, 
                                                Convert(varchar, fechaInicio, 8) as Hora, 
                                                Convert(varchar, fechaFin, 8) as Fin
                                        FROM ProfesionalesTurnos PT
                                        INNER JOIN Profesionales P ON (PT.IdProfesional = P.IdProfesional) ";


            var builder = _context.Connection
                .QueryBuilder(sql)
                .Where($@"
                        PT.IdEstadoProfesionalTurno = {(int)EstadoProfesionalTurno.Disponible} AND
                        PT.fechaInicio >= {DateTime.Now.ToString("yyyy-MM-dd HH:mm")}  AND
                        PT.IdEmpresa = {idEmpresa} AND
                        ({idProfesional} = 0 OR PT.IdProfesional = {idProfesional}) AND
                        P.IdEspecialidad = {idEspecialidad} "
                );

            return base.GetAllCustomQuery(builder);
        }

        public async Task<Custom.TurnosTotales> GetTotalesByEstado(int idEmpresa, int idProfesional, int idPaciente)
        {
            FormattableString sql = $@"SELECT SUM(CASE WHEN IdEstadoTurno = {(int)EstadoTurno.Cancelado} AND YEAR(FechaInicio) = YEAR(getdate()) AND MONTH(FechaInicio) = MONTH(getdate()) AND DAY(FechaInicio) = DAY(getdate()) THEN 1 ELSE 0 END) as CanceladosHoy,
		                                    SUM(CASE WHEN IdEstadoTurno = {(int)EstadoTurno.Cancelado} AND YEAR(FechaInicio) = YEAR(getdate()) AND MONTH(FechaInicio) = MONTH(getdate()) THEN 1 ELSE 0 END) 
                                                                                                                                                                                as CanceladosMensual,
		                                    SUM(CASE WHEN IdEstadoTurno = {(int)EstadoTurno.Recepcionado} AND YEAR(FechaInicio) = YEAR(getdate()) AND MONTH(FechaInicio) = MONTH(getdate()) AND DAY(FechaInicio) = DAY(getdate()) THEN 1 ELSE 0 END) as RecepcionadosHoy,
		                                    SUM(CASE WHEN IdEstadoTurno = {(int)EstadoTurno.Recepcionado} AND YEAR(FechaInicio) = YEAR(getdate()) AND MONTH(FechaInicio) = MONTH(getdate()) THEN 1 ELSE 0 END) 
                                                                                                                                                                                as RecepcionadosMensual,
		                                    SUM(CASE WHEN IdEstadoTurno = {(int)EstadoTurno.Atendido} AND YEAR(FechaInicio) = YEAR(getdate()) AND MONTH(FechaInicio) = MONTH(getdate()) AND DAY(FechaInicio) = DAY(getdate()) THEN 1 ELSE 0 END) as AtendidosHoy,
		                                    SUM(CASE WHEN IdEstadoTurno = {(int)EstadoTurno.Atendido} AND YEAR(FechaInicio) = YEAR(getdate()) AND MONTH(FechaInicio) = MONTH(getdate()) THEN 1 ELSE 0 END) 
                                                                                                                                                                                as AtendidosMensual,
                                            SUM(CASE WHEN IdTipoTurno = {(int)TipoTurno.SobreTurno} AND YEAR(FechaInicio) = YEAR(getdate()) AND MONTH(FechaInicio) = MONTH(getdate()) AND DAY(FechaInicio) = DAY(getdate()) THEN 1 ELSE 0 END) as SobreTurnosHoy,
		                                    SUM(CASE WHEN IdTipoTurno = {(int)TipoTurno.SobreTurno} AND YEAR(FechaInicio) = YEAR(getdate()) AND MONTH(FechaInicio) = MONTH(getdate()) THEN 1 ELSE 0 END) 
                                                                                                                                                                                as SobreTurnosMensual,
                                            SUM(CASE WHEN (IdTipoTurno = {(int)EstadoTurno.Agendado} OR IdTipoTurno = {(int)EstadoTurno.ReAgendado}) AND YEAR(FechaInicio) = YEAR(getdate()) AND MONTH(FechaInicio) = MONTH(getdate()) AND DAY(FechaInicio) = DAY(getdate()) THEN 1 ELSE 0 END) as AgendadosHoy,
		                                    SUM(CASE WHEN (IdTipoTurno = {(int)EstadoTurno.Agendado} OR IdTipoTurno = {(int)EstadoTurno.ReAgendado}) AND YEAR(FechaInicio) = YEAR(getdate()) AND MONTH(FechaInicio) = MONTH(getdate()) THEN 1 ELSE 0 END) 
                                                                                                                                                                                as AgendadosMensual
                                    FROM Turnos T 
                                    WHERE T.IdEmpresa = {idEmpresa} 
                                    AND ({idProfesional} = 0 OR T.IdProfesional = {idProfesional})
                                    AND ({idPaciente} = 0 OR T.IdPaciente = {idPaciente}) ";

            var builder = _context.Connection
                .QueryBuilder(sql);

            return await builder.QuerySingleAsync<Custom.TurnosTotales>();
        }

        public async Task<List<Custom.Turno>> GetSobreTurnos(DateTime desde, DateTime hasta, int idEmpresa, int idProfesional, IDbTransaction transaction = null)
        { 
            FormattableString sql = $@"SELECT TUR.*,
                            ESP.Descripcion as Especialidad,
		                    PRO.Nombre as Profesional,
		                    PAC.Nombre as Paciente,
		                    Convert(varchar, TUR.fechaInicio, 23) as Fecha, 
                            Convert(varchar, TUR.fechaInicio, 8) as Hora, 
                            PAC.telefono as Telefono,
		                    ETU.Descripcion as Estado,
                            ETU.Clase as EstadoClase,
                            CASE WHEN ISNULL(PAC.IdTipoIva, 0) = 0 THEN 1 ELSE 0 END as FichaIncompleta,
                            TTU.Siglas as TipoTurno
                    FROM Turnos TUR
                    INNER JOIN Especialidades ESP ON (TUR.IdEspecialidad = ESP.IdEspecialidad)
                    INNER JOIN Profesionales PRO ON (TUR.IdProfesional = PRO.IdProfesional)
                    INNER JOIN Pacientes PAC ON (TUR.IdPaciente = PAC.IdPaciente)
                    INNER JOIN EstadoTurno ETU ON (TUR.IdEstadoTurno = ETU.IdEstadoTurno)
                    INNER JOIN TipoTurnos TTU ON (TTU.IdTipoTurno = TUR.IdTipoTurno) ";

            var builder = _context.Connection
                .QueryBuilder(sql)
                .Where($@"
                        TUR.IdEstadoTurno != {(int)EstadoTurno.Cancelado} AND
                        TUR.fechaInicio >= {desde}  AND
                        TUR.fechaFin <= {hasta} AND
                        TUR.IdEmpresa = {idEmpresa} AND
                        ({idProfesional} = 0 OR TUR.IdProfesional = {idProfesional}) AND
                        TUR.IdTipoTurno = {(int)TipoTurno.SobreTurno} "
            );

            var result = await builder.QueryAsync<Custom.Turno>();

            return result.AsList();
        }
        public async Task<Custom.Turno> GetByIdProfesionalTurnoCustom(int idProfesionalTurno, IDbTransaction transaction = null)
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
                        CASE WHEN ISNULL(PAC.IdTipoIva, 0) = 0 THEN 1 ELSE 0 END as FichaIncompleta,
                        TTU.Siglas as TipoTurno
                FROM Turnos TUR
                INNER JOIN Especialidades ESP ON (TUR.IdEspecialidad = ESP.IdEspecialidad)
                INNER JOIN Profesionales PRO ON (TUR.IdProfesional = PRO.IdProfesional)
                INNER JOIN Pacientes PAC ON (TUR.IdPaciente = PAC.IdPaciente)
                INNER JOIN EstadoTurno ETU ON (TUR.IdEstadoTurno = ETU.IdEstadoTurno)
                INNER JOIN TipoTurnos TTU ON (TTU.IdTipoTurno = TUR.IdTipoTurno) ")
                .Where($"TUR.IdProfesionalTurno = {idProfesionalTurno}");

            return await builder.QuerySingleOrDefaultAsync<Custom.Turno>(transaction);
        }

        public async Task<IList<Custom.Turno>> GetTurnosByFecha(int idEmpresa, DateTime fecha, int idProfesional = 0, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT TUR.*,
                                        ESP.Descripcion as Especialidad,
		                                PRO.Nombre as Profesional,
		                                PAC.Nombre as Paciente,
		                                Convert(varchar, TUR.fechaInicio, 23) as Fecha, 
                                        Convert(varchar, TUR.fechaInicio, 8) as Hora, 
                                        PAC.telefono as Telefono,
		                                ETU.Descripcion as Estado,
                                        ETU.Clase as EstadoClase,
                                        CASE WHEN ISNULL(PAC.IdTipoIva, 0) = 0 THEN 1 ELSE 0 END as FichaIncompleta,
                                        TTU.Siglas as TipoTurno
                                FROM Turnos TUR
                                INNER JOIN Especialidades ESP ON (TUR.IdEspecialidad = ESP.IdEspecialidad)
                                INNER JOIN Profesionales PRO ON (TUR.IdProfesional = PRO.IdProfesional)
                                INNER JOIN Pacientes PAC ON (TUR.IdPaciente = PAC.IdPaciente)
                                INNER JOIN EstadoTurno ETU ON (TUR.IdEstadoTurno = ETU.IdEstadoTurno)
                                INNER JOIN TipoTurnos TTU ON (TTU.IdTipoTurno = TUR.IdTipoTurno)
                    WHERE TUR.fechaInicio >= {fecha}  AND
                        TUR.fechaFin <= {fecha.AddHours(23).AddMinutes(59)} AND
                        TUR.IdEmpresa = {idEmpresa} AND
                        ({idProfesional} = 0 OR TUR.IdProfesional = {idProfesional})
                    ORDER BY fechaInicio
                    
                ");

            var result = await query.QueryAsync<Custom.Turno>(transaction);

            return result.AsList();
        }

        public async Task<List<Select2Custom>> GetMedicamentos(string q)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"v.IdVademecum Id")
                .Select($"(v.PrincipioActivo + ' ' + v.Potencia + ' - (' + v.NombreComercial + ')') Text")
                .From($"Vademecum v")
                .Where($"(v.PrincipioActivo + ' ' + v.Potencia + ' ' + v.NombreComercial) LIKE {string.Concat("%", q, "%")}");

            var result = await query.QueryAsync<Custom.Select2Custom>();

            return result.AsList();
        }

        public async Task<Custom.TurnosTotales> GetTotalesDashboard(int idEmpresa, int idProfesional)
        {
            FormattableString sql = $@"SELECT SUM(AgendadosHoy) AS AgendadosHoy, 
                                            SUM(AgendadosManiana) AS AgendadosManiana, 
                                            SUM(ConfirmadosManiana) AS ConfirmadosManiana, 
                                            SUM(TotalHoy) AS TotalHoy 
                                    FROM (
                                        SELECT SUM(CASE WHEN IdEstadoTurno != {(int)EstadoTurno.Cancelado} AND YEAR(FechaInicio) = YEAR(getdate()) AND MONTH(FechaInicio) = MONTH(getdate()) AND DAY(FechaInicio) = DAY(getdate()) THEN 1 ELSE 0 END) as AgendadosHoy, 

                                                SUM(CASE WHEN IdEstadoTurno != {(int)EstadoTurno.Cancelado} AND YEAR(FechaInicio) = YEAR(getdate()) AND MONTH(FechaInicio) = MONTH(getdate()) AND DAY(FechaInicio) = DAY(getdate() + 1) THEN 1 ELSE 0 END) as AgendadosManiana, 
		                                        SUM(CASE WHEN IdEstadoTurno = {(int)EstadoTurno.Confirmado} AND YEAR(FechaInicio) = YEAR(getdate()) AND MONTH(FechaInicio) = MONTH(getdate()) AND DAY(FechaInicio) = DAY(getdate() + 1) THEN 1 ELSE 0 END) as ConfirmadosManiana, 
                                                0 as TotalHoy
                                        FROM Turnos T 
                                        WHERE T.IdEmpresa = {idEmpresa} 
                                        AND ({idProfesional} = 0 OR T.IdProfesional = {idProfesional})

                                        UNION ALL

                                        SELECT 0, 0, 0, COUNT(*) as TotalHoy
                                        FROM ProfesionalesTurnos PT
                                        INNER JOIN Profesionales P ON (PT.IdProfesional = P.IdProfesional)
                                        WHERE PT.fechaInicio >= {DateTime.Now.Date}  AND
                                            PT.fechaFin <= {DateTime.Now.Date.AddDays(1)} AND
                                            PT.IdEmpresa = {idEmpresa} AND
                                            ({idProfesional} = 0 OR PT.IdProfesional = {idProfesional}) 
                                    ) AS vwTotales ";

            var builder = _context.Connection
                .QueryBuilder(sql);

            return await builder.QuerySingleAsync<Custom.TurnosTotales>();
        }

        public async Task<List<Custom.Turno>> GetReporteTurnos(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                            .QueryBuilder($@"
                                    SELECT TUR.*,
                        ISNULL(ESP.Descripcion, 'GUARDIA') as Especialidad,
		                ISNULL(PRO.Nombre, pev.Nombre) as Profesional,
		                PAC.Nombre as Paciente,
		                Convert(varchar, TUR.fechaInicio, 23) as Fecha, 
                        Convert(varchar, TUR.fechaInicio, 8) as Hora, 
                        PAC.telefono as Telefono,
		                ETU.Descripcion as Estado,
                        ETU.Clase as EstadoClase,
                        CASE WHEN ISNULL(PAC.IdTipoIva, 0) = 0 THEN 1 ELSE 0 END as FichaIncompleta,
                        TTU.Siglas as TipoTurno,
                        TTU.Descripcion as TipoTurnoDescripcion,
                        Emp.NombreFantasia as Empresa,
                        Emp.Direccion as EmpresaDireccion,
                        Emp.Telefono as EmpresaTelefono,
                        Emp.Email as EmpresaEmail,
                        ISNULL(f.Nombre, '') AS Financiador,
                        ISNULL(fp.Nombre, '') AS FinanciadorPlan,
                        PAC.Email as Email,
                        PAC.Documento as Documento
                FROM Turnos TUR
                LEFT JOIN Especialidades ESP ON (TUR.IdEspecialidad = ESP.IdEspecialidad)
                LEFT JOIN Profesionales PRO ON (TUR.IdProfesional = PRO.IdProfesional)
                INNER JOIN Pacientes PAC ON (TUR.IdPaciente = PAC.IdPaciente)
                INNER JOIN EstadoTurno ETU ON (TUR.IdEstadoTurno = ETU.IdEstadoTurno)
                INNER JOIN TipoTurnos TTU ON (TTU.IdTipoTurno = TUR.IdTipoTurno) 
                INNER JOIN Empresas Emp ON (Emp.IdEmpresa = TUR.IdEmpresa)
                LEFT JOIN Financiadores f
						ON (f.IdFinanciador = PAC.IdFinanciador)
                    LEFT JOIN FinanciadoresPlanes fp
						ON (fp.IdFinanciadorPlan = PAC.IdFinanciadorPlan) 
                LEFT JOIN Evoluciones ev ON (ev.idTurno = TUR.idTurno)
                LEFT JOIN Profesionales pev ON (pev.idProfesional = ev.idProfesional)
                /**where**/                
            ");

            builder.Where($"TUR.IdEmpresa = {filters["IdEmpresa"]}");
            /*builder.Append($"AND TUR.idTipoTurno IN ({ (int)EstadoTurno.Atendido} , {(int)EstadoTurno.AusenteConAviso}, {(int)EstadoTurno.AusenteSinAviso}, {(int)EstadoTurno.Agendado}, {(int)EstadoTurno.ReAgendado})";*/
            if (filters.ContainsKey("IdTipoTurno"))
                builder.Append($"AND TUR.IdTipoTurno = {filters["IdTipoTurno"]}");

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND TUR.FechaInicio >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
            {
                var date = DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null);
                builder.Append($"AND TUR.FechaInicio <= {date.AddDays(1)}");
            }

            if (filters.ContainsKey("IdProfesional"))
                builder.Append($"AND TUR.IdProfesional = {filters["IdProfesional"]}");


            if (filters.ContainsKey("IdFinanciador"))
                builder.Append($"AND TUR.IdFinanciador = {filters["IdFinanciador"]}");


            return (await builder.QueryAsync<Custom.Turno>()).AsList();
        }

        public async Task<List<Custom.TurnosEstadisticasEstados>> GetEstadisticasEstados(int idEmpresa, DateTime desde, DateTime hasta, int idProfesional, int idEspecialidad, IDbTransaction transaction = null)
        {
            FormattableString sql = $@"SELECT ET.Descripcion as Estado,
                                                COUNT(*) as Cantidad
                                        FROM TurnosHistorial TH (NOLOCK)
                                        INNER JOIN (
	                                        SELECT TH.IdTurno, 
			                                        MAX(TH.[fecha]) as lastFechaEstado
	                                        FROM TurnosHistorial TH (NOLOCK)
	                                        INNER JOIN Turnos T (NOLOCK) ON (T.IdTurno = TH.IdTurno)
	                                        INNER JOIN Evoluciones E (NOLOCK) ON (E.IdTurno = TH.IdTurno)
	                                        WHERE T.IdTipoTurno = {(int)TipoTurno.Turno} 
                                                    AND TH.IdEstadoTurno NOT IN ({(int)EstadoTurno.Recepcionado},{(int)EstadoTurno.Atendido}) 
                                                    AND T.FechaInicio BETWEEN {desde.Date} AND {hasta.Date.AddDays(1)}
                                                    AND ({idProfesional} = 0 OR T.idProfesional = {idProfesional}) 
                                                    AND ({idEspecialidad} = 0 OR T.idEspecialidad = {idEspecialidad}) 
	                                        GROUP BY TH.IdTurno
                                        ) as vwTH ON (TH.IdTurno = vwTH.IdTurno AND TH.fecha = vwTH.lastFechaEstado)
                                        INNER JOIN EstadoTurno ET (NOLOCK) ON (ET.IdEstadoTurno = TH.IdEstadoTurno)
                                        GROUP BY ET.Descripcion";

            var builder = _context.Connection
                .QueryBuilder(sql);

            var results = await builder.QueryAsync<Custom.TurnosEstadisticasEstados>(transaction);

            return results.AsList();
        }

        public async Task<Custom.TurnosEstadisticasAusentes> GetEstadisticasAusentes(int idEmpresa, DateTime desde, DateTime hasta, int idProfesional, int idEspecialidad)
        {
            FormattableString sql = $@"SELECT SUM(CASE WHEN IdEstadoTurno = {(int)EstadoTurno.AusenteConAviso} THEN 1 ELSE 0 END) as ConAviso,
		                                        SUM(CASE WHEN IdEstadoTurno = {(int)EstadoTurno.AusenteSinAviso} THEN 1 ELSE 0 END) as SinAviso,
		                                        SUM(CASE WHEN IdEstadoTurno = {(int)EstadoTurno.Cancelado} THEN 1 ELSE 0 END) as Cancelado
                                        FROM Turnos T (NOLOCK)
                                        WHERE IdEstadoTurno in ({(int)EstadoTurno.AusenteConAviso}, {(int)EstadoTurno.AusenteSinAviso}, {(int)EstadoTurno.Cancelado})
                                                AND T.idEmpresa = {idEmpresa} 
                                                AND T.FechaInicio BETWEEN {desde.Date} AND {hasta.Date.AddDays(1)}
                                                AND ({idProfesional} = 0 OR T.idProfesional = {idProfesional}) 
                                                AND ({idEspecialidad}= 0 OR T.idEspecialidad = {idEspecialidad}) ";

            var builder = _context.Connection
                .QueryBuilder(sql);

            return await builder.QuerySingleAsync<Custom.TurnosEstadisticasAusentes>();
        }
    }
}
