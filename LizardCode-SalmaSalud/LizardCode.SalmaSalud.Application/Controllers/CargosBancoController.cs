using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.CargosBanco;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class CargosBancoController : BusinessController
    {
        private readonly ICargosBancoBusiness _cargosBancoBusiness;

        public CargosBancoController(ICargosBancoBusiness cargosBancoBusiness)
        {
            _cargosBancoBusiness = cargosBancoBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<ActionResult> Index()
        {
            var ejercicios = (await _lookupsBusiness.GetEjerciciosByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy"))
                })
                .ToList();

            var cuentasContables = (await _lookupsBusiness.GetCuentasContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Where(cc => cc.IdCodigoObservacion == (int)Domain.Enums.CodigoObservacion.GASTOS_BANCARIOS)
                .AsList();
            var bancos = (await _lookupsBusiness.GetBancosByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList();
            var alicuotas = (await _lookupsBusiness.GetAllAlicuotas()).ToList().Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA).ToList();

            var model = new CargosBancoViewModel
            {
                Fecha = DateTime.Now,
                Items = new List<CargosBancoItems>(),
                MaestroBancos = bancos.ToDropDownList(k => k.IdBanco, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
                MaestroCuentasContables = cuentasContables.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroAlicuotas = alicuotas.ToDropDownList(k => k.Valor, v => v.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _cargosBancoBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> Nuevo(CargosBancoViewModel model)
        {
            await _cargosBancoBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> Obtener(int id)
        {
            var asiento = await _cargosBancoBusiness.GetCustom(id);
            return Json(() => asiento);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> Modificar(CargosBancoViewModel model)
        {
            await _cargosBancoBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _cargosBancoBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> ObtenerAsiento(int id)
        {
            var asiento = await _cargosBancoBusiness.GetAsientoCustom(id);
            return Json(() => asiento);
        }

        [HttpPost]
        public async Task<bool> ValidacionRemota(string detalle, decimal importe, int idCuentaContable)
        {
            return await Task.FromResult(false);
        }
    }
}
