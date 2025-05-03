using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.CodigosRetencion;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class CodigosRetencionController : BusinessController
    {
        private readonly ICodigosRetencionBusiness _codigosRetencionBusiness;

        public CodigosRetencionController(ICodigosRetencionBusiness codigosRetencionBusiness)
        {
            _codigosRetencionBusiness = codigosRetencionBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var tipoRetencion = Utilities.EnumToDictionary<Domain.Enums.TipoRetencion>();
            var model = new CodigosRetencionViewModel
            {
                MaestroTipoRetencion = tipoRetencion
                    .ToDropDownList(descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _codigosRetencionBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(CodigosRetencionViewModel model)
        {
            await _codigosRetencionBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var codigosRetencion = await _codigosRetencionBusiness.GetCustom(id);
            return Json(() => codigosRetencion);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(CodigosRetencionViewModel model)
        {
            await _codigosRetencionBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _codigosRetencionBusiness.Remove(id);
            return Json(() => true);
        }

    }
}
