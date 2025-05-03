using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class RecibosAnticiposRepository : BaseRepository, IRecibosAnticiposRepository
    {
        public RecibosAnticiposRepository(IDbContext context) : base(context)
        {

        }

        public async Task<ReciboAnticipo> GetByIdRecibo(int idRecibo, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM RecibosAnticipos
                    WHERE
	                    IdRecibo = {idRecibo}"
                );

            return await builder.QuerySingleOrDefaultAsync<ReciboAnticipo>(transaction);
        }

        public async Task<ReciboAnticipo> GetByIdAnticipo(int idAnticipo, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM RecibosAnticipos
                    WHERE
	                    IdAnticipo = {idAnticipo}"
                );

            return await builder.QuerySingleOrDefaultAsync<ReciboAnticipo>(transaction);
        }

        public async Task<bool> DeleteByIdRecibo(int idRecibo, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM RecibosAnticipos
                    WHERE 
                        IdRecibo = {idRecibo} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<Domain.EntitiesCustom.ReciboAnticipo>> GetAnticiposImputar(int idCliente, string idMoneda, int idEmpresa, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT 0 Seleccionar, r.IdRecibo, r.Descripcion, ROUND(r.Importe - ISNULL(ABS(ra.Importe), 0),2) Saldo, '0' Importe
                        FROM Recibos r
                        LEFT JOIN (
                            SELECT IdAnticipo, SUM(a.Importe) AS Importe FROM RecibosAnticipos a
                                INNER JOIN recibos r ON r.IdRecibo = a.IdAnticipo
                                WHERE
                                    r.IdCliente = {idCliente}
                                GROUP BY a.IdAnticipo
                        ) ra ON r.IdRecibo = ra.IdAnticipo
                ")
                .Where($"r.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"r.IdCliente = {idCliente}")
                .Where($"r.IdTipoRecibo = {(int)TipoRecibo.Anticipo}")
                .Where($"r.Moneda = {idMoneda}")
                .Where($"ROUND(r.Importe - ISNULL(ABS(ra.Importe), 0), 2) > 0")
                .Where($"r.IdEmpresa = {idEmpresa}");

            return (await query.QueryAsync<Domain.EntitiesCustom.ReciboAnticipo>()).AsList();
        }

        public async Task<IList<Domain.EntitiesCustom.ReciboAnticipo>> GetByIdRecibo(int idRecibo, int idCliente)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT 1 Seleccionar, r.IdRecibo, r.Descripcion, ROUND(r.Importe - ISNULL(ABS(ra.Importe), 0),2) Saldo, ran.Importe
                        FROM Recibos r
                        INNER JOIN RecibosAnticipos ran ON r.IdRecibo = ran.IdAnticipo
                        LEFT JOIN (
                            SELECT IdAnticipo, SUM(a.Importe) AS Importe FROM RecibosAnticipos a
                                INNER JOIN recibos r ON r.IdRecibo = a.IdAnticipo
                                WHERE
                                    r.IdCliente = {idCliente} AND
                                    a.IdRecibo <> {idRecibo}
                                GROUP BY a.IdAnticipo
                        ) ra ON r.IdRecibo = ra.IdAnticipo
                ")
                .Where($"r.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}")
                .Where($"r.IdCliente = {idCliente}")
                .Where($"r.IdTipoRecibo = {(int)TipoRecibo.Anticipo}")
                .Where($"ran.IdRecibo = {idRecibo}")
                .Where($"ROUND(r.Importe - ISNULL(ABS(ra.Importe), 0), 2) > 0");

            return (await query.QueryAsync<Domain.EntitiesCustom.ReciboAnticipo>()).AsList();
        }
    }
}
