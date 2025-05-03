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
    public class SdoCtaCteCliRepository : BaseRepository, ISdoCtaCteCliRepository
    {
        public SdoCtaCteCliRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT sc.IdSaldoCtaCteCli, sc.IdUsuario, sc.Fecha, sc.Descripcion, sc.IdEstadoRegistro, sc.IdEmpresa,
		                                sc.FechaDesde, sc.FechaHasta, sc.Total,
		                                COUNT(*) as Cantidad 
                                FROM SaldoCtaCteCli sc
                                INNER JOIN SaldoCtaCteCliComprobantesVentas scv ON (scv.IdSaldoCtaCteCli = sc.IdSaldoCtaCteCli) 
                                GROUP BY sc.IdSaldoCtaCteCli, sc.IdUsuario, sc.Fecha, sc.Descripcion, sc.IdEstadoRegistro, sc.IdEmpresa,
		                                    sc.FechaDesde, sc.FechaHasta, sc.Total ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<List<Domain.EntitiesCustom.SdoCtaCteCliItem>> GetItemsByIdSaldoCtaCteCli(int idSaldoCtaCteCli, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    scv.Item,
	                    cv.IdCliente,
	                    cv.Fecha,
	                    cv.FechaVto Vencimiento,
	                    cv.IdComprobante,
	                    cv.Sucursal,
	                    cv.Numero,
	                    cvt.Neto NetoGravado,
	                    cvt.Alicuota IdAlicuota,
	                    cv.Total
                    FROM SaldoCtaCteCli sc
	                    INNER JOIN SaldoCtaCteCliComprobantesVentas scv ON sc.IdSaldoCtaCteCli = scv.IdSaldoCtaCteCli 
                        INNER JOIN ComprobantesVentas cv ON cv.IdComprobanteVenta = scv.IdComprobanteVenta
                        INNER JOIN ComprobantesVentasTotales cvt ON cv.IdComprobanteVenta = cvt.IdComprobanteVenta
                    WHERE
	                    sc.IdSaldoCtaCteCli = {idSaldoCtaCteCli} "
                );

            var result = await builder.QueryAsync<Domain.EntitiesCustom.SdoCtaCteCliItem>(transaction);

            return result.ToList();
        }
    }
}
