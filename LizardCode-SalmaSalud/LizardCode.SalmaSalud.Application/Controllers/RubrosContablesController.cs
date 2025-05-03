using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.RubrosContables;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class RubrosContablesController : BusinessController
    {
        private readonly IRubrosContablesBusiness _rubrosContablesBusiness;

        public RubrosContablesController(IRubrosContablesBusiness rubrosContablesBusiness)
        {
            _rubrosContablesBusiness = rubrosContablesBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var model = new RubrosContablesViewModel();
            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _rubrosContablesBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(RubrosContablesViewModel model)
        {
            await _rubrosContablesBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var rubro = await _rubrosContablesBusiness.Get(id);
            return Json(() => rubro);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(RubrosContablesViewModel model)
        {
            await _rubrosContablesBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _rubrosContablesBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetRubrosContables(string q)
        {
            var rubros = await _rubrosContablesBusiness.GetRubrosContables(q);
            return Json(() => rubros);
        }

    }
}
