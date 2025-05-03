using Dapper;
using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.SaldoInicioBanco;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class SaldoInicioBancoController : BusinessController
    {
        private readonly ISaldoInicioBancoBusiness _saldoInicioBancoBusiness;

        public SaldoInicioBancoController(ISaldoInicioBancoBusiness saldoInicioBancoBusiness)
        {
            _saldoInicioBancoBusiness = saldoInicioBancoBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<ActionResult> Index()
        {
            var clientes = (await _lookupsBusiness.GetAllClientesLookup()).AsList();
            var proveedores = (await _lookupsBusiness.GetAllProveedoresByIdEmpresaLookup(_permisosBusiness.User.IdEmpresa)).AsList();
            var bancos = (await _lookupsBusiness.GetBancosByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList();
            var monedas = (await _lookupsBusiness.GetAllMonedas()).AsList();
            var tipoCheques = Utilities.EnumToDictionary<TipoCheque>();

            var model = new SaldoInicioBancoViewModel
            {
                Cheques = new List<SaldoInicioBancoCheques>(),
                AnticiposClientes = new List<SaldoInicioBancoAnticiposClientes>(),
                AnticiposProveedores = new List<SaldoInicioBancoAnticiposProveedores>(),
                MaestroTipoCheques = tipoCheques.ToDropDownList(descriptionIncludesKey: false),
                MaestroProveedores = proveedores.ToDropDownList(k => k.IdProveedor, d => d.RazonSocial, descriptionIncludesKey: false),
                MaestroClientes = clientes.ToDropDownList(k => k.IdCliente, d => d.RazonSocial, descriptionIncludesKey: false),
                MaestroBancos = bancos.ToDropDownList(k => k.IdBanco, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroMonedas = monedas.ToDropDownList(k => k.Codigo, d => d.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _saldoInicioBancoBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Nuevo(SaldoInicioBancoViewModel model)
        {
            await _saldoInicioBancoBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Obtener(int id)
        {
            var ordenPago = await _saldoInicioBancoBusiness.Get(id);
            return Json(() => ordenPago);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _saldoInicioBancoBusiness.Remove(id);
            return Json(() => true);
        }
    }
}
