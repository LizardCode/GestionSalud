using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using NLog.Filters;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
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
    public class EvolucionesPrestacionesRepository : BaseRepository, IEvolucionesPrestacionesRepository
    {
        public EvolucionesPrestacionesRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<EvolucionPrestacion>> GetAllByIdEvolucion(int idEvolucion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ei.*
                    FROM EvolucionesPrestaciones ei
                    WHERE
                        ei.idEvolucion = {idEvolucion}");

            var results = await builder.QueryAsync<EvolucionPrestacion>(transaction);

            return results.AsList();
        }

        public async Task<bool> DeleteByIdEvolucion(int idEvolucion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE ei FROM EvolucionesPrestaciones ei                        
                    WHERE 
                        ei.idEvolucion = {idEvolucion}");

            var results = await builder.ExecuteAsync(transaction);

            return (results > 1);
        }

        public async Task<List<Custom.PrestacionFinanciador>> GetPrestacionesFinanciador(Dictionary<string, object> filters)
        {
            var builder = _context.Connection
                    .QueryBuilder($@"
                    SELECT evp.idEvolucionPrestacion,
                            ev.IdFinanciador,
                            ev.IdEmpresa,
		                    ev.fecha,
		                    f.nombre as Financiador,
		                    ISNULL(e.Descripcion, 'GUARDIA') as Especialidad,
		                    evp.descripcion as Prestacion,
		                    evp.codigo, 
		                    evp.valor,
		                    evp.IdTipoPrestacion
                    FROM EvolucionesPrestaciones evp
                    INNER JOIN Evoluciones ev ON (evp.idEvolucion = ev.idEvolucion)
                    INNER JOIN Financiadores f ON (ev.idFinanciador = f.IdFinanciador)
                    LEFT  JOIN Especialidades e ON (ev.idEspecialidad = e.IdEspecialidad)
                    /**where**/                
            ");


            builder.Where($"IdEmpresa = {filters["IdEmpresa"]}");

            if (filters.ContainsKey("IdFinanciador"))
                builder.Append($"AND IdFinanciador = {filters["IdFinanciador"]}");

            if (filters.ContainsKey("IdTipoPrestacion"))
                builder.Append($"AND IdTipoPrestacion = {filters["IdTipoPrestacion"]}");

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND Fecha >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
            {
                var date = DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null);
                builder.Append($"AND Fecha <= {date.AddDays(1)}");
            }


            return (await builder.QueryAsync<Custom.PrestacionFinanciador>()).AsList();
        }

        public async Task<List<Custom.EvolucionPrestacion>> GetPrestacionesALiquidar(DateTime desde, DateTime hasta, int idProfesional, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                        .CommandBuilder($@"
                                        SELECT ep.*,
                                                f.Nombre as Financiador,
		                                        CONCAT(CONVERT(varchar(10), e.fecha, 103), ' - ', ep.codigo, ' - ', p.Documento, ' ', ep.descripcion) as DescripcionLiquidacion
                                        FROM EvolucionesPrestaciones ep
                                        INNER JOIN Evoluciones        e ON (e.idEvolucion = ep.idEvolucion)
                                        INNER JOIN Pacientes          p ON (p.IdPaciente = e.idPaciente)
                                        INNER JOIN Financiadores      f ON (e.idFinanciador = f.IdFinanciador)

                                        LEFT JOIN EvolucionesPrestaciones epcp on (epcp.IdTipoPrestacion = {(int)TipoPrestacion.CoPago} AND epcp.idEvolucionPrestacionAsociada = ep.idEvolucionPrestacion)
                                        LEFT JOIN ComprobantesVentasItems cvi on (cvi.IdEvolucionPrestacion = epcp.idEvolucionPrestacion)
                                        LEFT JOIN ComprobantesVentas cv ON (cv.IdComprobanteVenta = cvi.IdComprobanteVenta AND cv.IdEstadoRegistro != {(int)EstadoRegistro.Eliminado})

                                        WHERE e.idEstadoRegistro != {(int)EstadoRegistro.Eliminado}
                                            AND e.fecha BETWEEN {desde} AND {hasta}
	                                        AND e.idProfesional = {idProfesional}
                                            AND ep.IdTipoPrestacion = {(int)TipoPrestacion.Prestacion}
	                                        AND (ISNULL(epcp.idEvolucionPrestacion, 0) = 0 OR ISNULL(cvi.IdEvolucionPrestacion, 0) > 0)
	                                        AND ep.idEvolucionPrestacion NOT IN (
		                                        SELECT ISNULL(lpp.idPrestacion, 0)
		                                        FROM LiquidacionesProfesionalesPrestaciones lpp
		                                        INNER JOIN LiquidacionesProfesionales        lp ON (lpp.idLiquidacion = lp.idLiquidacionProfesional)
		                                        WHERE lp.idEstadoRegistro != {(int)EstadoRegistro.Eliminado} )");


            var results = await builder.QueryAsync<Custom.EvolucionPrestacion>(transaction);

            return results.AsList();
        }
    }
}