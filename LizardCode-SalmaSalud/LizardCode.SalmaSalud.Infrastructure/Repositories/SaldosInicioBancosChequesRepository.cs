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
    public class SaldosInicioBancosChequesRepository : BaseRepository, ISaldosInicioBancosChequesRepository
    {
        public SaldosInicioBancosChequesRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> DeleteByIdSaldoInicioBanco(int idSaldoInicioBanco, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM saldo_inicio_bancos_cheques
                    WHERE 
                        idSaldoInicioBanco = {idSaldoInicioBanco} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<List<Custom.SaldoInicioBancoCheques>> GetAllByIdSaldoInicioBanco(int idSaldoInicioBanco, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"sibc.*")
                .Select($"tsi.Descripcion TipoSaldoInicio")
                .Select($"tch.Descripcion TipoCheque")
                .From($"SaldoInicioBancosCheques sibc")
                .From($"LEFT JOIN TipoSaldoInicioBancos tsi ON tsi.IdTipoSdoInicio = sibc.IdTipoSdoInicio")
                .From($"LEFT JOIN Cheques ch ON ch.IdCheque = sibc.IdCheque")
                .From($"LEFT JOIN TipoCheques tch ON tch.IdTipoCheque = ch.IdTipoCheque")
                .Where($"sibc.IdSaldoInicioBanco = {idSaldoInicioBanco}");

            var detalles = (await query.QueryAsync<Custom.SaldoInicioBancoCheques>()).AsList();

            return detalles;
        }

    }
}
