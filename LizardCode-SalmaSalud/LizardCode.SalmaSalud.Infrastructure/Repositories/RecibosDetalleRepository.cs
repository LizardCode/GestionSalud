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
    public class RecibosDetalleRepository : BaseRepository, IRecibosDetalleRepository
    {
        public RecibosDetalleRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> DeleteByIdRecibo(int idRecibo, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM RecibosDetalle
                    WHERE 
                        IdRecibo = {idRecibo} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<List<Custom.ReciboDetalle>> GetAllByIdRecibo(int idRecibo, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"rd.*")
                .Select($"ch.NroCheque")
                .Select($"ch.Banco BancoCheque")
                .Select($"ch.FechaEmision")
                .Select($"ch.FechaVto")
                .Select($"ch.Firmante FirmanteCheque")
                .Select($"ch.CUITFirmante")
                .Select($"ch.Endosante1 Endosante1Cheque")
                .Select($"ch.CUITEndosante1")
                .Select($"ch.Endosante2 Endosante2Cheque")
                .Select($"ch.CUITEndosante2")
                .Select($"ch.Endosante3 Endosante3Cheque")
                .Select($"ch.CUITEndosante3")
                .Select($"tr.Fecha FechaTransferencia")
                .Select($"tr.NroTransferencia")
                .Select($"tr.IdBanco IdBancoTranferencia")
                .Select($"tr.BancoOrigen")
                .Select($"do.FechaEmision FechaDocumento")
                .Select($"do.FechaVto FechaVtoDocumento")
                .Select($"do.NroDocumento")
                .Select($"do.Firmante FirmanteDocumento")
                .Select($"do.CUITFirmante CUITFirmanteDocumento")
                .From($"RecibosDetalle rd")
                .From($"LEFT JOIN Cheques ch ON rd.IdCheque = ch.IdCheque")
                .From($"LEFT JOIN Transferencias tr ON rd.IdTransferencia = tr.IdTransferencia")
                .From($"LEFT JOIN Documentos do ON rd.IdDocumento = do.IdDocumento")
                .Where($"rd.IdRecibo = {idRecibo}");

            var detalles = (await query.QueryAsync<Custom.ReciboDetalle>()).AsList();

            return detalles;
        }

    }
}
