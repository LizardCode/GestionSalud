using Dapper;
using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class PedidosLaboratoriosRepository : BaseRepository, IPedidosLaboratoriosRepository, IDataTablesCustomQuery
    {
        public PedidosLaboratoriosRepository(IDbContext context) : base(context)
        {
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                                SELECT PL.*,
		                                EPL.descripcion as Estado,
		                                EPL.clase as EstadoClase,
		                                PA.Nombre as Paciente,
		                                PA.Documento as PacienteDocumento,
		                                F.Nombre as Financiador,
		                                FP.Nombre as FinanciadorPlan,
		                                PRO.RazonSocial as Laboratorio,
		                                PRO.CUIT as LaboratorioCUIT,
		                                U.Nombre as Usuario,
										vwPLS.Valor
                                FROM PedidosLaboratorios PL
                                INNER JOIN EstadoPedidoLaboratorio EPL ON (PL.idEstadoPedidoLaboratorio = EPL.idEstadoPedidoLaboratorio)
                                INNER JOIN Presupuestos PR ON (PR.idPresupuesto = PL.idPresupuesto)
                                INNER JOIN Pacientes PA ON (PA.IdPaciente = PR.IdPaciente)
                                INNER JOIN Proveedores PRO ON (PRO.IdProveedor = PL.idLaboratorio)
                                INNER JOIN Usuarios U ON (U.IdUsuario = PL.idUsuario)
                                LEFT JOIN Financiadores F On (F.IdFinanciador = PL.idFinanciador)
                                LEFT JOIN FinanciadoresPlanes FP On (FP.IdFinanciadorPlan = PL.idFinanciadorPlan)
								LEFT JOIN (
									SELECT idPedidoLaboratorio, SUM(PLS.valor) as Valor
									FROM PedidosLaboratoriosServicios PLS
									GROUP BY idPedidoLaboratorio
								) as vwPLS ON (vwPLS.idPedidoLaboratorio = PL.idPedidoLaboratorio)
                                ");

            return base.GetAllCustomQuery(query);
        }


        public async Task<IList<Custom.PedidoLaboratorio>> GetPedidosPorPresupuesto(int idPresupuesto)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                                SELECT PL.*,
		                                EPL.descripcion as Estado,
		                                EPL.clase as EstadoClase,
		                                PA.Nombre as Paciente,
		                                PA.Documento as PacienteDocumento,
		                                F.Nombre as Financiador,
		                                FP.Nombre as FinanciadorPlan,
		                                PRO.RazonSocial as Laboratorio,
		                                PRO.CUIT as LaboratorioCUIT,
		                                U.Nombre as Usuario,
										vwPLS.Valor
                                FROM PedidosLaboratorios PL
                                INNER JOIN EstadoPedidoLaboratorio EPL ON (PL.idEstadoPedidoLaboratorio = EPL.idEstadoPedidoLaboratorio)
                                INNER JOIN Presupuestos PR ON (PR.idPresupuesto = PL.idPresupuesto)
                                INNER JOIN Pacientes PA ON (PA.IdPaciente = PR.IdPaciente)
                                INNER JOIN Proveedores PRO ON (PRO.IdProveedor = PL.idLaboratorio)
                                INNER JOIN Usuarios U ON (U.IdUsuario = PL.idUsuario)
                                LEFT JOIN Financiadores F On (F.IdFinanciador = PL.idFinanciador)
                                LEFT JOIN FinanciadoresPlanes FP On (FP.IdFinanciadorPlan = PL.idFinanciadorPlan)
								LEFT JOIN (
									SELECT idPedidoLaboratorio, SUM(PLS.valor) as Valor
									FROM PedidosLaboratoriosServicios PLS
									GROUP BY idPedidoLaboratorio
								) as vwPLS ON (vwPLS.idPedidoLaboratorio = PL.idPedidoLaboratorio)           
                                WHERE PL.idEstadoRegistro != {(int)EstadoRegistro.Eliminado} AND PL.idPresupuesto = {idPresupuesto}
                ");

            var result = await query.QueryAsync<Custom.PedidoLaboratorio>();

            return result.AsList();
        }
    }
}