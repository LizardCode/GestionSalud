using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class PedidosLaboratoriosHistorialRepository : BaseRepository, IPedidosLaboratoriosHistorialRepository
    {
        public PedidosLaboratoriosHistorialRepository(IDbContext context) : base(context)
        {

        }
        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection.QueryBuilder(sql);

            return base.GetAllCustomQuery(builder);
        }

        private FormattableString BuildCustomQuery() =>
                $@"SELECT PLH.*, 
		                    EPL.descripcion as [Estado], 
		                    EPL.clase as [EstadoClase],
		                    u.nombre as [Usuario]
                    FROM PedidosLaboratoriosHistorial PLH
                    INNER JOIN Usuarios u ON (u.IdUsuario = PLH.IdUsuario)
                    INNER JOIN EstadoPedidoLaboratorio EPL ON (PLH.idEstadoPedidoLaboratorio = EPL.idEstadoPedidoLaboratorio) ";


        public DataTablesCustomQuery GetHistorial(int idPedidoLaboratorio)
        {
            FormattableString sql = $@"SELECT PLH.*, 
		                                EPL.descripcion as [Estado], 
		                                EPL.clase as [EstadoClase],
		                                u.nombre as [Usuario]
                                FROM PedidosLaboratoriosHistorial PLH
                                INNER JOIN Usuarios u ON (u.IdUsuario = PLH.IdUsuario)
                                INNER JOIN EstadoPedidoLaboratorio EPL ON (PLH.idEstadoPedidoLaboratorio = EPL.idEstadoPedidoLaboratorio) 
                                WHERE PLH.idPedidoLaboratorio = {idPedidoLaboratorio} ";

            var builder = _context.Connection.QueryBuilder(sql);

            return base.GetAllCustomQuery(builder);
        }
    }
}
