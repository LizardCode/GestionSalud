using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;
using Dapper.DataTables.Models;
using System;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Collections.Generic;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class TurnosHistorialRepository : BaseRepository, ITurnosHistorialRepository
    {
        public TurnosHistorialRepository(IDbContext context) : base(context)
        {

        }
        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection.QueryBuilder(sql);

            return base.GetAllCustomQuery(builder);
        }

        private FormattableString BuildCustomQuery() =>
                $@"SELECT th.*,
		                    u.Nombre as Usuario,
		                    et.Descripcion as Estado
                    FROM TurnosHistorial th
                    INNER JOIN Usuarios u ON (u.IdUsuario = th.IdUsuario)
                    INNER JOIN EstadoTurno et ON (th.IdEstadoTurno = et.IdEstadoTurno) ";

        public DataTablesCustomQuery GetHistorial(int idTurno)
        {
            FormattableString sql = $@"SELECT th.*, 
		                                    et.descripcion as [Estado], 
		                                    et.clase as [EstadoClase],
		                                    u.nombre as [Usuario]
                                    FROM TurnosHistorial th
                                    INNER JOIN EstadoTurno et ON (et.idEstadoTurno = th.idEstadoTurno)
                                    INNER JOIN Usuarios u ON (u.idUsuario = th.idUsuario)
                                    WHERE th.idTurno = {idTurno} ";
            var builder = _context.Connection.QueryBuilder(sql);

            return base.GetAllCustomQuery(builder);
        }
    }
}
