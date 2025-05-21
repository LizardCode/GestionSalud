using Dapper;
using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Impresiones;
using LizardCode.SalmaSalud.Application.Models.OrdenesPago;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class OrdenesPagoController : BusinessController
    {
        private readonly IOrdenesPagoBusiness _ordenesPagoBusiness;
        private readonly IChequesBusiness _chequesBusiness;
        private readonly IMemoryCache _memoryCache;

        public OrdenesPagoController(IOrdenesPagoBusiness ordenesPagoBusiness, IChequesBusiness chequesBusiness, IMemoryCache memoryCache)
        {
            _ordenesPagoBusiness = ordenesPagoBusiness;
            _chequesBusiness = chequesBusiness;
            _memoryCache = memoryCache;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<ActionResult> Index()
        {
            var proveedores = (await _lookupsBusiness.GetAllProveedoresByIdEmpresaLookup(_permisosBusiness.User.IdEmpresa)).AsList();
            var bancos = (await _lookupsBusiness.GetBancosByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList();
            var monedas = (await _lookupsBusiness.GetAllMonedas()).AsList();
            var cuentas = (await _lookupsBusiness.GetCuentasContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList();
            var cuentasGastos = cuentas.Where(c => c.EsCtaGastos).ToList();
            var ejercicios = (await _lookupsBusiness.GetEjerciciosByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy"))
                })
                .ToList();

            var model = new OrdenesPagoViewModel
            {
                Items = new List<OrdenesPagoDetalle>(),
                MaestroProveedores = proveedores?.ToDropDownList(k => k.IdProveedor, d => d.RazonSocial, descriptionIncludesKey: false) ?? default,
                MaestroEstadoOrdenPago = Utilities.EnumToDictionary<EstadoOrdenPago>().ToDropDownList(descriptionIncludesKey: false),
                MaestroBancos = bancos?.ToDropDownList(k => k.IdBanco, d => d.Descripcion, descriptionIncludesKey: false) ?? default,
                MaestroEjercicios = ejercicios.Count == 0 ? default : new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
                MaestroTipoPago = Utilities.EnumToDictionary<TipoPago>().ToDropDownList(descriptionIncludesKey: false),
                MaestroMonedas = monedas.ToDropDownList(k => k.Codigo, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroTipoOrdenPago = Utilities.EnumToDictionary<TipoOrdenPago>().ToDropDownList(descriptionIncludesKey: false),
                MaestroCuentas = cuentas?.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false) ?? default,
                MaestroCuentasGastos = cuentasGastos?.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false) ?? default
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _ordenesPagoBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> Nuevo(OrdenesPagoViewModel model)
        {
            await _ordenesPagoBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> Obtener(int id)
        {
            var ordenPago = await _ordenesPagoBusiness.Get(id);
            return Json(() => ordenPago);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _ordenesPagoBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ObtenerAsiento(int id)
        {
            var asiento = await _ordenesPagoBusiness.GetAsientoCustom(id);
            return Json(() => asiento);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ObtenerComprobantesImputar(int id, string idMoneda, string idMonedaPago, double cotizacion)
        {
            var comprobantes = await _ordenesPagoBusiness.GetComprobantesImputar(id, idMoneda, idMonedaPago, cotizacion);
            return Json(() => comprobantes);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ObtenerPlanillasImputar(string idMoneda, string idMonedaPago, double cotizacion)
        {
            var planillas = await _ordenesPagoBusiness.GetPlanillasImputar(idMoneda, idMonedaPago, cotizacion);
            return Json(() => planillas);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> AddPagar(OrdenesPagoViewModel model)
        {
            await _ordenesPagoBusiness.AddPagar(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> GetChequesCartera(string q)
        {
            var cheques = await _chequesBusiness.GetChequesCartera(q);
            return Json(() => cheques);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> GetPrimerChequeDisponible(int idBanco, int idTipoCheque)
        {
            var cheque = await _chequesBusiness.GetPrimerChequeDisponible(idBanco, idTipoCheque);
            return Json(() => cheque);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ValidarNumeroChequeDisponible(int idBanco, int idTipoCheque, string nroCheque)
        {
            var valid = await _chequesBusiness.ValidarNumeroChequeDisponible(idBanco, idTipoCheque, nroCheque);
            return Json(() => valid);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ObtenerDetallePago(int id)
        {
            var pagos = await _ordenesPagoBusiness.ObtenerDetallePago(id);
            return Json(() => pagos);
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
        public async Task<JsonResult> VerificarChequeDisponible(int idBanco, int idTipoCheque, string nroCheque)
        {
            var chequeDisponible = await _ordenesPagoBusiness.VerificarChequeDisponible(idBanco, idTipoCheque, nroCheque);
            return Json(() => chequeDisponible);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> ObtenerAnticiposImputar(int idProveedor, string idMoneda)
        {
            var anticipos = await _ordenesPagoBusiness.GetAnticiposImputar(idProveedor, idMoneda);
            return Json(() => anticipos);
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR, PROVEEDOR")]
        public async Task<ActionResult> Imprimir(int id)
        {
            var ordenPago = await _ordenesPagoBusiness.Get(id);
            var fileName = $"ORDEN DE PAGO {ordenPago.IdOrdenPago}.pdf";
            var pdf = fileName.FromCache<PDF>();

            if (pdf != null)
                return File(pdf.Content, "application/pdf", fileName);
            else
            {
                pdf = await _impresionesBusiness.GenerarImpresionOrdenPago(id);
                _memoryCache.Set(fileName, pdf, TimeSpan.FromMinutes(5));

                return File(pdf.Content, "application/pdf", fileName);
            }
        }


        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR, PROVEEDOR")]
        public async Task<IActionResult> GenerarRetencionPDF(int id, int idTipoRetencion)
        {
            try
            {
                var pdf = await _impresionesBusiness.GenerarImpresionRetenciones(id, idTipoRetencion);
                if (pdf != null)
                {
                    var fileGUID = Guid.NewGuid().ToString("N");
                    var fileName = $"Retenciones_{id}_{idTipoRetencion}.pdf";
                    _memoryCache.Set(fileGUID, pdf, TimeSpan.FromMinutes(5));

                    return Json(new { success = true, message = fileGUID, fileName });
                }
                else
                {
                    return Json(new { success = false, message = "No se encontro el registro." });
                }
            }
            catch (BusinessException bEx)
            {
                return Json(new { success = false, message = bEx.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult DescargarRetencionPDF(string fileGUID, string fileName)
        {
            var pdf = fileGUID.FromCache<PDF>();

            if (pdf != null)
                return File(pdf.Content, "application/pdf", fileName);

            return new EmptyResult();
        }
    }
}
