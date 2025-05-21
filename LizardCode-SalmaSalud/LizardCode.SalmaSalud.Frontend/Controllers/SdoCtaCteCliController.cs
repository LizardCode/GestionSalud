using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.SdoCtaCteCli;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class SdoCtaCteCliController : BusinessController
    {
        private readonly ISdoCtaCteCliBusiness _sdoCtaCteCliBusiness;

        public SdoCtaCteCliController(ISdoCtaCteCliBusiness sdoCtaCteCliBusiness)
        {
            _sdoCtaCteCliBusiness = sdoCtaCteCliBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var clientes = (await _lookupsBusiness.GetAllClientesLookup()).ToList();
            var comprobantes = (await _lookupsBusiness.GetAllComprobantes()).ToList();
            var alicuotas = (await _lookupsBusiness.GetAllAlicuotas()).ToList().Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA).ToList();

            var model = new SdoCtaCteCliViewModel
            {
                Items = new List<SdoCtaCteCliDetalle>(),

                MaestroClientes = clientes.Select(s=> new { Id = s.IdCliente, Text = string.Format("{0} {1}", s.CUIT, s.NombreFantasia) })
                                            .ToList().ToDropDownList(k => k.Id, v => v.Text, descriptionIncludesKey: false),
                MaestroComprobantes = comprobantes.ToDropDownList(k => k.IdComprobante, v => v.Descripcion, descriptionIncludesKey: false),
                MaestroAlicuotas = alicuotas.ToDropDownList(k => k.Valor, v => v.Descripcion, descriptionIncludesKey: false)
                //MaestroAlicuotas = alicuotas.Select(s => new { Id = s.Valor.ToString().Replace(".",","), Text = s.Descripcion })
                //                            .ToList().ToDropDownList(k => k.Id, v => v.Text, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _sdoCtaCteCliBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(SdoCtaCteCliViewModel model)
        {
            await _sdoCtaCteCliBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var result = await _sdoCtaCteCliBusiness.GetCustom(id);
            return Json(() => result);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(SdoCtaCteCliViewModel model)
        {
            await _sdoCtaCteCliBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _sdoCtaCteCliBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ProcesarExcel(SdoCtaCteCliViewModel model)
        {
            var results = await _sdoCtaCteCliBusiness.ProcesarExcel(model.FileExcel);
            return Json(() => results);
        }
    }
}
