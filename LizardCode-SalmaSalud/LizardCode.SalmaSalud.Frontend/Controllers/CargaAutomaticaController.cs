using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.CargaAutomatica;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class CargaAutomaticaController : BusinessController
    {
        private readonly ICargaAutomaticaBusiness _cargaAutomaticaBusiness;

        public CargaAutomaticaController(ICargaAutomaticaBusiness cargaAutomaticaBusiness)
        {
            _cargaAutomaticaBusiness = cargaAutomaticaBusiness;
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
            var cuentas = (await _lookupsBusiness.GetCuentasContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList()
                .Where(c => c.IdCodigoObservacion == (int)CodigoObservacion.PERCEPCION_ING_BRUTOS || c.IdCodigoObservacion == (int)CodigoObservacion.PERCEPCION_IVA || c.IdCodigoObservacion == (int)CodigoObservacion.IMPUESTOS_INTERNOS)
                .ToList();

            var centroCostos = (await _lookupsBusiness.GetAllCentroCostos(_permisosBusiness.User.IdEmpresa)).AsList();
            var condiciones = (await _lookupsBusiness.GetAllCondicionVentaCompra((int)TipoCondicion.Compras)).AsList();

            var model = new CargaAutomaticaViewModel
            {
                Items = new List<CargaAutomaticaDetalle>(),
                ListaPercepciones = new List<CargaAutomaticaPercepciones>(),
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
                MaestroAlicuotas = alicuotas.ToDropDownList(k => k.Valor, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroComprobantes = new List<Comprobante>().ToDropDownList(k => k.IdComprobante, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroProveedores = proveedores.ToDropDownList(k => k.IdProveedor, d => d.RazonSocial, descriptionIncludesKey: false),
                MaestroMonedas = monedas.ToDropDownList(k => k.Codigo, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroCuentasPercepciones = cuentas.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroCentroCostos = centroCostos.ToDropDownList(k => k.IdCentroCosto, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroCondicion = condiciones.ToDropDownList(k => k.IdCondicion, d => d.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _cargaAutomaticaBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(CargaAutomaticaViewModel model)
        {
            await _cargaAutomaticaBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var comprobante = await _cargaAutomaticaBusiness.GetCustom(id);
            return Json(() => comprobante);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerAsiento(int id)
        {
            var asiento = await _cargaAutomaticaBusiness.GetAsientoCustom(id);
            return Json(() => asiento);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _cargaAutomaticaBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetComprobantesByProveedor(int idProveedor)
        {
            var comprobantes = await _lookupsBusiness.GetComprobantesByProveedor(idProveedor);
            return Json(() => comprobantes);
        }

        //[HttpPost]
        //[Authorize(Roles = "ADMIN, ADMINISTRACION")]
        //public async Task<JsonResult> GetItemsEstimado(string annoMesDesde, string annoMesHasta, DateTime fecha, int idProveedor, string idMoneda1, string idMoneda2)
        //{
        //    var items = await _cargaAutomaticaBusiness.GetItemsFacturaByEstimado(annoMesDesde, annoMesHasta, fecha, idProveedor, idMoneda1, idMoneda2);
        //    if(items ==  null)
        //        return Json(() => null);
        //    return Json(() => items);
        //}

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetFechaCambio(string idMoneda1, string idMoneda2, DateTime fecha)
        {
            var cotizacion = await _lookupsBusiness.GetFechaCambio(idMoneda1, idMoneda2, fecha, _permisosBusiness.User.IdEmpresa);
            return Json(() => cotizacion);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ValidarComprobantesById(int idComprobanteCompra)
        {
            var comprobanteValid = await _cargaAutomaticaBusiness.ValidateComprobante(idComprobanteCompra);
            return Json(() => comprobanteValid);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetVencimiento(int idCondicion, DateTime fecha)
        {
            var condicion = await _lookupsBusiness.GetAllCondicionVentaCompra(null);

            return Json(() => fecha.AddDays(condicion.FirstOrDefault(f => f.IdCondicion == idCondicion).Dias));
        }
    }
}
