using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Feriados;
using LizardCode.SalmaSalud.Application.Models.Pacientes;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class FeriadosController : BusinessController
    {
        private readonly IFeriadosBusiness _feriadosBusiness;

        public FeriadosController(IFeriadosBusiness feriadosBusiness)
        {
            _feriadosBusiness = feriadosBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var tiposFeriados = Utilities.EnumToDictionary<TipoFeriado>();            

            var model = new FeriadoViewModel
            {
                MaestroTiposFeriado = tiposFeriados
                    .ToDropDownList(descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _feriadosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(FeriadoViewModel model)
        {
            await _feriadosBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var feriado = await _feriadosBusiness.Get(id);
            return Json(() => feriado);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(FeriadoViewModel model)
        {
            await _feriadosBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _feriadosBusiness.Remove(id);
            return Json(() => true);
        }
    }
}
