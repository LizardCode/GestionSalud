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
    public class PlantillasDetalleRepository : IPlantillasDetalleRepository
    {
        private readonly IDbContext _context;

        public PlantillasDetalleRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteByIdPlantilla(int idPlantilla, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM PlantillasDetalle
                    WHERE IdPlantilla = {idPlantilla}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<List<PlantillaDetalle>> GetByIdPlantilla(int idPlantilla, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
                        pd.IdPlantillaDetalle,
		                pd.IdPlantilla,
		                pd.IdPlantillaEtiqueta,
		                pd.Top,
		                pd.Left,
		                pd.Width,
		                pd.Height
                    FROM PlantillasDetalle pd
                    WHERE
	                    pd.IdPlantilla = {idPlantilla}"
                );

            var result = await builder.QueryAsync<PlantillaDetalle>(transaction);

            return result.AsList();
        }

        public async Task<PlantillaDetalle> GetByIdPlantillaAndEtiqueta(int idPlantilla, int idPlantillaEtiqueta, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM PlantillasDetalle
                    WHERE
	                    IdPlantilla = {idPlantilla}
	                    AND IdPlantillaEtiqueta = {idPlantillaEtiqueta}"
                );

            return await builder.QuerySingleOrDefaultAsync<PlantillaDetalle>(transaction);
        }

        public async Task<List<Domain.EntitiesCustom.PlantillaDetalle>> GetCustomByIdPlantilla(int idPlantilla)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT pd.*, pe.Ejemplo Text, pe.IdPlantillaEtiqueta Id FROM PlantillasDetalle pd
                        INNER JOIN Plantillas p ON pd.IdPlantilla = p.IdPlantilla
                        INNER JOIN PlantillasEtiquetas pe ON p.IdTipoPlantilla = pe.IdTipoPlantilla AND pd.IdPlantillaEtiqueta = pe.IdPlantillaEtiqueta
                    WHERE
	                    p.IdPlantilla = {idPlantilla}"
                );

            return (await builder.QueryAsync<Domain.EntitiesCustom.PlantillaDetalle>()).AsList();
        }

        public async Task<bool> Insert(PlantillaDetalle entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    INSERT INTO PlantillasDetalle
                    (
                        IdPlantilla,
                        IdPlantillaEtiqueta,
                        Top,
                        `Left`,
                        Width,
                        Height
                    )
                    VALUES
                    (
                        {entity.IdPlantilla},
                        {entity.IdPlantillaEtiqueta},
                        {entity.Top},
                        {entity.Left},
                        {entity.Width},
                        {entity.Height}
                    )");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> Update(PlantillaDetalle entity, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    UPDATE PlantillasDetalle SET
                        IdPlantilla = {entity.IdPlantilla},
                        IdPlantillaEtiqueta = {entity.IdPlantillaEtiqueta},
                        Top = {entity.Top},
                        `Left` = {entity.Left},
                        Width = {entity.Width},
                        Height = {entity.Height}
                     WHERE
	                    IdPlantillaDetalle = {entity.IdPlantillaDetalle}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

    }
    
}
