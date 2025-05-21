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
using LizardCode.SalmaSalud.Application.Models.Laboratorios;

namespace LizardCode.SalmaSalud.Controllers
{
    public class LaboratoriosController : BusinessController
    {
        private readonly IProveedoresBusiness _proveedoresBusiness;

        public LaboratoriosController(IProveedoresBusiness proveedoresBusiness)
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

            var model = new LaboratorioViewModel
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
            var results = await _proveedoresBusiness.GetAll(request, true);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Nuevo(LaboratorioViewModel model)
        {
            await _proveedoresBusiness.NewLaboratorio(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Obtener(int id)
        {
            var proveedor = await _proveedoresBusiness.GetLaboratorio(id);
            return Json(() => proveedor);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Modificar(LaboratorioViewModel model)
        {
            await _proveedoresBusiness.UpdateLaboratorio(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _proveedoresBusiness.RemoveLaboratorio(id);
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

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA, PROFESIONAL")]
        public async Task<JsonResult> GetServiciosByLaboratorio(int idLaboratorio)
        {
            var servicios = await _proveedoresBusiness.GetServiciosByIdLaboratorio(idLaboratorio);
            return Json(() => servicios.Select(s => new { value = s.IdLaboratorioServicio, text = s.Descripcion}));
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA, PROFESIONAL")]
        public async Task<JsonResult> ObtenerServicio(int id)
        {
            var servicio = await _proveedoresBusiness.GetServicioById(id);
            return Json(() => servicio);
        }

    }
}
