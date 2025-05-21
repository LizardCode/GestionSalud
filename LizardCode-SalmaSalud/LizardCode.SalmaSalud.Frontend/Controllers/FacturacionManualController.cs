using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.FacturacionManual;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class FacturacionManualController : BusinessController
    {
        private readonly IFacturacionManualBusiness _facturacionManualBusiness;
        private readonly IEmpresasBusiness _empresasBusiness;

        public FacturacionManualController(IFacturacionManualBusiness facturacionManualBusiness, IEmpresasBusiness empresasBusiness)
        {
            _facturacionManualBusiness = facturacionManualBusiness;
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
                    IdSucursal = e.IdSucursal,
                    Descripcion = string.Concat(e.CodigoSucursal, " - ", e.Descripcion)
                })
                .ToList();
            var ejercicios = (await _lookupsBusiness.GetEjerciciosByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    IdEjercicio = e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy"))
                })
                .ToList();
            var cuentas = (await _lookupsBusiness.GetCuentasContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList()
                .Where(c => c.IdCodigoObservacion == (int)CodigoObservacion.VENTA_DE_SERVICIOS)
                .ToList();

            var empresa = await _empresasBusiness.GetEmpresaById(_permisosBusiness.User.IdEmpresa);

            var condiciones = (await _lookupsBusiness.GetAllCondicionVentaCompra((int)TipoCondicion.Ventas)).AsList();

            var model = new FacturacionManualViewModel
            {
                Items = new List<FacturacionManualDetalle>(),
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
                MaestroAlicuotas = alicuotas.ToDropDownList(k => k.IdAlicuota, d => d.Valor, descriptionIncludesKey: false),
                MaestroComprobantes = new List<Comprobante>().ToDropDownList(k => k.IdComprobante, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroClientes = clientes.ToDropDownList(k => k.IdCliente, d => d.RazonSocial, descriptionIncludesKey: false),
                MaestroMonedas = monedas.ToDropDownList(k => k.Codigo, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroCuentas = cuentas.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false),
                AgentePercepcionAGIP = empresa.AgentePercepcionAGIP,
                AgentePercepcionARBA = empresa.AgentePercepcionARBA,
                MaestroSucursales = new SelectList(sucursales, "IdSucursal", "Descripcion"),
                MaestroCondicion = condiciones.ToDropDownList(k => k.IdCondicion, d => d.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _facturacionManualBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(FacturacionManualViewModel model)
        {
            await _facturacionManualBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var estimado = await _facturacionManualBusiness.GetCustom(id);
            return Json(() => estimado);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerAsiento(int id)
        {
            var asiento = await _facturacionManualBusiness.GetAsientoCustom(id);
            return Json(() => asiento);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _facturacionManualBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetComprobantesByCliente(int idCliente)
        {
            var comprobantes = await _lookupsBusiness.GetComprobantesByCliente(idCliente);
            return Json(() => comprobantes);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetFechaCambio(string idMoneda1, DateTime fecha)
        {
            var cotizacion = await _lookupsBusiness.GetFechaCambio(idMoneda1, fecha, _permisosBusiness.User.IdEmpresa);
            return Json(() => cotizacion);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ValidarComprobantesById(int idComprobanteVenta)
        {
            var comprobanteValid = await _facturacionManualBusiness.ValidateComprobante(idComprobanteVenta);
            return Json(() => comprobanteValid);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerCAEComprobantesById(int idComprobanteVenta)
        {
            var comprobanteValid = await _facturacionManualBusiness.ObtenerCAEComprobantesById(idComprobanteVenta);
            return Json(() => comprobanteValid);
        }
        
        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetPercepcionesByCliente(int idCliente, int idComprobante, DateTime fecha)
        {
            var percepciones = await _facturacionManualBusiness.GetPercepcionesByCliente(idCliente, idComprobante, fecha, _permisosBusiness.User.IdEmpresa);
            return Json(() => percepciones);
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> GetCantidadFacturasCompra()
        {
            var cantidad = await _facturacionManualBusiness.GetContidadFacturasCompra();
            return Json(() => new { cantidad = cantidad });
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> GetCantidadFacturasVenta()
        {
            var cantidad = await _facturacionManualBusiness.GetContidadFacturasVenta();
            return Json(() => new { cantidad = cantidad });
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> GetIVADashboard()
        {
            var importe = await _facturacionManualBusiness.GetIVADashboard();
            return Json(() => new { importe = importe });
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Imprimir(int id)
        {
            var comprobante = await _facturacionManualBusiness.GetCustom(id);
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

        [Authorize(Roles = "PROVEEDOR")]
        public async Task<ActionResult> GetCantidadFacturasCompraProveedor()
        {
            var cantidad = await _facturacionManualBusiness.GetCantidadFacturasComprasProveedor();
            return Json(() => new { cantidad = cantidad });
        }

        [Authorize(Roles = "PROVEEDOR")]
        public async Task<ActionResult> GetCantidadFacturasCompraPagasProveedor()
        {
            var cantidad = await _facturacionManualBusiness.GetCantidadFacturasComprasPagasProveedor();
            return Json(() => new { cantidad = cantidad });
        }

        [HttpPost]
        [Authorize(Roles = "PROVEEDOR")]
        public async Task<ActionResult> GetUltimasFacturasProveedor([FromForm] DataTablesRequest request)
        {
            var facturas = await _facturacionManualBusiness.GetUltimasFacturasProveedor(request);
            return Json(facturas);
        }
    }
}
