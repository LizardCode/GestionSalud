using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using System;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    internal class LiquidacionesProfesionalesRepository : BaseRepository, ILiquidacionesProfesionalesRepository, IDataTablesCustomQuery
    {
        public LiquidacionesProfesionalesRepository(IDbContext context) : base(context)
        {
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                                SELECT LP.*,
		                                P.nombre as Profesional,
                                        ELP.descripcion as Estado,
                                        ELP.clase as EstadoClase
                                FROM LiquidacionesProfesionales LP
                                INNER JOIN Profesionales         P ON (P.IdProfesional = LP.idProfesional)
                                INNER JOIN EstadoLiquidacionProfesional ELP ON (ELP.IdEstadoLiquidacionProfesional = LP.IdEstadoLiquidacionProfesional)
                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<Custom.LiquidacionProfesional> GetCustomById(int idLiquidacionProfesional, IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
            .QueryBuilder(sql)
                .Where($@"LP.idLiquidacionProfesional = {idLiquidacionProfesional} "
                );

            var result = await builder.QueryFirstOrDefaultAsync<Custom.LiquidacionProfesional>(transaction);

            return result;
        }

        private FormattableString BuildCustomQuery() =>
            $@"
				SELECT LP.*,
		                P.nombre as Profesional,
                        ELP.descripcion as Estado,
                        ELP.clase as EstadoClase
                FROM LiquidacionesProfesionales LP
                INNER JOIN Profesionales         P ON (P.IdProfesional = LP.idProfesional)
                INNER JOIN EstadoLiquidacionProfesional ELP ON (ELP.IdEstadoLiquidacionProfesional = LP.IdEstadoLiquidacionProfesional)
            ";
    }
}
 