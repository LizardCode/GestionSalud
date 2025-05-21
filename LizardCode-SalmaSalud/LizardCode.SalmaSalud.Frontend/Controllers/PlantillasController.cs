using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Business;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Models.Plantillas;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class PlantillasController : BusinessController
    {
        private readonly IPlantillasBusiness _plantillasBusiness;

        public PlantillasController(IPlantillasBusiness plantillasBusiness)
        {
            _plantillasBusiness = plantillasBusiness;
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Index()
        {
            var model = new PlantillaViewModel()
            {
                MaestroTiposPlantilla = Utilities.EnumToDictionary<TiposPlantilla>()
                    .ToDropDownList(descriptionIncludesKey: false)
                //EtiquetasPlanilla = new Dictionary<string, string>()
            };

            return ActivateMenuItem(model: model);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _plantillasBusiness.GetAllDT(request);
            return Json(results);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<JsonResult> Nuevo(PlantillaViewModel model)
        {
            await _plantillasBusiness.New(model);
            return Json(() => true);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<JsonResult> Obtener(int id)
        {
            var plantilla = await _plantillasBusiness.Get(id);
            return Json(() => plantilla);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<JsonResult> Modificar(PlantillaViewModel model)
        {
            await _plantillasBusiness.Update(model);
            return Json(() => true);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _plantillasBusiness.Remove(id);
            return Json(() => true);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<JsonResult> GetPlantillaEtiquetas(int idTipoPlantilla)
        {
            var etiquetas = await _plantillasBusiness.GetPlantillaEtiquetas(idTipoPlantilla);
            return Json(() => etiquetas);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> ValidarTipoPlantilla(int idTipoPlantilla, int? idPlantilla)
        {
            var results = await _plantillasBusiness.ValidarTipoPlantilla(idTipoPlantilla, idPlantilla);
            return Json(results.IsNull() ? true : results);
        }
    }
}
