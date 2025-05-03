using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Guardias;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class GuardiasController : BusinessController
    {
        private readonly IGuardiasBusiness _guardiasBusiness;

        public GuardiasController(IGuardiasBusiness guardiasBusiness)
        {
            _guardiasBusiness = guardiasBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<ActionResult> Index()
        {
            var profesionales = (await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa)).ToList();
            var estados = Utilities.EnumToDictionary<EstadoGuardia>();

            var model = new GuardiasViewModel
            {

                MaestroProfesionales = profesionales
                    .ToDropDownList(k => k.IdProfesional, t => t.Nombre, descriptionIncludesKey: false),

                MaestroEstados = estados.ToDropDownList(descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _guardiasBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Nuevo(GuardiasViewModel model)
        {
            await _guardiasBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Obtener(int id)
        {
            var liquidacion = await _guardiasBusiness.Get(id);
            return Json(() => liquidacion);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _guardiasBusiness.Remove(id);
            return Json(() => true);
        }
    }
}
