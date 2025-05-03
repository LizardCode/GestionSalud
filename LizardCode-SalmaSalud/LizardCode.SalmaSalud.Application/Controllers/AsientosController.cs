using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Asientos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class AsientosController : BusinessController
    {
        private readonly IAsientosBusiness _asientosBusiness;

        public AsientosController(IAsientosBusiness asientosBusiness)
        {
            _asientosBusiness = asientosBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var ejercicios = (await _lookupsBusiness.GetEjerciciosByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy"))
                })
                .ToList();

            var tiposAsiento = (await _lookupsBusiness.GetTiposAsientoByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList();
            var cuentasContables = (await _lookupsBusiness.GetCuentasContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList();

            var model = new AsientosViewModel
            {
                Fecha = DateTime.Now,
                Items = new List<AsientosDetalle>(),
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.First().IdEjercicio),
                MaestroTipoAsientos = tiposAsiento.ToDropDownList(k => k.IdTipoAsiento, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroCuentasContables = cuentasContables.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _asientosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(AsientosViewModel model)
        {
            await _asientosBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var asiento = await _asientosBusiness.GetCustom(id);
            return Json(() => asiento);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(AsientosViewModel model)
        {
            await _asientosBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _asientosBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> GetCuentasByTipoAsiento(int idTipoAsiento)
        {
            var cuentas = await _asientosBusiness.GetCuentasByTipoAsiento(idTipoAsiento);
            return Json(() => cuentas);
        }

    }
}
