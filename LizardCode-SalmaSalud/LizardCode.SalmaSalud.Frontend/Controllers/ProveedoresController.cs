using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Proveedores;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class ProveedoresController : BusinessController
    {
        private readonly IProveedoresBusiness _proveedoresBusiness;

        public ProveedoresController(IProveedoresBusiness proveedoresBusiness)
        {
            _proveedoresBusiness = proveedoresBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<ActionResult> Index()
        {
            var tiposIVA = Utilities.EnumToDictionary<TipoIVA>();
            var tiposTelefonos = Utilities.EnumToDictionary<TipoTelefono>();
            var empresas = (await _lookupsBusiness.GetEmpresasByIdUsuarioLookup(_permisosBusiness.User.Id)).ToList();
            var retenciones = await _lookupsBusiness.GetAllCodigosRetencion();

            var model = new ProveedorViewModel
            {
                MaestroRetencionGanancias = new SelectList(retenciones.Where(r => r.IdTipoRetencion == (int)TipoRetencion.Ganancias).ToList(), "IdCodigoRetencion", "Descripcion"),

                MaestroRetencionIVA = new SelectList(retenciones.Where(r => r.IdTipoRetencion == (int)TipoRetencion.IVA).ToList(), "IdCodigoRetencion", "Descripcion"),

                MaestroRetencionIBr = new SelectList(retenciones.Where(r => r.IdTipoRetencion == (int)TipoRetencion.IngresosBrutos).ToList(), "IdCodigoRetencion", "Descripcion"),

                MaestroRetencionSUSS = new SelectList(retenciones.Where(r => r.IdTipoRetencion == (int)TipoRetencion.SUSS).ToList(), "IdCodigoRetencion", "Descripcion"),

                MaestroRetencionGananciasMonotributo = new SelectList(retenciones.Where(r => r.IdTipoRetencion == (int)TipoRetencion.GananciasMonotributo).ToList(), "IdCodigoRetencion", "Descripcion"),

                MaestroRetencionIVAMonotributo = new SelectList(retenciones.Where(r => r.IdTipoRetencion == (int)TipoRetencion.IVAMonotributo).ToList(), "IdCodigoRetencion", "Descripcion"),

                MaestroTipoIVA = tiposIVA
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroTipoTelefono = tiposTelefonos
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroEmpresas = empresas
                    .ToDropDownList(k => k.IdEmpresa, t => t.RazonSocial, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _proveedoresBusiness.GetAll(request, false);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Nuevo(ProveedorViewModel model)
        {
            await _proveedoresBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Obtener(int id)
        {
            var proveedor = await _proveedoresBusiness.Get(id);
            return Json(() => proveedor);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Modificar(ProveedorViewModel model)
        {
            await _proveedoresBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _proveedoresBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> ValidarNroCUIT(string cuit, int? idProveedor)
        {
            var results = await _proveedoresBusiness.ValidarNroCUIT(cuit, idProveedor);
            return Json(results.IsNull() ? true : results);
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        [HttpPost]
        public async Task<ActionResult> Padron(string cuit)
        {
            var contribuyente = await _proveedoresBusiness.GetPadron(cuit);
            return Json(() => contribuyente);
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> ObtenerTotalesDashboard()
        {
            var proveedores = await _lookupsBusiness.GetAllProveedoresByIdEmpresaLookup(_permisosBusiness.User.IdEmpresa);
            return Json(() => new { cantidad = proveedores.Count });
        }
    }
}
