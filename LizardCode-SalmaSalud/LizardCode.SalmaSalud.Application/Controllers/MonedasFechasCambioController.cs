using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.MonedasFechasCambio;
using System;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class MonedasFechasCambioController : BusinessController
    {
        private readonly IMonedasFechasCambioBusiness _monedasFechasCambioBusiness;

        public MonedasFechasCambioController(IMonedasFechasCambioBusiness monedasFechasCambioBusiness)
        {
            _monedasFechasCambioBusiness = monedasFechasCambioBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public ActionResult Index()
        {
            return ActivateMenuItem(model: new MonedasFechasCambioViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _monedasFechasCambioBusiness.GetAll(request);
            return Json(results);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var moneda = await _monedasFechasCambioBusiness.Get(id);
            return Json(() => moneda);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(MonedasFechasCambioViewModel model)
        {
            await _monedasFechasCambioBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetFechaCambio(int idMoneda, DateTime fecha)
        {
            var cotizacion = await _monedasFechasCambioBusiness.GetFechaCambio(idMoneda, fecha);
            return Json(() => cotizacion.HasValue ? cotizacion : 0);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerCotizaciones(int id)
        {
            var cotizaciones = await _monedasFechasCambioBusiness.GetCotizaciones(id);
            return Json(() => cotizaciones);
        }

    }
}
