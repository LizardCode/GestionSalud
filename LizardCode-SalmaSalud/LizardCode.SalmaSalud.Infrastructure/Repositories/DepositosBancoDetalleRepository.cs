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
    public class DepositosBancoDetalleRepository : BaseRepository, IDepositosBancoDetalleRepository
    {
        public DepositosBancoDetalleRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> DeleteByIdDepositoBanco(int idDepositoBanco, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM DepositosBancoDetalle
                    WHERE 
                        IdDepositoBanco = {idDepositoBanco} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<List<Custom.DepositoBancoDetalle>> GetAllByIdDepositoBanco(int idDepositoBanco, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"dbd.*")
                .Select($"ch.IdBanco")
                .Select($"ch.NroCheque")
                .Select($"ch.Banco BancoCheque")
                .Select($"ch.FechaEmision")
                .Select($"ch.FechaVto")
                .Select($"tr.Fecha FechaTransferencia")
                .Select($"tr.NroTransferencia")
                .Select($"tr.IdBanco IdBancoTranferencia")
                .From($"DepositosBancoDetalle dbd")
                .From($"LEFT JOIN Cheques ch ON dbd.IdCheque = ch.IdCheque")
                .From($"LEFT JOIN Transferencias tr ON dbd.IdTransferencia = tr.IdTransferencia")
                .Where($"dbd.IdDepositoBanco = {idDepositoBanco}");

            var detalles = (await query.QueryAsync<Custom.DepositoBancoDetalle>()).AsList();

            return detalles;
        }

    }
}
