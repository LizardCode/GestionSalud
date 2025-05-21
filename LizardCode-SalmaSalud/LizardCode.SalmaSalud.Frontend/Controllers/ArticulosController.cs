using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Articulos;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class ArticulosController : BusinessController
    {
        private readonly IArticulosBusiness _articulosBusiness;

        public object TipoAlicutas { get; private set; }

        public ArticulosController(IArticulosBusiness articulosBusiness)
        {
            _articulosBusiness = articulosBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var rubros = await _lookupsBusiness.GetAllRubrosArticulos(_permisosBusiness.User.IdEmpresa);
            var alicuotas = (await _lookupsBusiness.GetAllAlicuotas()).Where(a => a.IdTipoAlicuota == (int)TipoAlicuota.IVA).ToList();
            var cuentasVta = (await _lookupsBusiness.GetCuentasContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa)).Where(cc => cc.IdCodigoObservacion == (int)CodigoObservacion.VENTA_DE_ARTICULOS).ToList();
            var cuentasComp = (await _lookupsBusiness.GetCuentasContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa)).Where(cc => cc.IdCodigoObservacion == (int)CodigoObservacion.COMPRA_ARTICULOS).ToList();

            var model = new ArticulosViewModel
            {
                MaestroRubros = new SelectList(rubros, "IdRubroArticulo", "Descripcion"),
                MaestroCuentasContablesVentas = cuentasVta.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroCuentasContablesCompras = cuentasComp.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroAlicuotas = alicuotas.ToDropDownList(k => k.Valor, d => d.Valor, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _articulosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(ArticulosViewModel model)
        {
            await _articulosBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var articulo = await _articulosBusiness.Get(id);
            return Json(() => articulo);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(ArticulosViewModel model)
        {
            await _articulosBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _articulosBusiness.Remove(id);
            return Json(() => true);
        }
    }
}
