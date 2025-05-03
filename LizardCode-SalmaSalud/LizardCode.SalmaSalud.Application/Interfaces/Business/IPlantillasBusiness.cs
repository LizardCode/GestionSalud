using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Plantillas;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public interface IPlantillasBusiness
    {
        Task<PlantillaViewModel> Get(int idPlantilla);
        Task<IList<Plantilla>> GetAll();
        Task<DataTablesResponse<Domain.EntitiesCustom.Plantilla>> GetAllDT(DataTablesRequest request);
        Task<Domain.EntitiesCustom.Plantilla> GetCustom(int idPlantilla);
        Task New(PlantillaViewModel model);
        Task Remove(int idPlantilla);
        Task Update(PlantillaViewModel model);
        Task<List<PlantillaEtiqueta>> GetPlantillaEtiquetas(int idTipoPlantilla);
        Task<string> ValidarTipoPlantilla(int idTipoPlantilla, int? idPlantilla);
    }
}