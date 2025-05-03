using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Cheques;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class ChequesController : BusinessController
    {
        private readonly IChequesBusiness _chequesBusiness;

        public ChequesController(IChequesBusiness chequesBusiness)
        {
            _chequesBusiness = chequesBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<ActionResult> IndexAsync()
        {
            var bancos = (await _lookupsBusiness.GetBancosByIdEmpresa(_permisosBusiness.User.IdEmpresa)).ToList();
            var ejercicios = (await _lookupsBusiness.GetEjerciciosByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy"))
                })
                .ToList();
            var model = new ChequesViewModel
            {
                MaestroTipoCheque = Utilities.EnumToDictionary<TipoCheque>()
                    .ToDropDownList(descriptionIncludesKey: false),
                MaestroTipoChequePropios = Utilities.EnumToDictionary<TipoChequePropio>()
                    .ToDropDownList(descriptionIncludesKey: false),
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
                MaestroEstadoCheque = Utilities.EnumToDictionary<EstadoCheque>()
                    .ToDropDownList(descriptionIncludesKey: false),
                MaestroBancos = bancos.ToDropDownList(k => k.IdBanco, d => d.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _chequesBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Nuevo(ChequesViewModel model)
        {
            await _chequesBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Obtener(int id)
        {
            var cheque = await _chequesBusiness.Get(id);
            return Json(() => cheque);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _chequesBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> ValidarNumeroCheque(int idBanco, int idTipoCheque, string nroDesde, string nroHasta)
        {
            var valid = await _chequesBusiness.ValidarNumeroCheque(idBanco, idTipoCheque, nroDesde, nroHasta);
            if (!valid)
            {
                return await Task.FromResult(Json(false));
            }

            return await Task.FromResult(Json(true));
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> ObtenerChequesADebitar(int idBanco)
        {
            var cheques = await _chequesBusiness.GetChequesADebitar(idBanco);
            return Json(() => cheques);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Debitar(ChequesViewModel model, int idEjercicio, DateTime fecha, int idBancoDebitar)
        {
            await _chequesBusiness.Debitar(model.ChequesADebitar, idEjercicio, fecha, idBancoDebitar);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> ObtenerAsiento(int id)
        {
            var asiento = await _chequesBusiness.GetAsientoCustom(id);
            return Json(() => asiento);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ReverseById(int idCheque)
        {
            await _chequesBusiness.ReverseById(idCheque);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Detalle(int id)
        {
            var detalle = await _chequesBusiness.Detalle(id);
            return Json(() => detalle);
        }
    }
}
