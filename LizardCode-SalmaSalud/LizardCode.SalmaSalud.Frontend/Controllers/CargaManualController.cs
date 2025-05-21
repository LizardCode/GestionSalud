using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.CargaManual;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Helpers.Utilities;

namespace LizardCode.SalmaSalud.Controllers
{
    public class CargaManualController : BusinessController
    {
        private readonly ICargaManualBusiness _cargaManualBusiness;

        public CargaManualController(ICargaManualBusiness cargaManualBusiness)
        {
            _cargaManualBusiness = cargaManualBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var alicuotas = (await _lookupsBusiness.GetAllAlicuotas()).AsList().Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA).ToList();
            var proveedores = (await _lookupsBusiness.GetAllProveedoresByIdEmpresaLookup(_permisosBusiness.User.IdEmpresa)).AsList();
            var monedas = (await _lookupsBusiness.GetAllMonedas()).AsList();
            var ejercicios = (await _lookupsBusiness.GetEjerciciosByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy"))
                })
                .ToList();
            var cuentas = (await _lookupsBusiness.GetCuentasContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList();

            var ctasPercepcion = cuentas
                .Where(c => c.IdCodigoObservacion == (int)CodigoObservacion.PERCEPCION_ING_BRUTOS || c.IdCodigoObservacion == (int)CodigoObservacion.PERCEPCION_IVA || c.IdCodigoObservacion == (int)CodigoObservacion.IMPUESTOS_INTERNOS)
                .ToList();

            var tipoInterfaz = new[]
            {
                new { Id = (int)TipoInterfaz.MisComprobantesAFIP, Descripcion = TipoInterfaz.MisComprobantesAFIP.Description() },
                new { Id = (int)TipoInterfaz.CustomInterfaz, Descripcion = TipoInterfaz.CustomInterfaz.Description() }
            }
            .ToList();

            var ctasGastos = cuentas.Where(c => c.EsCtaGastos).ToList();
            var centroCostos = (await _lookupsBusiness.GetAllCentroCostos(_permisosBusiness.User.IdEmpresa)).AsList();
            var condiciones = (await _lookupsBusiness.GetAllCondicionVentaCompra((int)TipoCondicion.Compras)).AsList();

            var model = new CargaManualViewModel
            {
                Items = new List<CargaManualDetalle>(),
                ListaPercepciones = new List<CargaManualPercepciones>(),
                MaestroEjercicios = ejercicios.Count == 0 ? default : new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
                MaestroAlicuotas = alicuotas?.ToDropDownList(k => k.IdAlicuota, d => d.Valor, descriptionIncludesKey: false) ?? default,
                MaestroComprobantes = new List<Comprobante>().ToDropDownList(k => k.IdComprobante, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroProveedores = proveedores?.ToDropDownList(k => k.IdProveedor, d => d.RazonSocial, descriptionIncludesKey: false) ?? default,
                MaestroMonedas = monedas?.ToDropDownList(k => k.Codigo, d => d.Descripcion, descriptionIncludesKey: false) ?? default,
                MaestroCuentasPercepciones = ctasPercepcion?.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false) ?? default,
                MaestroCuentas = ctasGastos?.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false) ?? default,
                MaestroCentroCostos = centroCostos?.ToDropDownList(k => k.IdCentroCosto, d => d.Descripcion, descriptionIncludesKey: false) ?? default,
                MaestroCondicion = condiciones?.ToDropDownList(k => k.IdCondicion, d => d.Descripcion, descriptionIncludesKey: false) ?? default,
                MaestroInterfaz = tipoInterfaz.ToDropDownList(k => k.Id, d => d.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _cargaManualBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> Nuevo(CargaManualViewModel model)
        {
            await _cargaManualBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> Obtener(int id)
        {
            var comprobante = await _cargaManualBusiness.GetCustom(id);
            return Json(() => comprobante);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ObtenerItems(int id)
        {
            var comprobante = await _cargaManualBusiness.GetItemsPagosCustom(id);
            return Json(() => comprobante);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ObtenerAsiento(int id)
        {
            var asiento = await _cargaManualBusiness.GetAsientoCustom(id);
            return Json(() => asiento);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _cargaManualBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> Modificar(CargaManualViewModel model)
        {
            await _cargaManualBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> GetComprobantesByProveedor(int idProveedor)
        {
            var comprobantes = await _lookupsBusiness.GetComprobantesByProveedor(idProveedor);
            return Json(() => comprobantes);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> GetFechaCambio(string idMoneda1, DateTime fecha)
        {
            var cotizacion = await _lookupsBusiness.GetFechaCambio(idMoneda1, fecha, _permisosBusiness.User.IdEmpresa);
            return Json(() => cotizacion);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ValidarComprobantesById(int idComprobanteCompra)
        {
            var comprobanteValid = await _cargaManualBusiness.ValidateComprobante(idComprobanteCompra);
            return Json(() => comprobanteValid);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> GetVencimiento(int idCondicion, DateTime fecha)
        {
            var condicion = await _lookupsBusiness.GetAllCondicionVentaCompra(null);

            return Json(() => fecha.AddDays(condicion.FirstOrDefault(f => f.IdCondicion == idCondicion).Dias));
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ProcesarCSV(CargaManualViewModel model)
        {
            var results = model.IdTipoInterfaz switch
            {
                (int)TipoInterfaz.MisComprobantesAFIP => await _cargaManualBusiness.ProcesarCSV(model.FileCSV, model.FechaInterfaz, model.IdEjercicioInterfaz, model.IdCuentaContable),
                (int)TipoInterfaz.CustomInterfaz => await _cargaManualBusiness.ProcesarCustomDawa(model.FileCSV, model.FechaInterfaz, model.IdEjercicioInterfaz),
                _ => throw new BusinessException("El Tipo de Interfaz no se encuentra Implementada. Contacte al Administrador del Sistema.")
            };

            return Json(() => results);
        }
    }
}
