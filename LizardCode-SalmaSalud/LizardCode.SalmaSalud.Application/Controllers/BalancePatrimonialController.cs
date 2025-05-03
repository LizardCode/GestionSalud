using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.BalancePatrimonial;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class BalancePatrimonialController : BusinessController
    {
        private readonly IBalancePatrimonialBusiness _balancePatrimonialBusiness;

        public BalancePatrimonialController(IBalancePatrimonialBusiness balancePatrimonialBusiness)
        {
            _balancePatrimonialBusiness = balancePatrimonialBusiness;
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
                    e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy")),
                    e.FechaFin
                })
                .ToList();

            var model = new BalancePatrimonialViewModel
            { 
                IdEjercicio = ejercicios.Last().IdEjercicio,
                FechaHasta = ejercicios.Last().FechaFin.ToString("dd/MM/yyyy"),
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _balancePatrimonialBusiness.GetAll(request);
            return Json(results);
        }
    }
}
