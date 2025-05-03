using Dapper;
using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
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
    public class PlantillasRepository : BaseRepository, IPlantillasRepository, IDataTablesCustomQuery
    {
        public PlantillasRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        p.IdPlantilla, 
                        p.IdEmpresa, 
                        p.Descripcion,
                        p.IdTipoPlantilla,
                        p.IdEstadoRegistro,
                        tp.Descripcion TipoPlantilla 
                    FROM Plantillas p
                        INNER JOIN TipoPlantilla tp ON tp.IdTipoPlantilla = p.IdTipoPlantilla ");

            return base.GetAllCustomQuery(builder);
        }

        public async Task<List<PlantillaEtiqueta>> GetPlantillaEtiquetasByTipo(int idTipoPlantilla, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM PlantillasEtiquetas 
                    WHERE
	                    IdTipoPlantilla = {idTipoPlantilla}"
                );

            var result = await builder.QueryAsync<PlantillaEtiqueta>(transaction);

            return result.AsList();
        }

        public async Task<List<Plantilla>> GetPlantillasByTipo(int idEmpresa, int idTipoPlantilla, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT * FROM Plantillas
                    WHERE IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} 
	                    AND IdEmpresa = {idEmpresa} 
	                    AND IdTipoPlantilla = {idTipoPlantilla} "
                );

            var result = await builder.QueryAsync<Plantilla>(transaction);

            return result.AsList();
        }
    }
}
