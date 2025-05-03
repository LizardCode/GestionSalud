using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.OpenXmlFormats.Spreadsheet;
using LizardCode.SalmaSalud.Application.Business;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Prestaciones;
using LizardCode.SalmaSalud.Application.Models.SdoCtaCteCli;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Numerics;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class PrestacionesController : BusinessController
    {
        private readonly IPrestacionesBusiness _prestacionesBusiness;

        public PrestacionesController(IPrestacionesBusiness prestacionesBusiness)
        {
            _prestacionesBusiness = prestacionesBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var profesionales = await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa);

            var model = new PrestacionViewModel
            {
                MaestroProfesionales = profesionales
                                        .ToDropDownList(k => k.IdProfesional, t => t.Nombre, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _prestacionesBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(PrestacionViewModel model)
        {
            await _prestacionesBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, PROFESIONAL, PROFESIONAL_EXTERNO")]
        public async Task<JsonResult> Obtener(int id)
        {
            var prestacion = await _prestacionesBusiness.Get(id);
            return Json(() => prestacion);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(PrestacionViewModel model)
        {
            await _prestacionesBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _prestacionesBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> ValidarCodigo(string codigo)
        {
            var result = await _prestacionesBusiness.ValidarCodigo(codigo);
            return Json(result);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ImportarPrestacionesExcel(PrestacionViewModel model)
        {
            var results = await _prestacionesBusiness.ImportarPrestacionesExcel(model.FileExcel);
            return Json(() => results);
        }
    }
}