using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Sucursales;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class SucursalesController : BusinessController
    {
        private readonly ISucursalesBusiness _sucursalesBusiness;

        public SucursalesController(ISucursalesBusiness sucursalesBusiness)
        {
            _sucursalesBusiness = sucursalesBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public ActionResult Index()
        {
            var commonBoolean = new[]
            {
                new { Id = "N", Descripcion = "No" },
                new { Id = "S", Descripcion = "Si" }
            }
            .ToList();

            var model = new SucursalesViewModel
            {
                MaestroCommonBool = commonBoolean
                    .ToDropDownList(k => k.Id, v => v.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _sucursalesBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(SucursalesViewModel model)
        {
            await _sucursalesBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var sucursal = await _sucursalesBusiness.Get(id);
            return Json(() => sucursal);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetSucursalesNumeracionByIdSucursal(int id)
        {
            var numeracion = await _sucursalesBusiness.GetSucursalesNumeracionByIdSucursal(id);
            return Json(() => numeracion);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ActualizaNumeracion(int idSucursal, int idComprobante, string numerador)
        {
            await _sucursalesBusiness.ActualizaNumeracion(idSucursal, idComprobante, numerador);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> AFIPConsultaNumeracion(int idSucursal, int idComprobante)
        {
            var numeroAFIP = await _sucursalesBusiness.AFIPConsultaNumeracion(idSucursal, idComprobante);
            return Json(() => numeroAFIP);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(SucursalesViewModel model)
        {
            await _sucursalesBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _sucursalesBusiness.Remove(id);
            return Json(() => true);
        }

    }
}
