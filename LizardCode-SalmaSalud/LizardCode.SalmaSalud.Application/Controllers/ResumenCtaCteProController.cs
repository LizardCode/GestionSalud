using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.ResumenCtaCtePro;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class ResumenCtaCteProController : BusinessController
    {
		private readonly IResumenCtaCteProBusiness _resumenCtaCteProBusiness;

		public ResumenCtaCteProController(IResumenCtaCteProBusiness resumenCtaCteProBusiness)
		{
			_resumenCtaCteProBusiness = resumenCtaCteProBusiness;
		}

		[Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
		public async Task<ActionResult> Index()
		{
			var primerDiaMes = await _lookupsBusiness.GetMinFechaComprobanteCompra(_permisosBusiness.User.IdEmpresa);
			var ultimoDiaMes = await _lookupsBusiness.GetMaxFechaComprobanteCompra(_permisosBusiness.User.IdEmpresa);

			var primerDiaMesOPago = await _lookupsBusiness.GetMinFechaOrdenPago(_permisosBusiness.User.IdEmpresa);
			var ultimoDiaMesOPago = await _lookupsBusiness.GetMaxFechaOrdenPago(_permisosBusiness.User.IdEmpresa);

			if (primerDiaMes > primerDiaMesOPago)
				primerDiaMes = primerDiaMesOPago;

			if (ultimoDiaMes < ultimoDiaMesOPago)
				ultimoDiaMes = ultimoDiaMesOPago;

			var proveedores = (await _lookupsBusiness.GetAllProveedoresByIdEmpresaLookup(_permisosBusiness.User.IdEmpresa)).AsList();
			var saldosEnCero = new[]
			{
				new { Id = 1, Descripcion = "Si" },
				new { Id = 0, Descripcion = "No" }
			}
			.ToList();

			var model = new ResumenCtaCteProViewModel
			{
				FechaDesde = primerDiaMes.HasValue ? primerDiaMes.Value.ToString("dd/MM/yyyy") : string.Empty,
				FechaHasta = ultimoDiaMes.HasValue ? ultimoDiaMes.Value.ToString("dd/MM/yyyy") : string.Empty,
				MaestroProveedores = proveedores?.ToDropDownList(k => k.IdProveedor, d => d.RazonSocial, descriptionIncludesKey: false) ?? default,
				MaestroSaldosEnCero = saldosEnCero.ToDropDownList(k => k.Id, v => v.Descripcion, descriptionIncludesKey: false)
			};

			return ActivateMenuItem(model: model);
		}

		[HttpPost]
		[Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
		public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
		{
			var results = await _resumenCtaCteProBusiness.GetAll(request);
			return Json(results);
		}

		[HttpPost]
		[Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
		public async Task<JsonResult> GetCtasPagar(int idProveedor, DateTime? fechaDesde, DateTime? fechaHasta)
		{
			var results = await _resumenCtaCteProBusiness.GetCtasPagar(idProveedor, fechaDesde, fechaHasta);
			return Json(() => results);
		}

		[HttpPost]
		[Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
		public async Task<JsonResult> GetCtaCteDetalle(int idProveedor, DateTime? fechaDesde, DateTime? fechaHasta)
		{
			var results = await _resumenCtaCteProBusiness.GetCtaCteDetalle(idProveedor, fechaDesde, fechaHasta);
			return Json(() => results);
		}

		[HttpPost]
		[Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
		public async Task<JsonResult> DetalleGeneralCtasPagar(DateTime? fechaDesde, DateTime? fechaHasta)
		{
			var results = await _resumenCtaCteProBusiness.DetalleGeneralCtasPagar(fechaDesde, fechaHasta);
			return Json(() => results);
		}
	}
}
