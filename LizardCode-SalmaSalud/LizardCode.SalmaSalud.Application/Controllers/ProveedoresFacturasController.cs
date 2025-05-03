using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.ProveedoresFacturas;
using System;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class ProveedoresFacturasController : BusinessController
    {
        private readonly ICargaManualBusiness _cargaManualBusiness;

        public ProveedoresFacturasController(ICargaManualBusiness cargaManualBusiness)
        {
            _cargaManualBusiness = cargaManualBusiness;

        }

        [Authorize(Roles = "PROVEEDOR")]
        public async Task<ActionResult> Index()
        {
            var primerDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);

            var model = new ProveedoresFacturasViewModel
            {
                FechaDesde = primerDiaMes.ToString("dd/MM/yyyy"),
                FechaHasta = ultimoDiaMes.ToString("dd/MM/yyyy")
            };

            return ActivateMenuItem(model: model);
        }

    }
}
