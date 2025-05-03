using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.ListadoRetenciones;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class ListadoRetencionesController : BusinessController
    {
        private readonly IListadoRetencionesBusiness _listadoRetencionesBusiness;

        public ListadoRetencionesController(IListadoRetencionesBusiness listadoRetencionesBusiness)
        {
            _listadoRetencionesBusiness = listadoRetencionesBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<ActionResult> Index()
        {
            var primerDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);
            var listaRetenciones = Utilities.EnumToDictionary<Domain.Enums.ListaRetencionesPercepciones>();

            var model = new ListadoRetencionesViewModel
            {
                MaestroTipoRetecion = listaRetenciones.ToDropDownList(descriptionIncludesKey: false),
                FechaDesde = primerDiaMes.ToString("dd/MM/yyyy"),
                FechaHasta = ultimoDiaMes.ToString("dd/MM/yyyy")
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _listadoRetencionesBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR PAGAR")]
        public async Task<ActionResult> DescargarSicore(int IdTipoRetencion, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var sicore = await _listadoRetencionesBusiness.GetSicore(IdTipoRetencion, fechaDesde, fechaHasta);
            return File(new MemoryStream(Encoding.ASCII.GetBytes(sicore)), "application/text", $"SICORE_{fechaDesde.Value.ToString("yyyyMMdd")}_{fechaHasta.Value.ToString("yyyyMMdd")}.TXT");
        }

    }
}
