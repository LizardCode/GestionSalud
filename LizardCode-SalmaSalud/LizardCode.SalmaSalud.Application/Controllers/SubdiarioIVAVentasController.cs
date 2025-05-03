using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.SubdiarioIVAVentas;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class SubdiarioIVAVentasController : BusinessController
    {
        private readonly ISubdiarioIVAVentasBusiness _subdiarioIVAVentasBusiness;
        private readonly ILookupsBusiness _lookupsBusiness;
        private readonly IPermisosBusiness _permisosBusiness;

        public SubdiarioIVAVentasController(ISubdiarioIVAVentasBusiness subdiarioIVAVentasBusiness, ILookupsBusiness lookupsBusiness, IPermisosBusiness permisosBusiness)
        {
            _subdiarioIVAVentasBusiness = subdiarioIVAVentasBusiness;
            _lookupsBusiness = lookupsBusiness;
            _permisosBusiness = permisosBusiness;

        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS POR COBRAR")]
        public async Task<ActionResult> Index()
        {
            var primerDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);
            var clientes = (await _lookupsBusiness.GetAllClientesLookup()).AsList();

            var model = new SubdiarioIVAVentasViewModel
            {
                FechaDesde = primerDiaMes.ToString("dd/MM/yyyy"),
                FechaHasta = ultimoDiaMes.ToString("dd/MM/yyyy"),
                MaestroClientes = clientes.ToDropDownList(k => k.IdCliente, d => d.RazonSocial, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS POR COBRAR")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _subdiarioIVAVentasBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS POR COBRAR")]
        public async Task<ActionResult> DescargarCITIVentas(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var citi = await _subdiarioIVAVentasBusiness.GetCITIVentas(fechaDesde, fechaHasta);
            return File(new MemoryStream(Encoding.ASCII.GetBytes(citi)), "application/text", $"CITI_VENTAS_{fechaDesde.Value.ToString("yyyyMMdd")}_{fechaHasta.Value.ToString("yyyyMMdd")}.TXT");
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS POR COBRAR")]
        public async Task<ActionResult> DescargarCITIVentasAli(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var citi = await _subdiarioIVAVentasBusiness.GetCITIVentasAli(fechaDesde, fechaHasta);
            return File(new MemoryStream(Encoding.ASCII.GetBytes(citi)), "application/text", $"CITI_VENTAS_ALICUOTAS_{fechaDesde.Value.ToString("yyyyMMdd")}_{fechaHasta.Value.ToString("yyyyMMdd")}.TXT");
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS POR COBRAR")]
        public async Task<ActionResult> DetalleAlicuotas(DateTime? fechaDesde, DateTime? fechaHasta, int? idCliente)
        {
            var results = await _subdiarioIVAVentasBusiness.DetalleAlicuotas(fechaDesde, fechaHasta, idCliente);
            return Json(results);
        }

    }
}
