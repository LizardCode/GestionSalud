using Dapper;
using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class EvolucionesRepository : BaseRepository, IEvolucionesRepository, IDataTablesCustomQuery
    {
        public EvolucionesRepository(IDbContext context) : base(context)
        {
        }

        public async Task<List<Custom.Evolucion>> GetEvolucionesPaciente(int idPaciente, IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
            .QueryBuilder(sql)
                .Where($@"ev.idEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND ev.idPaciente = {idPaciente} ");

            var results = await builder.QueryAsync<Custom.Evolucion>(transaction);

            return results.ToList();
        }

        public async Task<Custom.Evolucion> GetCustomById(int idEvolucion, IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
            .QueryBuilder(sql)
                .Where($@"ev.idEvolucion = {idEvolucion} "
                );

            var result = await builder.QueryFirstOrDefaultAsync<Custom.Evolucion>(transaction);

            return result;
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						ev.*,
                        ISNULL(e.Descripcion, 'GUARDIA') AS Especialidad,
                        p.Nombre AS Paciente,
                        pro.Nombre AS Profesional,
		                eev.Descripcion as Estado,
                        eev.Clase as EstadoClase,
                        TTU.Siglas as TipoTurno,
                        ISNULL(Fin.Nombre, '') as Financiador,
                        ISNULL(FPa.Nombre, '') as FinanciadorPlan,
                        Emp.NombreFantasia as Empresa
                    FROM Evoluciones ev
					INNER JOIN Pacientes p
						ON p.IdPaciente = ev.IdPaciente
                    INNER JOIN Profesionales pro
						ON ev.IdProfesional = pro.IdProfesional
                    LEFT JOIN Especialidades e
						ON ev.IdEspecialidad = e.IdEspecialidad
                    INNER JOIN EstadoEvolucion eev ON (eev.IdEstadoEvolucion = ev.IdEstadoEvolucion)
                    LEFT JOIN Turnos Tur ON (Tur.IdTurno = ev.IdTurno)
                    LEFT JOIN TipoTurnos Ttu ON (Ttu.IdTipoTurno = Tur.IdTipoTurno)
                    LEFT JOIN Financiadores Fin ON (Fin.IdFinanciador = ev.IdFinanciador)
                    LEFT JOIN FinanciadoresPlanes FPa ON (FPa.IdFinanciadorPlan = ev.IdFinanciadorPlan)
                    INNER JOIN Empresas Emp ON (Emp.IdEmpresa = ev.IdEmpresa)
                ");

            return base.GetAllCustomQuery(query);
        }

        private FormattableString BuildCustomQuery() =>
            $@"
            SELECT
				ev.*,
                ISNULL(e.Descripcion, 'GUARDIA') AS Especialidad,
                p.Nombre AS Paciente,
                pro.Nombre AS Profesional,
		        eev.Descripcion as Estado,
                eev.Clase as EstadoClase,
                TTU.Siglas as TipoTurno,
                TTU.Descripcion as TipoTurnoDescripcion,
                ISNULL(Fin.Nombre, '') as Financiador,
                ISNULL(FPa.Nombre, '') as FinanciadorPlan,
                Emp.NombreFantasia as Empresa,
                Tur.FechaInicio as FechaTurno
            FROM Evoluciones ev
			INNER JOIN Pacientes p
				ON p.IdPaciente = ev.IdPaciente
            INNER JOIN Profesionales pro
				ON ev.IdProfesional = pro.IdProfesional
            LEFT JOIN Especialidades e
				ON ev.IdEspecialidad = e.IdEspecialidad
            INNER JOIN EstadoEvolucion eev ON (eev.IdEstadoEvolucion = ev.IdEstadoEvolucion)
            LEFT JOIN Turnos Tur ON (Tur.IdTurno = ev.IdTurno)
            LEFT JOIN TipoTurnos Ttu ON (Ttu.IdTipoTurno = Tur.IdTipoTurno)
            LEFT JOIN Financiadores Fin ON (Fin.IdFinanciador = ev.IdFinanciador)
            LEFT JOIN FinanciadoresPlanes FPa ON (FPa.IdFinanciadorPlan = ev.IdFinanciadorPlan)
            INNER JOIN Empresas Emp ON (Emp.IdEmpresa = ev.IdEmpresa)
            ";


        public async Task<Custom.EvolucionesEstadisticas> GetEstadisticas(int idEmpresa, DateTime desde, DateTime hasta, int idProfesional, int idEspecialidad)
        {
            FormattableString sql = $@"SELECT SUM(CASE WHEN T.IdTipoTurno = {(int)TipoTurno.Turno} THEN 1 ELSE 0 END) as Turnos,
                                            SUM(CASE WHEN T.IdTipoTurno = {(int)TipoTurno.SobreTurno} THEN 1 ELSE 0 END) as SobreTurnos,
		                                    SUM(CASE WHEN T.IdTipoTurno = {(int)TipoTurno.DemandaEspontanea} THEN 1 ELSE 0 END) as DemandaEspontanea,
		                                    SUM(CASE WHEN T.IdTipoTurno = {(int)TipoTurno.Guardia} THEN 1 ELSE 0 END) as Guardia
                                    FROM Evoluciones E (NOLOCK)
                                    INNER JOIN Turnos T (NOLOCK) ON (T.IdTurno = E.idTurno)
                                    WHERE E.idEmpresa = {idEmpresa} 
                                        AND E.Fecha BETWEEN {desde.Date} AND {hasta.Date.AddDays(1)}
                                        AND ({idProfesional} = 0 OR E.idProfesional = {idProfesional}) 
                                        AND ({idEspecialidad}= 0 OR E.idEspecialidad = {idEspecialidad}) ";

            var builder = _context.Connection
                .QueryBuilder(sql);

            return await builder.QuerySingleAsync<Custom.EvolucionesEstadisticas>();
        }

        public async Task<List<Custom.EvolucionesEstadisticasFinanciador>> GetEstadisticasFinanciador(int idEmpresa, DateTime desde, DateTime hasta, int idProfesional, int idEspecialidad, IDbTransaction transaction = null)
        {
            FormattableString sql = $@"SELECT ISNULL(F.Nombre, 'SIN COBERTURA') as Financiador,
		                                        COUNT(*) as Cantidad
                                        FROM Evoluciones E (NOLOCK)
                                        LEFT JOIN Financiadores F (NOLOCK) ON (F.IdFinanciador = E.idFinanciador)                                        
                                        WHERE E.idEmpresa = {idEmpresa} 
                                            AND E.Fecha BETWEEN {desde.Date} AND {hasta.Date.AddDays(1)}
                                            AND ({idProfesional} = 0 OR E.idProfesional = {idProfesional}) 
                                            AND ({idEspecialidad} = 0 OR E.idEspecialidad = {idEspecialidad}) 
                                        GROUP BY F.Nombre";

            var builder = _context.Connection
                .QueryBuilder(sql);

            var results = await builder.QueryAsync<Custom.EvolucionesEstadisticasFinanciador>(transaction);

            return results.AsList();
        }

        public async Task<List<Custom.EvolucionesEstadisticasEspecialidad>> GetEstadisticasEspecialidad(int idEmpresa, DateTime desde, DateTime hasta, int idProfesional, int idEspecialidad, IDbTransaction transaction = null)
        {
            FormattableString sql = $@"SELECT ISNULL(ES.Descripcion, 'GUARDIA') as Especialidad,
		                                        COUNT(*) as Cantidad
                                        FROM Evoluciones E (NOLOCK)
                                        LEFT JOIN Especialidades ES (NOLOCK) ON (ES.IdEspecialidad = E.IdEspecialidad)                                        
                                        WHERE E.idEmpresa = {idEmpresa} 
                                            AND E.Fecha BETWEEN {desde.Date} AND {hasta.Date.AddDays(1)}
                                            AND ({idProfesional} = 0 OR E.idProfesional = {idProfesional}) 
                                            AND ({idEspecialidad} = 0 OR E.idEspecialidad = {idEspecialidad}) 
                                        GROUP BY ES.Descripcion";

            var builder = _context.Connection
                .QueryBuilder(sql);

            var results = await builder.QueryAsync<Custom.EvolucionesEstadisticasEspecialidad>(transaction);

            return results.AsList();
        }
    }
}
