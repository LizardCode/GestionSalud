using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.CentrosCosto;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class CentrosCostoController : BusinessController
    {
        private readonly ICentrosCostoBusiness _centrosCostoBusiness;

        public CentrosCostoController(ICentrosCostoBusiness centrosCostoBusiness)
        {
            _centrosCostoBusiness = centrosCostoBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS")]
        public async Task<ActionResult> Index()
        {
            var model = new CentrosCostoViewModel();
            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _centrosCostoBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS")]
        public async Task<JsonResult> Nuevo(CentrosCostoViewModel model)
        {
            await _centrosCostoBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS")]
        public async Task<JsonResult> Obtener(int id)
        {
            var ceco = await _centrosCostoBusiness.Get(id);
            return Json(() => ceco);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS")]
        public async Task<JsonResult> Modificar(CentrosCostoViewModel model)
        {
            await _centrosCostoBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _centrosCostoBusiness.Remove(id);
            return Json(() => true);
        }
    }
}
