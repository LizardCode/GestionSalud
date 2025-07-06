using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Pacientes;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Business;
using NPOI.OpenXmlFormats.Spreadsheet;

namespace LizardCode.SalmaSalud.Controllers
{
    public class PacientesController : BusinessController
    {
        private readonly IPacientesBusiness _pacientesBusiness;

        public PacientesController(IPacientesBusiness pacientesBusiness)
        {
            _pacientesBusiness = pacientesBusiness;
        }


        [Authorize(Roles = "ADMIN, RECEPCION, PROFESIONAL")]
        public async Task<ActionResult> Index()
        {
            var tiposTelefonos = Utilities.EnumToDictionary<TipoTelefono>();
            var financiadores = (await _lookupsBusiness.GetAllFinanciadores()).ToList();

            var model = new PacienteViewModel
            {
                MaestroTipoTelefono = tiposTelefonos
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroFinanciadores = financiadores
                    .ToDropDownList(k => k.IdFinanciador, t => t.Nombre, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, PROFESIONAL, PROFESIONAL_EXTERNO")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _pacientesBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Nuevo(PacienteViewModel model)
        {
            await _pacientesBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var usuario = await _pacientesBusiness.Get(id);
            return Json(() => usuario);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Modificar(PacienteViewModel model)
        {
            await _pacientesBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _pacientesBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> ValidarNroCUIT(string cuit, int? idPaciente)
        {
            var results = await _pacientesBusiness.ValidarNroCUIT(cuit, idPaciente);
            return Json(results.IsNull() ? true : results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> ValidarNroDocumento(string documento, int? idPaciente)
        {
            var results = await _pacientesBusiness.ValidarNroDocumento(documento, idPaciente);
            return Json(results.IsNull() ? true : results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> ValidarFinanciadorNro(string financiadorNro, int? idPaciente)
        {
            if (string.IsNullOrEmpty(financiadorNro))
                return Json(true);

            var results = await _pacientesBusiness.ValidarNroFinanciador(financiadorNro, idPaciente);
            return Json(results.IsNull() ? true : results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, PROFESIONAL, PROFESIONAL_EXTERNO")]
        public async Task<JsonResult> ObtenerPorDocumento(string documento)
        {
            var paciente = await _pacientesBusiness.GetLikeDocument(documento);
            return Json(() => paciente);
        }

        //[Authorize(Roles = "ADMIN")]
        //public async Task<ActionResult> ObtenerTotalesDashboard()
        //{
        //    var Pacientes = await _lookupsBusiness.GetAllPacientesByIdEmpresaLookup(_permisosBusiness.User.IdEmpresa);
        //    return Json(() => new { cantidad = Pacientes.Count });
        //}

        [Authorize(Roles = "ADMIN, RECEPCION, PROFESIONAL")]
        public async Task<JsonResult> ObtenerPorTelefono(int idPaciente, string telefono)
        {
            var paciente = await _pacientesBusiness.GetLikePhone(telefono);
            if (paciente != null && paciente.IdPaciente != idPaciente)
                return Json(() => paciente);
            else
                return Json(() => null);
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL, PROFESIONAL_EXTERNO")]
        public async Task<IActionResult> HistoriaClinicaView(int id, bool showResumenPaciente)
        {
            var model = new HistoriaClinicaViewModel
            {
                ShowResumenPaciente = showResumenPaciente,
                IdPaciente = id                
            };

            return View("HistoriaClinica", model);
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL, RECEPCION, PROFESIONAL_EXTERNO")]
        public async Task<IActionResult> ResumenView(int id, bool showNombre = true, bool showButton = true, bool forzarParticular = false)
        {
            var paciente = await _pacientesBusiness.GetCustomById(id);
            if (paciente == null)
                throw new BusinessException("Paciente no encontrado.");

            var model = new ResumenViewModel
            {
                IdPaciente = paciente.IdPaciente,
                Paciente = paciente.Nombre,
                PacienteUltimaAtencion = paciente.UltimaAtencion.HasValue ? paciente.UltimaAtencion.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                Financiador = paciente.Financiador,
                FinanciadorPlan = paciente.FinanciadorPlan,
                NroAfiliadoSocio = paciente.FinanciadorNro,
                ShowNombre = showNombre,
                ShowButton = showButton,
                ForzarParticular = forzarParticular
            };

            return View("_Resumen", model);
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL, RECEPCION, PROFESIONAL_EXTERNO")]
        public async Task<IActionResult> BusquedaView()
        {
            return View("Busqueda");
        }
    }
}
