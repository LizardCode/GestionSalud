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
    public class CargosBancoItemsComprobantesCompraRepository : ICargosBancoItemsComprobantesCompraRepository
    {
        private readonly IDbContext _context;

        public CargosBancoItemsComprobantesCompraRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdCargoBancoItem(int idCargoBanco, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM CargosBancoItemsComprobantesCompras
                    WHERE 
                        IdCargoBanco = {idCargoBanco} AND
                        Item = {item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> DeleteByIdCargoBanco(int idCargoBanco, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM CargosBancoItemsComprobantesCompras
                    WHERE 
                        IdCargoBanco = {idCargoBanco} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> DeleteByIdComprobanteCompraItem(int idComprobanteCompra, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM CargosBancoItemsComprobantesCompras
                    WHERE 
                        IdComprobanteCompra = {idComprobanteCompra} AND
                        Item = {item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<List<CargoBancoItemComprobanteCompra>> GetByIdCargoBanco(int idCargoBanco, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM CargosBancoItemsComprobantesCompras
                    WHERE
	                    IdCargoBanco = {idCargoBanco}"
                );

            return (await builder.QueryAsync<CargoBancoItemComprobanteCompra>(transaction)).AsList();
        }

        public async Task<CargoBancoItemComprobanteCompra> GetByIdCargoBancoItem(int idCargoBanco, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM CargosBancoItemsComprobantesCompras
                    WHERE
	                    IdCargoBanco = {idCargoBanco} AND
                        Item = {item}"
                );

            return await builder.QuerySingleOrDefaultAsync<CargoBancoItemComprobanteCompra>(transaction);
        }

        public async Task<bool> Insert(CargoBancoItemComprobanteCompra entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO CargosBancoItemsComprobantesCompras
                    (
                        IdCargoBanco,
                        Item,
                        IdComprobanteCompra
                    )
                    VALUES
                    (
                        {entity.IdCargoBanco},
                        {entity.Item},
                        {entity.IdComprobanteCompra}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
}
