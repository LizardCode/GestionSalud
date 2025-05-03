using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.CuentasContables;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class CuentasContablesController : BusinessController
    {
        private readonly ICuentasContablesBusiness _cuentasContablesBusiness;

        public CuentasContablesController(ICuentasContablesBusiness cuentasContablesBusiness)
        {
            _cuentasContablesBusiness = cuentasContablesBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var codigosObservacion = Utilities.EnumToDictionary<CodigoObservacion>();
            var rubrosContables = (await _lookupsBusiness.GetRubrosContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(rc =>
                new
                {
                    rc.IdRubroContable,
                    rc.CodigoRubro,
                    Descripcion = $"{rc.CodigoRubro} - {rc.Descripcion}"
                })
                .ToList();

            var model = new CuentasContablesViewModel
            {
                MaestroCodigosObservacion = codigosObservacion.ToDropDownList(descriptionIncludesKey: false),
                MaestroRubrosContables = rubrosContables
                    .ToDropDownList(r => r.IdRubroContable, r => r.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _cuentasContablesBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(CuentasContablesViewModel model)
        {
            await _cuentasContablesBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var contacto = await _cuentasContablesBusiness.Get(id);
            return Json(() => contacto);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(CuentasContablesViewModel model)
        {
            await _cuentasContablesBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _cuentasContablesBusiness.Remove(id);
            return Json(() => true);
        }

    }
}
