using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.BalanceSumSdo;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class BalanceSumSdoController : BusinessController
    {
        private readonly IBalanceSumSdoBusiness _balanceSumSdoBusiness;

        public BalanceSumSdoController(IBalanceSumSdoBusiness balanceSumSdoBusiness)
        {
            _balanceSumSdoBusiness = balanceSumSdoBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var primerDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);
            var cuentas = (await _lookupsBusiness.GetCuentasContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList();
            var ejercicios = (await _lookupsBusiness.GetEjerciciosByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    IdEjercicio = e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy")),
                    FechaFin = e.FechaFin
                })
                .ToList();

            var model = new BalanceSumSdosViewModel
            {
                IdEjercicio = ejercicios.Last().IdEjercicio,
                FechaDesde = primerDiaMes.ToString("dd/MM/yyyy"),
                FechaHasta = ultimoDiaMes.ToString("dd/MM/yyyy"),
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
                MaestroCuentas = cuentas.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _balanceSumSdoBusiness.GetAll(request);
            return Json(results);
        }
    }
}
