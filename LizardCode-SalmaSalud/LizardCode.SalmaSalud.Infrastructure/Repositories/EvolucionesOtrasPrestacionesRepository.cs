using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class EvolucionesOtrasPrestacionesRepository : BaseRepository, IEvolucionesOtrasPrestacionesRepository
    {
        public EvolucionesOtrasPrestacionesRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<EvolucionOtraPrestacion>> GetAllByIdEvolucion(int idEvolucion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ei.*
                    FROM EvolucionesOtrasPrestaciones ei
                    WHERE
                        ei.idEvolucion = {idEvolucion}");

            var results = await builder.QueryAsync<EvolucionOtraPrestacion>(transaction);

            return results.AsList();
        }

        public async Task<bool> DeleteByIdEvolucion(int idEvolucion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE ei FROM EvolucionesOtrasPrestaciones ei                        
                    WHERE 
                        ei.idEvolucion = {idEvolucion}");

            var results = await builder.ExecuteAsync(transaction);

            return (results > 1);
        }

        public async Task<List<Custom.PrestacionProfesional>> GetPrestacionesProfesional(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT evp.idEvolucionPrestacion,
                            ev.IdProfesional,
                            ev.IdEmpresa,
		                    ev.fecha,
		                    p.nombre as Profesional,
		                    ISNULL(e.Descripcion, 'GUARDIA') as Especialidad,
		                    evp.descripcion as Prestacion,
		                    evp.codigo, 
		                    evp.valor
                    FROM EvolucionesOtrasPrestaciones evp
                    INNER JOIN Evoluciones ev ON evp.idEvolucion = ev.idEvolucion
                    INNER JOIN Profesionales p ON ev.idProfesional = p.idProfesional
                    LEFT  JOIN Especialidades e ON ev.idEspecialidad = e.IdEspecialidad
                    /**where**/
                ");

            if (filters.ContainsKey("IdProfesional"))
                builder.Append($"AND IdProfesional = {filters["IdProfesional"]}");

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
            {
                var date = DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null);
                builder.Append($"AND Fecha <= {date.AddDays(1)}");
            }

            builder.Where($"IdEmpresa = {filters["IdEmpresa"]}");

            return (await builder.QueryAsync<Custom.PrestacionProfesional>()).AsList();
        }

        public async Task<IList<Custom.EvolucionOtraPrestacion>> GetPrestacionesALiquidar(DateTime desde, DateTime hasta, int idProfesional, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                        .CommandBuilder($@"
                                        SELECT ep.*,
												CONCAT(CONVERT(varchar(10), e.fecha, 103), ' - ', ep.codigo, ' - ', p.Documento, ' ', ep.descripcion) as DescripcionLiquidacion
                                        FROM EvolucionesOtrasPrestaciones ep
                                        INNER JOIN Evoluciones        e ON (e.idEvolucion = ep.idEvolucion)
                                        INNER JOIN Pacientes          p ON (p.IdPaciente = e.idPaciente)
                                        WHERE e.idEstadoRegistro != {(int)EstadoRegistro.Eliminado}
                                            AND e.fecha BETWEEN {desde} AND {hasta}
	                                        AND e.idProfesional = {idProfesional}
	                                        AND ep.idEvolucionPrestacion NOT IN (
		                                        SELECT ISNULL(lpp.idOtraPrestacion, 0)
		                                        FROM LiquidacionesProfesionalesPrestaciones lpp
		                                        INNER JOIN LiquidacionesProfesionales        lp ON (lpp.idLiquidacion = lp.idLiquidacionProfesional)
		                                        WHERE lp.idEstadoRegistro != {(int)EstadoRegistro.Eliminado} )");


            var results = await builder.QueryAsync<Custom.EvolucionOtraPrestacion>(transaction);

            return results.AsList();
        }
    }
}