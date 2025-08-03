using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Especialidades;
using LizardCode.SalmaSalud.Application.Models.Pacientes;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class EspecialidadesController : BusinessController
    {
        private readonly IEspecialidadesBusiness _especialidadesBusiness;

        public EspecialidadesController(IEspecialidadesBusiness e)
        {
            _especialidadesBusiness = e;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var model = new EspecialidadViewModel();

            return ActivateMenuItem(model: model);
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _especialidadesBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(EspecialidadViewModel model)
        {
            await _especialidadesBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var Especialidad = await _especialidadesBusiness.Get(id);
            return Json(() => Especialidad);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(EspecialidadViewModel model)
        {
            await _especialidadesBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _especialidadesBusiness.Remove(id);
            return Json(() => true);
        }
    }
}
