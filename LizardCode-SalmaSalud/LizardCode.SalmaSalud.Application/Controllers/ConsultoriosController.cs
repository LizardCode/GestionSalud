using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Business;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Consultorios;
using LizardCode.SalmaSalud.Application.Models.Pacientes;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class ConsultoriosController : BusinessController
    {
        private readonly IConsultoriosBusiness _consultoriosBusiness;

        public ConsultoriosController(IConsultoriosBusiness consultoriosBusiness)
        {
            _consultoriosBusiness = consultoriosBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            return ActivateMenuItem(model: new ConsultorioViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _consultoriosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(ConsultorioViewModel model)
        {
            await _consultoriosBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var consultorio = await _consultoriosBusiness.Get(id);
            return Json(() => consultorio);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(ConsultorioViewModel model)
        {
            await _consultoriosBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _consultoriosBusiness.Remove(id);
            return Json(() => true);
        }
    }
}