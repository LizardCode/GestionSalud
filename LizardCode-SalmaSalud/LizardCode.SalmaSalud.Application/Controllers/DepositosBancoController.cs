using Dapper;
using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.DepositosBanco;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class DepositosBancoController : BusinessController
    {
        private readonly IDepositosBancoBusiness _depositosBancoBusiness;
        private readonly IChequesBusiness _chequesBusiness;

        public DepositosBancoController(IDepositosBancoBusiness depositosBancoBusiness, IChequesBusiness chequesBusiness)
        {
            _depositosBancoBusiness = depositosBancoBusiness;
            _chequesBusiness = chequesBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<ActionResult> Index()
        {
            var tipoDeposito = Utilities.EnumToDictionary<TipoDeposito>();
            var monedas = (await _lookupsBusiness.GetAllMonedas()).AsList();
            var bancos = (await _lookupsBusiness.GetBancosByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList();
            var ejercicios = (await _lookupsBusiness.GetEjerciciosByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    IdEjercicio = e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy"))
                })
                .ToList();

            var model = new DepositosBancoViewModel
            {
                Items = new List<DepositosBancoDetalle>(),
                MaestroTipoDepositoBanco = tipoDeposito.ToDropDownList(descriptionIncludesKey: false),
                MaestroBancos = bancos.ToDropDownList(k => k.IdBanco, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
                MaestroMonedas = monedas.ToDropDownList(k => k.Codigo, d => d.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _depositosBancoBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> Nuevo(DepositosBancoViewModel model)
        {
            await _depositosBancoBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> Obtener(int id)
        {
            var recibo = await _depositosBancoBusiness.Get(id);
            return Json(() => recibo);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> Modificar(DepositosBancoViewModel model)
        {
            await _depositosBancoBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _depositosBancoBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> GetChequesCartera(string q)
        {
            var cheques = await _chequesBusiness.GetChequesCartera(q);
            return Json(() => cheques);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> GetPrimerChequeDisponible(int idBanco, int idTipoCheque)
        {
            var cheque = await _chequesBusiness.GetPrimerChequeDisponible(idBanco, idTipoCheque);
            return Json(() => cheque);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> ValidarNumeroChequeDisponible(int idBanco, int idTipoCheque, string nroCheque)
        {
            var valid = await _chequesBusiness.ValidarNumeroChequeDisponible(idBanco, idTipoCheque, nroCheque);
            return Json(() => valid);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> ObtenerAsiento(int id)
        {
            var asiento = await _depositosBancoBusiness.GetAsientoCustom(id);
            return Json(() => asiento);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> GetFechaCambio(string idMoneda1, DateTime fecha)
        {
            var cotizacion = await _lookupsBusiness.GetFechaCambio(idMoneda1, fecha, _permisosBusiness.User.IdEmpresa);
            return Json(() => cotizacion);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA")]
        public async Task<JsonResult> VerificarChequeDisponible(int idBanco, int idTipoCheque, string nroCheque)
        {
            var chequeDisponible = await _depositosBancoBusiness.VerificarChequeDisponible(idBanco, idTipoCheque, nroCheque);
            return Json(() => chequeDisponible);
        }

    }
}
