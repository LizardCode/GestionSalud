using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.PrestacionesFinanciador;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class PrestacionesFinanciadorController : BusinessController
    {
        private readonly IFinanciadoresBusiness _financiadoresBusiness;
        private readonly IFinanciadoresPlanesRepository _financiadoresPlanesRepository;
        private readonly IPrestacionesFinanciadorBusiness _prestacionesFinanciadorBusiness;


        public PrestacionesFinanciadorController(IFinanciadoresBusiness financiadoresBusiness,
                                                    IFinanciadoresPlanesRepository financiadoresPlanesRepository,
                                                    IPrestacionesFinanciadorBusiness PrestacionesFinanciadorBusiness)
        {
            _financiadoresBusiness = financiadoresBusiness;
            _financiadoresPlanesRepository = financiadoresPlanesRepository;
            _prestacionesFinanciadorBusiness = PrestacionesFinanciadorBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index(int idFinanciador)
        {
            if (idFinanciador == 0)
                throw new System.Exception("No se ha especificado el Financiador");

            var financiador = await _financiadoresBusiness.Get(idFinanciador);
            if (financiador == null)
                throw new System.Exception("No encontró el Financiador");

            ViewData["Financiador"] = financiador.Nombre.ToUpperInvariant();
            HttpContext.Session.SetInt32("ID_FINANCIADOR", idFinanciador);

            var profesionales = await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa);

            var planes = await _financiadoresPlanesRepository.GetAllByIdFinanciador(idFinanciador);
            var prestaciones = await _lookupsBusiness.GetAllPrestaciones();

            var model = new PrestacionViewModel
            {

                MaestroFinanaciadorPlanes = planes
                    .ToDropDownList(k => k.IdFinanciadorPlan, t => t.Nombre, descriptionIncludesKey: false),

                MaestroPrestaciones = prestaciones
                    .ToDropDownList(k => k.IdPrestacion, t => t.Codigo, descriptionIncludesKey: false),

                MaestroProfesionales = profesionales
                    .ToDropDownList(k => k.IdProfesional, t => t.Nombre, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model, menuItem: "Financiadores");
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var idFinanciador = HttpContext.Session.GetInt32("ID_FINANCIADOR");

            var results = await _prestacionesFinanciadorBusiness.GetAll(request, idFinanciador ?? 0);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(PrestacionViewModel model)
        {
            var idFinanciador = HttpContext.Session.GetInt32("ID_FINANCIADOR");

            await _prestacionesFinanciadorBusiness.New(model, idFinanciador ?? 0);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, PROFESIONAL, PROFESIONAL_EXTERNO")]
        public async Task<JsonResult> Obtener(int id)
        {
            var prestacion = await _prestacionesFinanciadorBusiness.Get(id);
            return Json(() => prestacion);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(PrestacionViewModel model)
        {
            var idFinanciador = HttpContext.Session.GetInt32("ID_FINANCIADOR");

            await _prestacionesFinanciadorBusiness.Update(model, idFinanciador ?? 0);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _prestacionesFinanciadorBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> ValidarCodigo(string codigo, int? idFinanaciadorPrestacion, int? idFinanciadorPlan)
        {
            var result = false;
            var prestacion = await _prestacionesFinanciadorBusiness.GetByCodigo(codigo);

            if (prestacion != null)
            { 
                if (idFinanaciadorPrestacion > 0)
                {
                    result = prestacion.IdFinanciadorPrestacion == idFinanaciadorPrestacion;
                }

                if (idFinanciadorPlan > 0)
                {
                    result = result || (prestacion.IdFinanciadorPlan == idFinanciadorPlan);
                }
            }

            return Json(!result);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ImportarPrestacionesFinanciadorExcel(PrestacionViewModel model)
        {
            var idFinanciador = HttpContext.Session.GetInt32("ID_FINANCIADOR");

            var results = await _prestacionesFinanciadorBusiness.ImportarPrestacionesExcel(model.FileExcel, idFinanciador ?? 0);
            return Json(() => results);
        }
    }
}