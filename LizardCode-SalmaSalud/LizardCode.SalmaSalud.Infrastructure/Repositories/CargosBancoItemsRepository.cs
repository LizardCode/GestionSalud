using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class CargosBancoItemsRepository : ICargosBancoItemsRepository
    {
        private readonly IDbContext _context;

        public CargosBancoItemsRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdCargoBanco(int idCargoBanco, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM CargosBancoItems
                    WHERE IdCargoBanco = {idCargoBanco}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> DeleteByIdCargoBancoAndItem(int idCargoBanco, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM CargosBancoItems
                    WHERE IdCargoBanco = {idCargoBanco} AND Item = {item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<Domain.EntitiesCustom.CargoBancoItem>> GetAllByIdCargoBanco(int idCargoBanco, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    cbi.*,
                        cbi.Alicuota as IdAlicuota,
                        cc.CodigoCuenta Codigo,
                        cc.Descripcion Cuenta
                    FROM CargosBancoItems cbi
                    INNER JOIN CuentasContables cc ON cbi.IdCuentaContable = cc.IdCuentaContable
                    WHERE
                        cbi.IdCargoBanco = {idCargoBanco}");

            var results = await builder.QueryAsync<Domain.EntitiesCustom.CargoBancoItem>(transaction);

            return results.AsList();
        }

        public async Task<CargoBancoItem> GetByIdCargoBancoAndItem(int idCargoBanco, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM CargosBancoItems
                    WHERE
	                    IdCargoBanco = {idCargoBanco}
	                    AND Item = {item}"
                );

            return await builder.QuerySingleOrDefaultAsync<CargoBancoItem>(transaction);
        }

        public async Task<bool> Insert(CargoBancoItem entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO CargosBancoItems
                    (
                        IdCargoBanco,
                        Item,
                        IdCuentaContable,
                        Detalle,
                        Importe,
                        Alicuota,
                        Total
                    )
                    VALUES
                    (
                        {entity.IdCargoBanco},
                        {entity.Item},
                        {entity.IdCuentaContable},
                        {entity.Detalle},
                        {entity.Importe},
                        {entity.Alicuota},
                        {entity.Total}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> Update(CargoBancoItem entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE CargosBancoItems SET
	                    IdCuentaContable = {entity.IdCuentaContable},
	                    Detalle = {entity.Detalle},
                        Importe = {entity.Importe},
                        Alicuota = {entity.Alicuota},
	                    Total = {entity.Total}
                     WHERE
	                    IdCargoBanco = {entity.IdCargoBanco} AND Item = {entity.Item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}
