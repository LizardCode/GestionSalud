using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.SubdiarioVentas;
using System;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class SubdiarioVentasController : BusinessController
    {
        private readonly ISubdiarioVentasBusiness _subdiarioVentasBusiness;
        private readonly IMemoryCache _memoryCache;

        public SubdiarioVentasController(ISubdiarioVentasBusiness subdiarioVentasBusiness, IMemoryCache memoryCache)
        {
            _subdiarioVentasBusiness = subdiarioVentasBusiness;
            _memoryCache = memoryCache;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<ActionResult> Index()
        {
            var primerDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);

            var model = new SubdiarioVentasViewModel
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
            var results = await _subdiarioVentasBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> GetDetalle(int idComprobante, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var results = await _subdiarioVentasBusiness.GetDetalle(idComprobante, fechaDesde, fechaHasta);
            return Json(() => results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<IActionResult> GenerarExcel(int idComprobante, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                var excel = await _subdiarioVentasBusiness.GetExcel(idComprobante, fechaDesde, fechaHasta);
                if (excel != null)
                {
                    var fileGUID = Guid.NewGuid().ToString("N");
                    var fileName = $"SubdiarioVentas.xlsx";
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
        public async Task<ActionResult> DetalleImputaciones(DateTime? fechaDesde, DateTime? fechaHasta, int? idCliente)
        {
            var results = await _subdiarioVentasBusiness.DetalleImputaciones(fechaDesde, fechaHasta, idCliente);
            return Json(results);
        }
    }
}
