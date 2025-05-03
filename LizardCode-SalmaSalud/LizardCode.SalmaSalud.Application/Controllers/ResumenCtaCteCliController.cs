using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.ResumenCtaCteCli;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class ResumenCtaCteCliController : BusinessController
    {
		private readonly IResumenCtaCteCliBusiness _resumenCtaCteCliBusiness;

		public ResumenCtaCteCliController(IResumenCtaCteCliBusiness resumenCtaCteCliBusiness)
		{
			_resumenCtaCteCliBusiness = resumenCtaCteCliBusiness;
		}

		[Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
		public async Task<ActionResult> Index()
		{
			var primerDiaMes = await _lookupsBusiness.GetMinFechaComprobanteVenta(_permisosBusiness.User.IdEmpresa);
			var ultimoDiaMes = await _lookupsBusiness.GetMaxFechaComprobanteVenta(_permisosBusiness.User.IdEmpresa);

			var primerDiaMesRec = await _lookupsBusiness.GetMinFechaRecibo(_permisosBusiness.User.IdEmpresa);
			var ultimoDiaMesRec = await _lookupsBusiness.GetMaxFechaRecibo(_permisosBusiness.User.IdEmpresa);

			if (primerDiaMes > primerDiaMesRec)
				primerDiaMes = primerDiaMesRec;

			if (ultimoDiaMes < ultimoDiaMesRec)
				ultimoDiaMes = ultimoDiaMesRec;

			var clientes = (await _lookupsBusiness.GetAllClientesLookup()).AsList();
			var saldosEnCero = new[]
			{
				new { Id = 1, Descripcion = "Si" },
				new { Id = 0, Descripcion = "No" }
			}
			.ToList();

			var model = new ResumenCtaCteCliViewModel
			{
				FechaDesde = primerDiaMes.HasValue ? primerDiaMes.Value.ToString("dd/MM/yyyy") : string.Empty,
				FechaHasta = ultimoDiaMes.HasValue ? ultimoDiaMes.Value.ToString("dd/MM/yyyy") : string.Empty,
				MaestroClientes = clientes?.ToDropDownList(k => k.IdCliente, d => d.RazonSocial, descriptionIncludesKey: false) ?? default,
				MaestroSaldosEnCero = saldosEnCero.ToDropDownList(k => k.Id, v => v.Descripcion, descriptionIncludesKey: false)
			};

			return ActivateMenuItem(model: model);
		}

		[HttpPost]
		[Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
		public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
		{
			var results = await _resumenCtaCteCliBusiness.GetAll(request);
			return Json(results);
		}

		[HttpPost]
		[Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
		public async Task<JsonResult> GetCtasCobrar(int idCliente, DateTime? fechaDesde, DateTime? fechaHasta)
		{
			var results = await _resumenCtaCteCliBusiness.GetCtasCobrar(idCliente, fechaDesde, fechaHasta);
			return Json(() => results);
		}

		[HttpPost]
		[Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
		public async Task<JsonResult> GetCtaCteDetalle(int idCliente, DateTime? fechaDesde, DateTime? fechaHasta)
		{
			var results = await _resumenCtaCteCliBusiness.GetCtaCteDetalle(idCliente, fechaDesde, fechaHasta);
			return Json(() => results);
		}

		[HttpPost]
		[Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
		public async Task<JsonResult> DetalleGeneralCtasCobrar(DateTime? fechaDesde, DateTime? fechaHasta)
		{
			var results = await _resumenCtaCteCliBusiness.DetalleGeneralCtasCobrar(fechaDesde, fechaHasta);
			return Json(() => results);
		}

		[Authorize(Roles = "ADMIN, ADMINISTRACION")]
		public async Task<ActionResult> GetResumenCtaCteCliDashboard()
		{
			var importe = await _resumenCtaCteCliBusiness.GetResumenCtaCteCliDashboard();
			return Json(() => new { importe = importe });
		}
	}
}
