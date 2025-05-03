using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.TiposAsientos;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class TiposAsientosController : BusinessController
    {
        private readonly ITiposAsientosBusiness _tiposAsientosBusiness;

        public TiposAsientosController(ITiposAsientosBusiness tiposAsientosBusiness)
        {
            _tiposAsientosBusiness = tiposAsientosBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {

            var cuentasContables = (await _lookupsBusiness.GetCuentasContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList();

            var model = new TiposAsientosViewModel
            {
                MaestroCuentasContables = cuentasContables.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _tiposAsientosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(TiposAsientosViewModel model)
        {
            await _tiposAsientosBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            //var tipoAsiento = await _tiposAsientosBusiness.Get(id);
            var tipoAsiento = await _tiposAsientosBusiness.GetCustom(id);
            return Json(() => tipoAsiento);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(TiposAsientosViewModel model)
        {
            await _tiposAsientosBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _tiposAsientosBusiness.Remove(id);
            return Json(() => true);
        }

    }
}
