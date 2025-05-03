using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.CondicionVentasCompras;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class CondicionVentasComprasController : BusinessController
    {
        private readonly ICondicionVentasComprasBusiness _condicionVentasComprasBusiness;

        public CondicionVentasComprasController(ICondicionVentasComprasBusiness condicionVentasComprasBusiness)
        {
            _condicionVentasComprasBusiness = condicionVentasComprasBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public ActionResult Index()
        {
            var tipoCondicion = Utilities.EnumToDictionary<TipoCondicion>();
            var model = new CondicionVentasComprasViewModel
            {
                MaestroTipoCondicion = tipoCondicion
                    .ToDropDownList(descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _condicionVentasComprasBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(CondicionVentasComprasViewModel model)
        {
            await _condicionVentasComprasBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var condicion = await _condicionVentasComprasBusiness.Get(id);
            return Json(() => condicion);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(CondicionVentasComprasViewModel model)
        {
            await _condicionVentasComprasBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _condicionVentasComprasBusiness.Remove(id);
            return Json(() => true);
        }

    }
}
