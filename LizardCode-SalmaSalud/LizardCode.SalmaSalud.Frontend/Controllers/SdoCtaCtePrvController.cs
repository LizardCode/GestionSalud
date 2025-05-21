using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.SdoCtaCtePrv;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class SdoCtaCtePrvController : BusinessController
    {
        private readonly ISdoCtaCtePrvBusiness _sdoCtaCtePrvBusiness;

        public SdoCtaCtePrvController(ISdoCtaCtePrvBusiness sdoCtaCtePrvBusiness)
        {
            _sdoCtaCtePrvBusiness = sdoCtaCtePrvBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var proveedores = (await _lookupsBusiness.GetAllProveedoresByIdEmpresaLookup(_permisosBusiness.User.IdEmpresa)).ToList();
            var comprobantes = (await _lookupsBusiness.GetAllComprobantes()).ToList();
            var alicuotas = (await _lookupsBusiness.GetAllAlicuotas()).ToList().Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA).ToList();

            var model = new SdoCtaCtePrvViewModel
            {
                Items = new List<SdoCtaCtePrvDetalle>(),

                MaestroProveedores = proveedores.Select(s => new { Id = s.IdProveedor, Text = string.Format("{0} {1}", s.CUIT, s.NombreFantasia) })
                                            .ToList().ToDropDownList(k => k.Id, v => v.Text, descriptionIncludesKey: false),
                MaestroComprobantes = comprobantes.ToDropDownList(k => k.IdComprobante, v => v.Descripcion, descriptionIncludesKey: false),
                MaestroAlicuotas = alicuotas.ToDropDownList(k => k.Valor, v => v.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _sdoCtaCtePrvBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(SdoCtaCtePrvViewModel model)
        {
            await _sdoCtaCtePrvBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var result = await _sdoCtaCtePrvBusiness.GetCustom(id);
            return Json(() => result);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(SdoCtaCtePrvViewModel model)
        {
            await _sdoCtaCtePrvBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _sdoCtaCtePrvBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ProcesarExcel(SdoCtaCtePrvViewModel model)
        {
            var results = await _sdoCtaCtePrvBusiness.ProcesarExcel(model.FileExcel);
            return Json(() => results);
        }
    }
}
