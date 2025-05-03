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
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    internal class GuardiasRepository : BaseRepository, IGuardiasRepository, IDataTablesCustomQuery
    {
        public GuardiasRepository(IDbContext context) : base(context)
        {
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"                                
                                SELECT G.*,
                                        P.nombre as Profesional,
                                        EG.descripcion as Estado,
                                        EG.clase as EstadoClase
                                FROM Guardias G
                                INNER JOIN Profesionales         P ON (P.IdProfesional = G.idProfesional)
                                INNER JOIN EstadoGuardia EG ON (EG.idEstadoGuardia = G.idEstadoGuardia)
                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<Custom.Guardia> GetCustomById(int idGuardia, IDbTransaction transaction = null)
        {
            var sql = BuildCustomQuery();
            var builder = _context.Connection
            .QueryBuilder(sql)
                .Where($@"G.IdGuardia = {idGuardia} "
                );

            var result = await builder.QueryFirstOrDefaultAsync<Custom.Guardia>(transaction);

            return result;
        }

        private FormattableString BuildCustomQuery() =>
            $@"
                SELECT G.*,
                    P.nombre as Profesional,
                    EG.descripcion as Estado,
                    EG.clase as EstadoClase
                FROM Guardias G
                INNER JOIN Profesionales         P ON (P.IdProfesional = G.idProfesional)
                INNER JOIN EstadoGuardia EG ON (EG.idEstadoGuardia = G.idEstadoGuardia)
            ";

        public async Task<List<Custom.Guardia>> GetGuardiasALiquidar(DateTime desde, DateTime hasta, int idProfesional, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                        .CommandBuilder($@"
                                        SELECT g.*,
                                                '' as Financiador,
                                                CONCAT('GUARDIA - ', substring(CONVERT(varchar(16), g.desde, 120), 6, 11), ' - ', 
                                                                    substring(CONVERT(varchar(16), g.hasta, 120), 6, 11)) as DescripcionLiquidacion
                                        FROM Guardias g

                                        WHERE g.fecha BETWEEN {desde} AND {hasta}
                                            AND g.idProfesional = {idProfesional}
                                            AND g.idEstadoRegistro != {(int)EstadoRegistro.Eliminado}
                                            AND g.idGuardia NOT IN (
                                                SELECT ISNULL(lpp.idGuardia, 0)
                                                FROM LiquidacionesProfesionalesPrestaciones lpp
                                                INNER JOIN LiquidacionesProfesionales        lp ON (lpp.idLiquidacion = lp.idLiquidacionProfesional)
                                                WHERE lp.idEstadoRegistro != {(int)EstadoRegistro.Eliminado} )");


            var results = await builder.QueryAsync<Custom.Guardia>(transaction);

            return results.AsList();
        }

        public async Task<bool> ValidarGuardiaLiquidada(int idGuardia, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT LP.*
                    FROM ComprobantesComprasItems CCI
                    INNER JOIN ComprobantesCompras CC ON (CC.IdComprobanteCompra = CCI.IdComprobanteCompra)
                    INNER JOIN LiquidacionesProfesionales LP ON (CCI.IdLiquidacionProfesional = LP.IdLiquidacionProfesional)
                ")
                .Where($"CC.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"CCI.IdGuardia = {idGuardia}");

            var result = await query.QueryAsync<LiquidacionProfesional>(transaction);

            return result.AsList().Count > 0;
        }
    }
}