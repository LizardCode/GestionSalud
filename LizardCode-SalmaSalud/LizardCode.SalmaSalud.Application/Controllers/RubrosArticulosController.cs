using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.RubrosArticulos;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class RubrosArticulosController : BusinessController
    {
        private readonly IRubrosArticulosBusiness _rubrosArticulosBusiness;

        public RubrosArticulosController(IRubrosArticulosBusiness rubrosArticulosBusiness)
        {
            _rubrosArticulosBusiness = rubrosArticulosBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var model = new RubrosArticulosViewModel();

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _rubrosArticulosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(RubrosArticulosViewModel model)
        {
            await _rubrosArticulosBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var ceco = await _rubrosArticulosBusiness.Get(id);
            return Json(() => ceco);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(RubrosArticulosViewModel model)
        {
            await _rubrosArticulosBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _rubrosArticulosBusiness.Remove(id);
            return Json(() => true);
        }
    }
}
