using Dapper;
using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.PlanillaGastos;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class PlanillaGastosController : BusinessController
    {
        private readonly IPlanillaGastosBusiness _planillaGastosBusiness;

        public PlanillaGastosController(IPlanillaGastosBusiness planillaGastosBusiness)
        {
            _planillaGastosBusiness = planillaGastosBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<ActionResult> Index()
        {
            var ejercicios = (await _lookupsBusiness.GetEjerciciosByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy"))
                })
                .ToList();

            var cuentas = (await _lookupsBusiness.GetCuentasContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList();
            var alicuotas = (await _lookupsBusiness.GetAllAlicuotas()).AsList().Where(a => a.IdTipoAlicuota == (int)Domain.Enums.TipoAlicuota.IVA).ToList();
            var ctasPercepcion = cuentas
                .Where(c => c.IdCodigoObservacion == (int)CodigoObservacion.PERCEPCION_ING_BRUTOS || c.IdCodigoObservacion == (int)CodigoObservacion.PERCEPCION_IVA || c.IdCodigoObservacion == (int)CodigoObservacion.IMPUESTOS_INTERNOS)
                .ToList();

            var monedas = (await _lookupsBusiness.GetAllMonedas()).AsList();
            var ctasGastos = cuentas.Where(c => c.EsCtaGastos).ToList();
            var comprobantes = (await _lookupsBusiness.GetAllComprobantes()).Where(c => !c.EsCredito).AsList();

            var model = new PlanillaGastosViewModel
            {
                Fecha = DateTime.Now,
                Items = new List<PlanillaGastosDetalle>(),
                MaestroAlicuotas = alicuotas.ToDropDownList(k => k.IdAlicuota, d => d.Valor, descriptionIncludesKey: false),
                MaestroComprobantes = comprobantes.ToDropDownList(k => k.IdComprobante, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroEstadoPlanilla = Utilities.EnumToDictionary<EstadoPlanillaGastos>().ToDropDownList(descriptionIncludesKey: false),
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
                MaestroMonedas = monedas.ToDropDownList(k => k.Codigo, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroCuentasContables = ctasGastos.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false),
				MaestroCuentasPercepcion = ctasPercepcion.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false)
			};

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _planillaGastosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> Nuevo(PlanillaGastosViewModel model)
        {
            await _planillaGastosBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> Obtener(int id)
        {
            var asiento = await _planillaGastosBusiness.GetCustom(id);
            return Json(() => asiento);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ObtenerAsiento(int id)
        {
            var asiento = await _planillaGastosBusiness.GetAsientoCustom(id);
            return Json(() => asiento);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> Modificar(PlanillaGastosViewModel model)
        {
            await _planillaGastosBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _planillaGastosBusiness.Remove(id);
            return Json(() => true);
        }

        //[HttpPost]
        //[Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        //public async Task<JsonResult> GetItemsGastos(string annoMes, int numero, string moneda)
        //{
        //    var items = await _planillaGastosBusiness.GetItemsGastos(annoMes, numero, moneda);
        //    return Json(() => items);
        //}

        //[HttpPost]
        //[Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        //public async Task<JsonResult> GetSaldoItemGastos(string annoMes, int numero, int item)
        //{
        //    var sdo = await _planillaGastosBusiness.GetSaldoItemGastos(annoMes, numero, item);
        //    return Json(() => sdo);
        //}

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> GetProveedorByCUIT([FromForm] string cuit)
        {
            var proveedor = await _lookupsBusiness.GetProveedorByCUIT(cuit, _permisosBusiness.User.IdEmpresa);
            return Json(() => proveedor?.RazonSocial ?? string.Empty);
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ProcesarExcel(PlanillaGastosViewModel model)
        {
            var results = await _planillaGastosBusiness.ProcesarExcel(model.FileExcel);
            return Json(() => results);
        }

    }
}
