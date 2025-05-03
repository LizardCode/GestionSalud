using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class SdoCtaCtePrvRepository : BaseRepository, ISdoCtaCtePrvRepository
    {
        public SdoCtaCtePrvRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT sp.IdSaldoCtaCtePrv, sp.IdUsuario, sp.Fecha, sp.Descripcion, 
		                                    sp.IdEstadoRegistro, sp.IdEmpresa, sp.FechaDesde, sp.FechaHasta, sp.Total,
		                                    COUNT(*) as Cantidad 
                                    FROM SaldoCtaCtePrv sp
                                    INNER JOIN SaldoCtaCtePrvComprobantesCompras spc ON (spc.IdSaldoCtaCtePrv = sp.IdSaldoCtaCtePrv) 
                                    GROUP BY sp.IdSaldoCtaCtePrv, sp.IdUsuario, sp.Fecha, sp.Descripcion, 
			                                    sp.IdEstadoRegistro, sp.IdEmpresa, sp.FechaDesde, sp.FechaHasta, sp.Total");

            return base.GetAllCustomQuery(query);
        }

        public async Task<List<Domain.EntitiesCustom.SdoCtaCtePrvItem>> GetItemsByIdSaldoCtaCtePrv(int idSaldoCtaCtePrv, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    spc.Item,
	                    cc.IdProveedor,
	                    cc.Fecha,
	                    cc.FechaVto Vencimiento,
	                    cc.IdComprobante,
	                    cc.Sucursal,
	                    cc.Numero,
	                    cct.Neto NetoGravado,
	                    cct.Alicuota IdAlicuota,
                        cc.Percepciones Percepciones,
	                    cc.Total
                    FROM SaldoCtaCtePrv sp
	                    INNER JOIN SaldoCtaCtePrvComprobantesCompras spc ON sp.IdSaldoCtaCtePrv = spc.IdSaldoCtaCtePrv 
                        INNER JOIN ComprobantesCompras cc ON cc.IdComprobanteCompra = spc.IdComprobanteCompra
                        INNER JOIN ComprobantesComprasTotales cct ON cc.IdComprobanteCompra = cct.IdComprobanteCompra
                        /*LEFT  JOIN ComprobantesComprasPercepciones ccp ON cc.IdComprobanteCompra = ccp.IdComprobanteCompra*/
                    WHERE
	                    sp.IdSaldoCtaCtePrv = {idSaldoCtaCtePrv} "
                );

            var result = await builder.QueryAsync<Domain.EntitiesCustom.SdoCtaCtePrvItem>(transaction);

            return result.ToList();
        }
    }
}
