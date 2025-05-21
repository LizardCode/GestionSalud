using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Financiadores;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Application.Business;
using LizardCode.SalmaSalud.Application.Models.SdoCtaCteCli;
using Microsoft.Extensions.Options;

namespace LizardCode.SalmaSalud.Controllers
{
    public class FinanciadoresController : BusinessController
    {
        private readonly IFinanciadoresBusiness _financiadoresBusiness;

        public FinanciadoresController(IFinanciadoresBusiness financiadoresBusiness)
        {
            _financiadoresBusiness = financiadoresBusiness;
        }


        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Index()
        {
            var tiposIVA = Utilities.EnumToDictionary<TipoIVA>();
            var tiposTelefonos = Utilities.EnumToDictionary<TipoTelefono>();

            var model = new FinanciadorViewModel
            {
                MaestroTipoIVA = tiposIVA
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroTipoTelefono = tiposTelefonos
                    .ToDropDownList(descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _financiadoresBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Nuevo(FinanciadorViewModel model)
        {
            await _financiadoresBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Obtener(int id)
        {
            var usuario = await _financiadoresBusiness.Get(id);
            return Json(() => usuario);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Modificar(FinanciadorViewModel model)
        {
            await _financiadoresBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _financiadoresBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> ValidarNroCUIT(string cuit, int? idFinanciador)
        {
            var results = await _financiadoresBusiness.ValidarNroCUIT(cuit, idFinanciador);
            return Json(results.IsNull() ? true : results);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<ActionResult> Padron(string cuit)
        {
            var contribuyente = await _financiadoresBusiness.GetPadron(cuit);
            return Json(() => contribuyente);
        }

        //[Authorize(Roles = "ADMIN")]
        //public async Task<ActionResult> ObtenerTotalesDashboard()
        //{
        //    var Financiadores = await _lookupsBusiness.GetAllFinanciadoresByIdEmpresaLookup(_permisosBusiness.User.IdEmpresa);
        //    return Json(() => new { cantidad = Financiadores.Count });
        //}

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> GetPlanesByFinanciadorId([FromForm] int id)
        {
            var planes = await _financiadoresBusiness.GetAllByFinanciadorId(id);
            return Json(() => planes);
        }

        //[HttpPost]
        //[Authorize]
        //public async Task<JsonResult> ProcesarExcel(FinanciadorViewModel model)
        //{
        //    var results = await _financiadoresBusiness.ProcesarExcel(model.FileExcel);
        //    return Json(() => results);
        //}

        [Authorize]
        public async Task<JsonResult> ObtenerPrestacion(int id)
        {
            var prestacion = await _financiadoresBusiness.GetFinanciadorPrestacionById(id);
            return Json(() => prestacion);
        }
    }
}
