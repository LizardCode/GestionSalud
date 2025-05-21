using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Empresas;
using LizardCode.SalmaSalud.Domain.Enums;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LizardCode.Framework.Application.Common.Enums;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Controllers
{
    public class EmpresasController : BusinessController
    {
        private readonly IEmpresasBusiness _empresasBusiness;

        public EmpresasController(IEmpresasBusiness empresasBusiness)
        {
            _empresasBusiness = empresasBusiness;
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Index()
        {
            var tiposIVA = Utilities.EnumToDictionary<TipoIVA>();
            var tiposTelefonos = Utilities.EnumToDictionary<TipoTelefono>();
            var empresas = await _lookupsBusiness.GetAllEmpresasLookup();

            var model = new EmpresaViewModel
            {
                MaestroTipoIVA = tiposIVA
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroTipoTelefono = tiposTelefonos
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroEmpresas = empresas?.ToDropDownList(k => k.IdEmpresa, d => d.RazonSocial, descriptionIncludesKey: false) ?? default
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _empresasBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Nuevo(EmpresaViewModel model)
        {
            await _empresasBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Obtener(int id)
        {
            var empresa = await _empresasBusiness.Get(id);

            if (string.IsNullOrEmpty(empresa.TurnosHoraInicio))
            {
                //Horarios default
                var sHoraInicio = "Turnos:HoraInicio".FromAppSettings<string>(notFoundException: true);
                var sHoraIFin = "Turnos:HoraFin".FromAppSettings<string>(notFoundException: true);
                var sIntervaloTrunos = "Turnos:Intervalo".FromAppSettings<string>(notFoundException: true);

                empresa.TurnosHoraInicio = sHoraInicio.Split(":")[0];
                empresa.TurnosMinutosInicio = sHoraInicio.Split(":")[1];
                empresa.TurnosHoraFin = sHoraIFin.Split(":")[0];
                empresa.TurnosMinutosFin = sHoraIFin.Split(":")[1];
                empresa.TurnosIntervalo = sIntervaloTrunos;
                //Horarios default
            }

            return Json(() => empresa);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Modificar(EmpresaViewModel model)
        {
            await _empresasBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _empresasBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> DescargarCSR(int id)
        {
            var empresa = await _empresasBusiness.GetEmpresaById(id);
            return File(new MemoryStream(Encoding.ASCII.GetBytes(empresa.CSR)), "application/text", string.Concat(empresa.NombreFantasia, ".", "CSR"));
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<ActionResult> UploadCRT(int idEmpresa, string crt)
        {
            await _empresasBusiness.UploadCertificate(idEmpresa, crt);
            return Json(() => true);
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> ObtenerTotalesDashboard()
        {
            var empresas = await _empresasBusiness.GetAllByIdUsuario(_permisosBusiness.User.Id);
            return Json(() => new { cantidad = empresas.Count });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<ActionResult> CopiarPlanCtas(int idEmpresaDestino, int idEmpresaOrigen)
        {
            await _empresasBusiness.CopiarPlanCtas(idEmpresaDestino, idEmpresaOrigen);
            return Json(() => true);
        }
    }
}
