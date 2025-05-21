using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Bancos;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class BancosController : BusinessController
    {
        private readonly IBancosBusiness _bancosBusiness;

        public BancosController(IBancosBusiness bancosBusiness)
        {
            _bancosBusiness = bancosBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<ActionResult> IndexAsync()
        {
            var monedas = (await _lookupsBusiness.GetAllMonedas()).AsList();
            var proveedores = (await _lookupsBusiness.GetAllProveedoresByIdEmpresaLookup(_permisosBusiness.User.IdEmpresa)).AsList();
            var cuentasContables = (await _lookupsBusiness.GetCuentasContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Where(cc => cc.IdCodigoObservacion == (int)CodigoObservacion.BANCOS)
                .ToList();

            var model = new BancosViewModel
            {
                MaestroProveedores = proveedores
                    .ToDropDownList(k => k.IdProveedor, d => d.RazonSocial, descriptionIncludesKey: false),
                MaestroCuentasContables = cuentasContables
                    .ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroMonedas = monedas
                    .ToDropDownList(k => k.IdMoneda, d => d.Descripcion, descriptionIncludesKey: false),
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _bancosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Nuevo(BancosViewModel model)
        {
            await _bancosBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Obtener(int id)
        {
            var bancos = await _bancosBusiness.Get(id);
            return Json(() => bancos);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Modificar(BancosViewModel model)
        {
            await _bancosBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _bancosBusiness.Remove(id);
            return Json(() => true);
        }
    }
}
