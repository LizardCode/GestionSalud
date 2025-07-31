using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.RangosHorarios;
using System.Threading.Tasks;
using System.Linq;
using LizardCode.Framework.Application.Common.Extensions;

namespace LizardCode.SalmaSalud.Controllers
{
    public class RangosHorariosController : BusinessController
    {
        private readonly IRangosHorariosBusiness _RangosHorariosBusiness;

        public RangosHorariosController(IRangosHorariosBusiness RangosHorariosBusiness)
        {
            _RangosHorariosBusiness = RangosHorariosBusiness;
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<ActionResult> Index()
        {
            var especialidades = (await _lookupsBusiness.GetAllEspecialidades()).ToList();

            var model = new RangosHorariosViewModel
            {
                MaestroEspecialidades = especialidades
                    .ToDropDownList(k => k.IdEspecialidad, t => t.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _RangosHorariosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Nuevo(RangosHorariosViewModel model)
        {
            await _RangosHorariosBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var rango = await _RangosHorariosBusiness.Get(id);
            return Json(() => rango);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Modificar(RangosHorariosViewModel model)
        {
            await _RangosHorariosBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _RangosHorariosBusiness.Remove(id);
            return Json(() => true);
        }
    }
}
