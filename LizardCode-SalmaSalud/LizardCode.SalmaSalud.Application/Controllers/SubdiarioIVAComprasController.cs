using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.SubdiarioIVACompras;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class SubdiarioIVAComprasController : BusinessController
    {
        private readonly ISubdiarioIVAComprasBusiness _subdiarioIVAComprasBusiness;
        private readonly ILookupsBusiness _lookupsBusiness;
        private readonly IPermisosBusiness _permisosBusiness;

        public SubdiarioIVAComprasController(ISubdiarioIVAComprasBusiness subdiarioIVAComprasBusiness, ILookupsBusiness lookupsBusiness, IPermisosBusiness permisosBusiness)
        {
            _subdiarioIVAComprasBusiness = subdiarioIVAComprasBusiness;
            _lookupsBusiness = lookupsBusiness;
            _permisosBusiness = permisosBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<ActionResult> Index()
        {
            var primerDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);
            var proveedores = (await _lookupsBusiness.GetAllProveedoresByIdEmpresaLookup(_permisosBusiness.User.IdEmpresa)).AsList();

            var model = new SubdiarioIVAComprasViewModel
            {
                FechaDesde = primerDiaMes.ToString("dd/MM/yyyy"),
                FechaHasta = ultimoDiaMes.ToString("dd/MM/yyyy"),
                MaestroProveedores = proveedores.ToDropDownList(k => k.IdProveedor, d => d.RazonSocial, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _subdiarioIVAComprasBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<ActionResult> DescargarCITICompras(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var citi = await _subdiarioIVAComprasBusiness.GetCITICompras(fechaDesde, fechaHasta);
            return File(new MemoryStream(Encoding.ASCII.GetBytes(citi)), "application/text", $"CITI_COMPRAS_{fechaDesde.Value.ToString("yyyyMMdd")}_{fechaHasta.Value.ToString("yyyyMMdd")}.TXT");
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS POR PAGAR")]
        public async Task<ActionResult> DescargarCITIComprasAli(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var citi = await _subdiarioIVAComprasBusiness.GetCITIComprasAli(fechaDesde, fechaHasta);
            return File(new MemoryStream(Encoding.ASCII.GetBytes(citi)), "application/text", $"CITI_COMPRAS_ALICUOTAS_{fechaDesde.Value.ToString("yyyyMMdd")}_{fechaHasta.Value.ToString("yyyyMMdd")}.TXT");
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<ActionResult> DetalleAlicuotas(DateTime? fechaDesde, DateTime? fechaHasta, int? idProveedor)
        {
            var results = await _subdiarioIVAComprasBusiness.DetalleAlicuotas(fechaDesde, fechaHasta, idProveedor);
            return Json(results);
        }

    }
}
