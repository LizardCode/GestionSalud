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
    public class CodigosRetencionGananciasItemsRepository : ICodigosRetencionGananciasItemsRepository
    {
        private readonly IDbContext _context;

        public CodigosRetencionGananciasItemsRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdCodigoRetencion(int idCodigoRetencion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM CodigosRetencionGananciasItems
                    WHERE IdCodigoRetencion = {idCodigoRetencion}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> DeleteByIdCodigoRetencionAndItem(int idCodigoRetencion, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM CodigosRetencionGananciasItems
                    WHERE IdCodigoRetencion = {idCodigoRetencion} AND Item = {item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<IList<CodigosRetencionGananciasItems>> GetAllByIdCodigoRetencion(int idCodigoRetencion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    *
                    FROM CodigosRetencionGananciasItems 
                    WHERE
                        IdCodigoRetencion = {idCodigoRetencion}");

            var results = await builder.QueryAsync<CodigosRetencionGananciasItems>(transaction);

            return results.AsList();
        }

        public async Task<CodigosRetencionGananciasItems> GetByIdCodigoRetencionAndItem(int idCodigoRetencion, int item, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM CodigosRetencionGananciasItems
                    WHERE
	                    IdCodigoRetencion = {idCodigoRetencion}
	                    AND Item = {item}"
                );

            return await builder.QuerySingleOrDefaultAsync<CodigosRetencionGananciasItems>(transaction);
        }

        public async Task<CodigosRetencionGananciasItems> GetByImporteDesdeHasta(int idCodigoRetencion, double baseRetencionGanancias, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT crgi.* FROM CodigosRetencionGananciasItems crgi
                    WHERE
                        crgi.IdCodigoRetencion = {idCodigoRetencion} AND
                        {baseRetencionGanancias} BETWEEN crgi.ImporteDesde AND crgi.ImporteHasta"
                );

            return await builder.QuerySingleOrDefaultAsync<CodigosRetencionGananciasItems>(transaction);
        }

        public async Task<bool> Insert(CodigosRetencionGananciasItems entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO CodigosRetencionGananciasItems
                    (
                        IdCodigoRetencion,
                        Item,
                        ImporteDesde,
                        ImporteHasta,
                        ImporteRetencion,
                        MasPorcentaje,
                        SobreExcedente
                    )
                    VALUES
                    (
                        {entity.IdCodigoRetencion},
                        {entity.Item},
                        {entity.ImporteDesde},
                        {entity.ImporteHasta},
                        {entity.ImporteRetencion},
                        {entity.MasPorcentaje},
                        {entity.SobreExcedente}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> Update(CodigosRetencionGananciasItems entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE CodigosRetencionGananciasItems SET
	                    ImporteDesde = {entity.ImporteDesde},
	                    ImporteHasta = {entity.ImporteHasta},
                        ImporteRetencion = {entity.ImporteRetencion},
	                    MasPorcentaje = {entity.MasPorcentaje},
                        SobreExcedente = {entity.SobreExcedente}
                     WHERE
	                    IdCodigoRetencion = {entity.IdCodigoRetencion} AND Item = {entity.Item}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}
