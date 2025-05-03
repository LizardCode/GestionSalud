using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class PlanillaGastosRepository : BaseRepository, IPlanillaGastosRepository
    {
        public PlanillaGastosRepository(IDbContext context) : base(context)
        {

        }

        public async Task<PlanillaGastos> GetCustomByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
                        pg.*
                    FROM PlanillaGastos pg
                    WHERE
                        pg.IdPlanillaGastos = {idPlanillaGastos} ");

            return await builder.QueryFirstOrDefaultAsync<Domain.EntitiesCustom.PlanillaGastos>(transaction);
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var builder = _context.Connection.QueryBuilder($@"SELECT
                    pg.*
                FROM PlanillaGastos pg");

            return base.GetAllCustomQuery(builder);
        }

        public async Task<List<Select2Custom>> GetItemsGastos(int anno, int mes, int numero, string moneda, int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
                        epic.item Id,
                        (epic.item + ' - ' + epic.Descripcion) Text
                    FROM EstimadosProduccionItemsCostos epic
                        INNER JOIN EstimadosProduccion ep ON epic.IdEstimadoProduccion = ep.IdEstimadoProduccion
                    WHERE
                        ep.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                        ep.Anno = {anno} AND
                        ep.Mes = {mes} AND
                        ep.Numero = {numero} AND
                        ep.IdEmpresa = {idEmpresa} AND
                        epic.Moneda = {moneda} AND
                        epic.IdTipoItem = {(int)TipoItemEstimado.Gastos}
                    ");

            var results = await builder.QueryAsync<Select2Custom>();

            return results.AsList();
        }
    }
}
