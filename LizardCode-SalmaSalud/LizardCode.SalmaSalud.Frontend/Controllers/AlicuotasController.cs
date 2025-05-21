using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Alicuotas;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class AlicuotasController : BusinessController
    {
        private readonly IAlicuotasBusiness _alicuotasBusiness;

        public AlicuotasController(IAlicuotasBusiness alicuotasBusiness)
        {
            _alicuotasBusiness = alicuotasBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public ActionResult Index()
        {
            var tipoAlicuotas = Utilities.EnumToDictionary<TipoAlicuota>();
            var model = new AlicuotasViewModel
            {
                MaestroTipoAlicuotas = tipoAlicuotas
                    .ToDropDownList(descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _alicuotasBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(AlicuotasViewModel model)
        {
            await _alicuotasBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var alicuta = await _alicuotasBusiness.Get(id);
            return Json(() => alicuta);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(AlicuotasViewModel model)
        {
            await _alicuotasBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _alicuotasBusiness.Remove(id);
            return Json(() => true);
        }

    }
}
