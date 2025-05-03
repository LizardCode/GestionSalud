using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class OrdenesPagoDetalleRepository : BaseRepository, IOrdenesPagoDetalleRepository
    {
        public OrdenesPagoDetalleRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> DeleteByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM OrdenesPagoDetalle
                    WHERE 
                        IdOrdenPago = {idOrdenPago} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<Custom.OrdenPagoDetalle>> GetAllByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"opd.*")
                .Select($"ch.NroCheque NumeroCheque")
                .Select($"ch.Banco BancoCheque")
                .Select($"ch.FechaEmision")
                .Select($"ch.FechaVto")
                .Select($"tr.Fecha FechaTransferencia")
                .Select($"tr.NroTransferencia NumeroTransferencia")
                .Select($"tr.IdBanco IdBancoTranferencia")
                .Select($"tr.BancoOrigen")
                .From($"OrdenesPagoDetalle opd")
                .From($"LEFT JOIN cheques ch ON opd.IdCheque = ch.IdCheque")
                .From($"LEFT JOIN transferencias tr ON opd.IdTransferencia = tr.IdTransferencia")
                .Where($"opd.IdOrdenPago = {idOrdenPago}");

            var detalles = (await query.QueryAsync<Custom.OrdenPagoDetalle>()).AsList();

            return detalles;
        }

    }
}
