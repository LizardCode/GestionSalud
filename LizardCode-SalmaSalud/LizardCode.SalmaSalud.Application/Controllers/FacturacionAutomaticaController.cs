using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.FacturacionAutomatica;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class FacturacionAutomaticaController : BusinessController
    {
        private readonly IFacturacionAutomaticaBusiness _facturacionAutomaticaBusiness;
        private readonly IEmpresasBusiness _empresasBusiness;

        public FacturacionAutomaticaController(IFacturacionAutomaticaBusiness facturacionAutomaticaBusiness, IEmpresasBusiness empresasBusiness)
        {
            _facturacionAutomaticaBusiness = facturacionAutomaticaBusiness;
            _empresasBusiness = empresasBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var alicuotas = (await _lookupsBusiness.GetAllAlicuotas()).AsList().Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA).ToList();
            var clientes = (await _lookupsBusiness.GetAllClientesLookup()).AsList();
            var monedas = (await _lookupsBusiness.GetAllMonedas()).AsList();
            var sucursales = (await _lookupsBusiness.GetAllSucursalesByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    e.IdSucursal,
                    Descripcion = string.Concat(e.CodigoSucursal, " - ", e.Descripcion)
                })
                .ToList();
            var ejercicios = (await _lookupsBusiness.GetEjerciciosByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy"))
                })
                .ToList();

            var empresa = await _empresasBusiness.GetEmpresaById(_permisosBusiness.User.IdEmpresa);

            var condiciones = (await _lookupsBusiness.GetAllCondicionVentaCompra((int)TipoCondicion.Ventas)).AsList();

            var model = new FacturacionAutomaticaViewModel
            {
                Items = new List<FacturacionAutomaticaDetalle>(),
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
                MaestroAlicuotas = alicuotas.ToDropDownList(k => k.Valor, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroComprobantes = new List<Comprobante>().ToDropDownList(k => k.IdComprobante, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroClientes = clientes.ToDropDownList(k => k.IdCliente, d => d.RazonSocial, descriptionIncludesKey: false),
                MaestroMonedas = monedas.ToDropDownList(k => k.Codigo, d => d.Descripcion, descriptionIncludesKey: false),
                AgentePercepcionAGIP = empresa.AgentePercepcionAGIP,
                AgentePercepcionARBA = empresa.AgentePercepcionARBA,
                MaestroSucursales = new SelectList(sucursales, "IdSucursal", "Descripcion"),
                MaestroNumerosEstimados = new SelectList(new List<object>(), "Numero", "Descripcion"),
                MaestroCondicion = condiciones.ToDropDownList(k => k.IdCondicion, d => d.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _facturacionAutomaticaBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(FacturacionAutomaticaViewModel model)
        {
            await _facturacionAutomaticaBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var estimado = await _facturacionAutomaticaBusiness.GetCustom(id);
            return Json(() => estimado);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerAsiento(int id)
        {
            var asiento = await _facturacionAutomaticaBusiness.GetAsientoCustom(id);
            return Json(() => asiento);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _facturacionAutomaticaBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetComprobantesByCliente(int idCliente)
        {
            var comprobantes = await _lookupsBusiness.GetComprobantesByClienteSinCredito(idCliente);
            return Json(() => comprobantes);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetItemsCliente(DateTime fecha, int idCliente, string idMoneda1, string idMoneda2, bool porCuentaOrden)
        {
            var items = await _facturacionAutomaticaBusiness.GetItemsFacturaByCliente(fecha, idCliente, idMoneda1, idMoneda2, porCuentaOrden);
            if (items == null)
                return Json(() => null);
            return Json(() => items);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetFechaCambio(string idMoneda1, string idMoneda2, DateTime fecha)
        {
            var cotizacion = await _lookupsBusiness.GetFechaCambio(idMoneda1, idMoneda2, fecha, _permisosBusiness.User.IdEmpresa);
            return Json(() => cotizacion);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ValidarComprobantesById(int idComprobanteVenta)
        {
            var comprobanteValid = await _facturacionAutomaticaBusiness.ValidateComprobante(idComprobanteVenta);
            return Json(() => comprobanteValid);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerCAEComprobantesById(int idComprobanteVenta)
        {
            var comprobanteValid = await _facturacionAutomaticaBusiness.ObtenerCAEComprobantesById(idComprobanteVenta);
            return Json(() => comprobanteValid);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetPercepcionesByCliente(int idCliente, int idComprobante, DateTime fecha)
        {
            var percepciones = await _facturacionAutomaticaBusiness.GetPercepcionesByCliente(idCliente, idComprobante, fecha, _permisosBusiness.User.IdEmpresa);
            return Json(() => percepciones);
        }

        //[HttpPost]
        //[Authorize(Roles = "ADMIN, ADMINISTRACION")]
        //public async Task<JsonResult> GetEstimados(string annoMes)
        //{
        //    var estimados = await _facturacionAutomaticaBusiness.GetEstimados(annoMes, _permisosBusiness.User.IdEmpresa);
        //    return Json(() => estimados);
        //}

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Imprimir(int id)
        {
            var comprobante = await _facturacionAutomaticaBusiness.GetCustom(id);
            var pdf = await _impresionesBusiness.GenerarImpresionFactura(id);
            return File(pdf.Content, "application/pdf", $"{comprobante.Comprobante} {comprobante.Sucursal}-{comprobante.Numero} {comprobante.Cliente}.pdf");
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
