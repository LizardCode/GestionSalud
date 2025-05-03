using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class OrdenesPagoAnticiposRepository : BaseRepository, IOrdenesPagoAnticiposRepository
    {
        public OrdenesPagoAnticiposRepository(IDbContext context) : base(context)
        {

        }

        public async Task<IList<OrdenPagoAnticipo>> GetAllByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM OrdenesPagoAnticipos
                    WHERE
	                    IdOrdenPago = {idOrdenPago}"
                );

            return (await builder.QueryAsync<OrdenPagoAnticipo>(transaction)).AsList();
        }

        public async Task<OrdenPagoAnticipo> GetByIdAnticipo(int idAnticipo, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM OrdenesPagoAnticipos
                    WHERE
	                    IdAnticipo = {idAnticipo}"
                );

            return await builder.QuerySingleOrDefaultAsync<OrdenPagoAnticipo>(transaction);
        }

        public async Task<bool> DeleteByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM OrdenesPagoAnticipos
                    WHERE 
                        IdOrdenPago = {idOrdenPago} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<Custom.OrdenPagoAnticipo>> GetAnticiposImputar(int idProveedor, string idMoneda, int idEmpresa, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT 0 Seleccionar, op.IdOrdenPago, op.Descripcion, ROUND(op.Importe - ISNULL(ABS(opa.Importe), 0),2) Saldo, '0' Importe
                        FROM OrdenesPago op
                        LEFT JOIN (
                            SELECT IdAnticipo, SUM(a.Importe) AS Importe FROM OrdenesPagoAnticipos a
                                INNER JOIN OrdenesPago op ON op.IdOrdenPago = a.IdAnticipo
                                WHERE
                                    op.IdProveedor = {idProveedor}
                                GROUP BY a.IdAnticipo
                        ) opa ON op.IdOrdenPago = opa.IdAnticipo
                ")
                .Where($"op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"op.IdProveedor = {idProveedor}")
                .Where($"op.IdTipoOrdenPago = {(int)TipoOrdenPago.Anticipo}")
                .Where($"op.Moneda = {idMoneda}")
                .Where($"ROUND(op.Importe - ISNULL(ABS(opa.Importe), 0), 2) > 0")
                .Where($"op.IdEmpresa = {idEmpresa}");

            return (await query.QueryAsync<Custom.OrdenPagoAnticipo>(transaction)).AsList();
        }
        public async Task<IList<Custom.OrdenPagoAnticipo>> GetByIdOrdenPago(int idOrdenPago, int idProveedor, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT 1 Seleccionar, op.IdOrdenPago, op.Descripcion, ROUND(op.Importe - ISNULL(ABS(opa.Importe), 0),2) Saldo, opan.Importe
                        FROM OrdenesPago op
                        INNER JOIN OrdenesPagoAnticipos opan ON op.IdOrdenPago = opan.IdAnticipo
                        LEFT JOIN (
                            SELECT IdAnticipo, SUM(a.Importe) AS Importe FROM OrdenesPagoAnticipos a
                                INNER JOIN OrdenesPago op ON op.IdOrdenPago = a.IdAnticipo
                                WHERE
                                    op.IdProveedor = {idProveedor} AND
                                    a.IdOrdenPago <> {idOrdenPago}
                                GROUP BY a.IdAnticipo
                        ) opa ON op.IdOrdenPago = opa.IdAnticipo
                ")
                .Where($"op.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"op.IdProveedor = {idProveedor}")
                .Where($"op.IdTipoOrdenPago = {(int)TipoOrdenPago.Anticipo}")
                .Where($"opan.IdOrdenPago = {idOrdenPago}")
                .Where($"ROUND(op.Importe - ISNULL(ABS(opa.Importe), 0), 2) > 0");

            return (await query.QueryAsync<Custom.OrdenPagoAnticipo>(transaction)).AsList();
        }
    }
}
