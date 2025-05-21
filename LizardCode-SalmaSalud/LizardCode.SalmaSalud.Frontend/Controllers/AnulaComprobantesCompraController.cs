using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.AnulaComprobantesCompra;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class AnulaComprobantesCompraController : BusinessController
    {
        private readonly IAnulaComprobantesCompraBusiness _anulaComprobantesCompraBusiness;

        public AnulaComprobantesCompraController(IAnulaComprobantesCompraBusiness anulaComprobantesCompraBusiness)
        {
            _anulaComprobantesCompraBusiness = anulaComprobantesCompraBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var alicuotas = (await base._lookupsBusiness.GetAllAlicuotas()).AsList().Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA).ToList();
            var proveedores = (await _lookupsBusiness.GetAllProveedoresByIdEmpresaLookup(_permisosBusiness.User.IdEmpresa)).AsList();
            var ejercicios = (await _lookupsBusiness.GetEjerciciosByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    IdEjercicio = e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy"))
                })
                .ToList();
            var comprobantes = (await _lookupsBusiness.GetComprobantesParaCredito()).AsList();

            var condiciones = (await _lookupsBusiness.GetAllCondicionVentaCompra((int)TipoCondicion.Compras)).AsList();

            var model = new AnulaComprobantesCompraViewModel
            {
                Items = new List<AnulaComprobantesCompraDetalle>(),
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
                MaestroAlicuotas = alicuotas.ToDropDownList(k => k.Valor, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroComprobantes = comprobantes.ToDropDownList(k => k.IdComprobante, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroProveedores = proveedores.ToDropDownList(k => k.IdProveedor, d => d.RazonSocial, descriptionIncludesKey: false),
                MaestroCondicion = condiciones.ToDropDownList(k => k.IdCondicion, d => d.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _anulaComprobantesCompraBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(AnulaComprobantesCompraViewModel model)
        {
            await _anulaComprobantesCompraBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var estimado = await _anulaComprobantesCompraBusiness.GetCustom(id);
            return Json(() => estimado);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerAsiento(int id)
        {
            var asiento = await _anulaComprobantesCompraBusiness.GetAsientoCustom(id);
            return Json(() => asiento);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _anulaComprobantesCompraBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetItemsFactura(int idComprobanteAnular, string numeroComprobanteAnular, int idProveedor)
        {
            var items = await _anulaComprobantesCompraBusiness.GetItemsNCAnulaByFactura(idComprobanteAnular, numeroComprobanteAnular, idProveedor);
            if(items ==  null)
                return Json(() => null);
            return Json(() => items);
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
