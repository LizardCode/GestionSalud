using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.AnulaComprobantesVenta;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class AnulaComprobantesVentaController : BusinessController
    {
        private readonly IAnulaComprobantesVentaBusiness _anulaComprobantesVentaBusiness;

        public AnulaComprobantesVentaController(IAnulaComprobantesVentaBusiness anulaComprobantesVentaBusiness)
        {
            _anulaComprobantesVentaBusiness = anulaComprobantesVentaBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var alicuotas = (await _lookupsBusiness.GetAllAlicuotas()).AsList().Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA).ToList();
            var clientes = (await _lookupsBusiness.GetAllClientesLookup()).AsList();
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
            var comprobantes = (await _lookupsBusiness.GetComprobantesParaCredito()).AsList();

            var condiciones = (await _lookupsBusiness.GetAllCondicionVentaCompra((int)TipoCondicion.Ventas)).AsList();

            var model = new AnulaComprobantesVentaViewModel
            {
                Items = new List<AnulaComprobantesVentaDetalle>(),
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
                MaestroAlicuotas = alicuotas.ToDropDownList(k => k.Valor, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroComprobantes = comprobantes.ToDropDownList(k => k.IdComprobante, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroClientes = clientes.ToDropDownList(k => k.IdCliente, d => d.RazonSocial, descriptionIncludesKey: false),
                MaestroSucursales = new SelectList(sucursales, "IdSucursal", "Descripcion"),
                MaestroCondicion = condiciones.ToDropDownList(k => k.IdCondicion, d => d.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _anulaComprobantesVentaBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(AnulaComprobantesVentaViewModel model)
        {
            await _anulaComprobantesVentaBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var estimado = await _anulaComprobantesVentaBusiness.GetCustom(id);
            return Json(() => estimado);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerAsiento(int id)
        {
            var asiento = await _anulaComprobantesVentaBusiness.GetAsientoCustom(id);
            return Json(() => asiento);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _anulaComprobantesVentaBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetItemsFactura(int idComprobanteAnular, string sucursalAnular, string numeroComprobanteAnular)
        {
            var items = await _anulaComprobantesVentaBusiness.GetItemsNCAnulaByFactura(idComprobanteAnular, sucursalAnular, numeroComprobanteAnular);
            if(items ==  null)
                return Json(() => null);
            return Json(() => items);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ValidarComprobantesById(int idComprobanteVenta)
        {
            var comprobanteValid = await _anulaComprobantesVentaBusiness.ValidateComprobante(idComprobanteVenta);
            return Json(() => comprobanteValid);
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Imprimir(int id)
        {
            var pdf = await _impresionesBusiness.GenerarImpresionFactura(id);
            return File(pdf.Content, "application/pdf");
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
