using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPlantillasDetalleRepository
    {
        Task<bool> DeleteByIdPlantilla(int idPlantilla, IDbTransaction transaction = null);

        Task<List<PlantillaDetalle>> GetByIdPlantilla(int idPlantilla, IDbTransaction transaction = null);

        Task<PlantillaDetalle> GetByIdPlantillaAndEtiqueta(int idPlantilla, int idPlantillaEtiqueta, IDbTransaction transaction = null);

        Task<bool> Insert(PlantillaDetalle entity, IDbTransaction transaction = null);

        Task<bool> Update(PlantillaDetalle entity, IDbTransaction transaction = null);

        Task<List<Domain.EntitiesCustom.PlantillaDetalle>> GetCustomByIdPlantilla(int idPlantilla);
    }
}