using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Monedas;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class MonedasController : BusinessController
    {
        private readonly IMonedasBusiness _monedasBusiness;

        public MonedasController(IMonedasBusiness monedasBusiness)
        {
            _monedasBusiness = monedasBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public ActionResult Index()
        {
            return ActivateMenuItem(model: new MonedasViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _monedasBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> Nuevo(MonedasViewModel model)
        {
            await _monedasBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> Obtener(int id)
        {
            var moneda = await _monedasBusiness.Get(id);
            return Json(() => moneda);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> Modificar(MonedasViewModel model)
        {
            await _monedasBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _monedasBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<ActionResult> ObtenerTiposDeCambioDashboard([FromForm] DataTablesRequest request)
        {
            var tiposDeCambio = await _monedasBusiness.GetTiposDeCambio(request);
            return Json(tiposDeCambio);
        }

    }
}
