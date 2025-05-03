using Dapper;
using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class PresupuestosRepository : BaseRepository, IPresupuestosRepository, IDataTablesCustomQuery
    {
        public PresupuestosRepository(IDbContext context) : base(context)
        {
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                                SELECT 
                                    c.idPresupuesto,
                                    c.fecha,
                                    c.idEstadoPresupuesto,
                                    ep.Descripcion as Estado,
                                    ep.Clase as EstadoClase,
                                    p.Nombre as Paciente,
                                    p.Documento as PacienteDocumento,
                                    u.Nombre as Usuario,
                                    ISNULL(scp.coPago, 0) as TotalCoPagos,
                                    ISNULL(spv.valor, 0) as TotalPrestaciones,
                                    ISNULL(scp.coPago, 0) + ISNULL(spv.valor, 0) as Total,
	                                CASE WHEN ISNULL(ppl.idPresupuesto, 0) > 0 THEN 1 ELSE 0 END as EnPedido
                                FROM Presupuestos c
                                INNER JOIN EstadoPresupuesto ep ON ep.IdEstadoPresupuesto = c.IdEstadoPresupuesto
                                INNER JOIN Pacientes p ON p.IdPaciente = c.IdPaciente
                                INNER JOIN Usuarios u ON u.IdUsuario = c.IdUsuario
                                LEFT JOIN (
		                            SELECT pp.idPresupuesto, SUM(PP.coPago) as coPago
		                            FROM PresupuestosPrestaciones PP
		                            GROUP BY pp.idPresupuesto
	                            ) AS scp ON scp.idPresupuesto = c.idPresupuesto
	                            LEFT JOIN (
		                            SELECT pop.idPresupuesto, SUM(pop.valor) as valor
		                            FROM PresupuestosOtrasPrestaciones POP
		                            GROUP BY pop.idPresupuesto
	                            ) AS spv ON spv.idPresupuesto = c.idPresupuesto
                                LEFT JOIN (
	                                SELECT idPresupuesto
	                                FROM PedidosLaboratorios
	                                WHERE idEstadoRegistro != {(int)EstadoRegistro.Eliminado}
	                                GROUP BY idPresupuesto
                                ) AS ppl ON ppl.idPresupuesto = c.idPresupuesto
                                GROUP BY c.idPresupuesto,
                                        c.fecha,
                                        c.idEstadoPresupuesto,
                                        ep.Descripcion,
                                        ep.Clase,
                                        p.Nombre,
                                        p.Documento,
                                        u.Nombre,
			                            scp.coPago,
			                            spv.valor,
		                                ppl.idPresupuesto
                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<IList<Custom.Presupuesto>> GetPresupuestosAprobados(int idEmpresa, int idPaciente)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                        SELECT
                            c.idPresupuesto,
                            c.fecha,
                            c.idEstadoPresupuesto,
                            ep.Descripcion as Estado,
                            ep.Clase as EstadoClase,
                            p.Nombre as Paciente,
                            p.Documento as PacienteDocumento,
                            u.Nombre as Usuario,
                            ISNULL(scp.coPago, 0) as TotalCoPagos,
                            ISNULL(spv.valor, 0) as TotalPrestaciones,
                            ISNULL(scp.coPago, 0) + ISNULL(spv.valor, 0) as Total,
	                        CASE WHEN ISNULL(ppl.idPresupuesto, 0) > 0 THEN 1 ELSE 0 END as EnPedido
                        FROM Presupuestos c
                        INNER JOIN EstadoPresupuesto ep ON ep.IdEstadoPresupuesto = c.IdEstadoPresupuesto
                        INNER JOIN Pacientes p ON p.IdPaciente = c.IdPaciente
                        INNER JOIN Usuarios u ON u.IdUsuario = c.IdUsuario
                        LEFT JOIN (
		                    SELECT pp.idPresupuesto, SUM(PP.coPago) as coPago
		                    FROM PresupuestosPrestaciones PP
		                    GROUP BY pp.idPresupuesto
	                    ) AS scp ON scp.idPresupuesto = c.idPresupuesto
	                    LEFT JOIN (
		                    SELECT pop.idPresupuesto, SUM(pop.valor) as valor
		                    FROM PresupuestosOtrasPrestaciones POP
		                    GROUP BY pop.idPresupuesto
	                    ) AS spv ON spv.idPresupuesto = c.idPresupuesto
                        LEFT JOIN (
	                        SELECT idPresupuesto
	                        FROM PedidosLaboratorios
	                        WHERE idEstadoRegistro != {(int)EstadoRegistro.Eliminado}
	                        GROUP BY idPresupuesto
                        ) AS ppl ON ppl.idPresupuesto = c.idPresupuesto
                        WHERE ISNULL(c.idEvolucion, 0) = 0 AND c.idEmpresa = {idEmpresa} AND c.idPaciente = {idPaciente} AND c.idEstadoPresupuesto = {(int)EstadoPresupuesto.Aprobado}
                        GROUP BY c.idPresupuesto,
                                c.fecha,
                                c.idEstadoPresupuesto,
                                ep.Descripcion,
                                ep.Clase,
                                p.Nombre,
                                p.Documento,
                                u.Nombre,
			                    scp.coPago,
			                    spv.valor,
		                        ppl.idPresupuesto
                    ORDER BY c.fecha ASC                                        
                ");

            var result = await query.QueryAsync<Custom.Presupuesto>();

            return result.AsList();
        }

        public async Task<IList<Custom.Presupuesto>> GetPresupuestosAprobadosDisponibles(int idEmpresa)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                        SELECT
                            c.idPresupuesto,
                            c.fecha,
                            c.idEstadoPresupuesto,
                            ep.Descripcion as Estado,
                            ep.Clase as EstadoClase,
                            p.Nombre as Paciente,
                            p.Documento as PacienteDocumento,
                            u.Nombre as Usuario,
                            ISNULL(scp.coPago, 0) as TotalCoPagos,
                            ISNULL(spv.valor, 0) as TotalPrestaciones,
                            ISNULL(scp.coPago, 0) + ISNULL(spv.valor, 0) as Total,
	                        CASE WHEN ISNULL(ppl.idPresupuesto, 0) > 0 THEN 1 ELSE 0 END as EnPedido
                        FROM Presupuestos c
                        INNER JOIN EstadoPresupuesto ep ON ep.IdEstadoPresupuesto = c.IdEstadoPresupuesto
                        INNER JOIN Pacientes p ON p.IdPaciente = c.IdPaciente
                        INNER JOIN Usuarios u ON u.IdUsuario = c.IdUsuario
                        LEFT JOIN (
		                    SELECT pp.idPresupuesto, SUM(PP.coPago) as coPago
		                    FROM PresupuestosPrestaciones PP
		                    GROUP BY pp.idPresupuesto
	                    ) AS scp ON scp.idPresupuesto = c.idPresupuesto
	                    LEFT JOIN (
		                    SELECT pop.idPresupuesto, SUM(pop.valor) as valor
		                    FROM PresupuestosOtrasPrestaciones POP
		                    GROUP BY pop.idPresupuesto
	                    ) AS spv ON spv.idPresupuesto = c.idPresupuesto
                        LEFT JOIN (
	                        SELECT idPresupuesto
	                        FROM PedidosLaboratorios
	                        WHERE idEstadoRegistro != {(int)EstadoRegistro.Eliminado}
	                        GROUP BY idPresupuesto
                        ) AS ppl ON ppl.idPresupuesto = c.idPresupuesto
                        WHERE ISNULL(c.idEvolucion, 0) = 0 AND c.idEmpresa = {idEmpresa} AND c.idEstadoPresupuesto = {(int)EstadoPresupuesto.Aprobado}
                        GROUP BY c.idPresupuesto,
                                c.fecha,
                                c.idEstadoPresupuesto,
                                ep.Descripcion,
                                ep.Clase,
                                p.Nombre,
                                p.Documento,
                                u.Nombre,
			                    scp.coPago,
			                    spv.valor,
		                        ppl.idPresupuesto
                    ORDER BY c.fecha ASC                                        
                ");

            var result = await query.QueryAsync<Custom.Presupuesto>();

            return result.AsList();
        }

        public async Task<bool> PresupuestoEnPedido(long idPresupuesto, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT COUNT(*)
                    FROM PedidosLaboratorios
                    WHERE idEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND idPresupuesto = {idPresupuesto} ");

            var result = await builder.QuerySingleAsync<int>(transaction);

            return result > 0;
        }
    }
}
