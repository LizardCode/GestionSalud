using Dapper;
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
    public class OrdenesPagoPlanillaGastosRepository : BaseRepository, IOrdenesPagoPlanillaGastosRepository
    {
        public OrdenesPagoPlanillaGastosRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> DeleteByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM OrdenesPagoPlanillaGastos
                    WHERE 
                        IdOrdenPago = {idOrdenPago} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<OrdenPagoPlanillaGasto>> GetPlanillasGastosByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null)
        {
            var query = _context.Connection
               .QueryBuilder($@"
                    SELECT 
                            1 Seleccionar, 
                            pg.IdPlanillaGastos, 
                            pg.Fecha,
                            pg.Descripcion, 
                            pg.ImporteTotal Importe
                        FROM PlanillaGastos pg
                        INNER JOIN OrdenesPagoPlanillaGastos oppg ON pg.IdPlanillaGastos = oppg.IdPlanillaGastos
                ")
               .Where($"oppg.IdOrdenPago = {idOrdenPago}");

            return (await query.QueryAsync<OrdenPagoPlanillaGasto>()).AsList();
        }

        public async Task<List<OrdenPagoPlanillaGasto>> GetPlanillasImputar(string idModena, int idEmpresa, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT 0 Seleccionar, pg.IdPlanillaGastos, pg.Fecha, pg.Descripcion, pg.ImporteTotal Importe FROM planilla_gastos pg")
                .Where($"pg.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"pg.IdEstadoPlanilla = {(int)EstadoPlanillaGastos.Ingresada}")
                .Where($"pg.IdEmpresa = {idEmpresa}")
                .Where($"pg.Moneda = {idModena}");

            return (await query.QueryAsync<OrdenPagoPlanillaGasto>(transaction)).AsList();
        }
    }
}
