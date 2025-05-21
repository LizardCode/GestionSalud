using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.SubdiarioCobros;
using System;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class SubdiarioCobrosController : BusinessController
    {
        private readonly ISubdiarioCobrosBusiness _subdiarioCobrosBusiness;
        private readonly IMemoryCache _memoryCache;

        public SubdiarioCobrosController(ISubdiarioCobrosBusiness subdiarioCobrosBusiness, IMemoryCache memoryCache)
        {
			_subdiarioCobrosBusiness = subdiarioCobrosBusiness;
            _memoryCache = memoryCache;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<ActionResult> Index()
        {
            var primerDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);

            var model = new SubdiarioCobrosViewModel
            {
                FechaDesde = primerDiaMes.ToString("dd/MM/yyyy"),
                FechaHasta = ultimoDiaMes.ToString("dd/MM/yyyy")
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _subdiarioCobrosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> GetDetalle(int idRecibo, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var results = await _subdiarioCobrosBusiness.GetDetalle(idRecibo, fechaDesde, fechaHasta);
            return Json(() => results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<IActionResult> GenerarExcel(int idRecibo, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                var excel = await _subdiarioCobrosBusiness.GetExcel(idRecibo, fechaDesde, fechaHasta);
                if (excel != null)
                {
                    var fileGUID = Guid.NewGuid().ToString("N");
                    var fileName = $"SubdiarioCobros.xlsx";
                    _memoryCache.Set(fileGUID, excel, TimeSpan.FromMinutes(5));

                    return Json(new { success = true, message = fileGUID, fileName });
                }
                else
                {
                    return Json(new { success = false, message = "No se encontraron registros para exportar." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> DescargarExcel(string fileGUID, string fileName)
        {
            var data = fileGUID.FromCache<byte[]>();

            if (data != null)
                return File(data, "application/vnd.ms-excel", fileName);

            return new EmptyResult();
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<ActionResult> DetalleImputaciones(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var results = await _subdiarioCobrosBusiness.DetalleImputaciones(fechaDesde, fechaHasta);
            return Json(results);
        }
    }
}
