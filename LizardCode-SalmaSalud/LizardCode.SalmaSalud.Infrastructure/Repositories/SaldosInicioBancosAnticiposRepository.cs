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
    public class SaldosInicioBancosAnticiposRepository : BaseRepository, ISaldosInicioBancosAnticiposRepository
    {
        public SaldosInicioBancosAnticiposRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> DeleteByIdSaldoInicioBanco(int idSaldoInicioBanco, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM SaldoInicioBancosAnticipos
                    WHERE 
                        IdSaldoInicioBanco = {idSaldoInicioBanco} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<Custom.SaldoInicioBancoAnticipos>> GetAllByIdSaldoInicioBanco(int idSaldoInicioBanco, int idTipoSaldo, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"siba.*")
                .Select($"tsi.Descripcion TipoSaldoInicio")
                .Select($"c.RazonSocial Cliente")
                .Select($"p.RazonSocial Proveedor")
                .From($"SaldoInicioBancosAnticipos siba")
                .From($"LEFT JOIN TipoSaldoInicioBancos tsi ON tsi.IdTipoSdoInicio = siba.IdTipoSdoInicio")
                .From($"LEFT JOIN Clientes c ON c.IdCliente = siba.IdCliente")
                .From($"LEFT JOIN Proveedores p ON p.IdProveedor = siba.IdProveedor")
                .Where($"siba.IdTipoSdoInicio = {idTipoSaldo}")
                .Where($"siba.IdSaldoInicioBanco = {idSaldoInicioBanco}");

            var detalles = (await query.QueryAsync<Custom.SaldoInicioBancoAnticipos>()).AsList();

            return detalles;
        }

    }
}
