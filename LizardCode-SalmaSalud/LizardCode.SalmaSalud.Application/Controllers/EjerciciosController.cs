using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Ejercicios;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class EjerciciosController : BusinessController
    {
        private readonly IEjerciciosBusiness _ejerciciosBusiness;

        public EjerciciosController(IEjerciciosBusiness ejerciciosBusiness)
        {
            _ejerciciosBusiness = ejerciciosBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var model = new EjerciciosViewModel();

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _ejerciciosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(EjerciciosViewModel model)
        {
            await _ejerciciosBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var ceco = await _ejerciciosBusiness.Get(id);
            return Json(() => ceco);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ValidarFechaInicio(string mesAnnoInicio)
        {
            var results = await _ejerciciosBusiness.ValidarFecha(mesAnnoInicio, false);
            return Json(results.IsNull() ? true : results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ValidarFechaFin(string mesAnnoFin)
        {
            var results = await _ejerciciosBusiness.ValidarFecha(mesAnnoFin, true);
            return Json(results.IsNull() ? true : results);
        }
    }
}
