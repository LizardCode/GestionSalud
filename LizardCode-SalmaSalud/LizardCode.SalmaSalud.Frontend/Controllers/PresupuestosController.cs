using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using LizardCode.SalmaSalud.Application.Business;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Presupuestos;
using LizardCode.SalmaSalud.Application.Models.Turnos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class PresupuestosController : BusinessController
    {
        private readonly IPresupuestosBusiness _presupuestosBusiness;
        private readonly IFinanciadoresBusiness _financiadoresBusiness;
        private readonly IPacientesBusiness _pacientesBusiness;

        public PresupuestosController(IPresupuestosBusiness presupuestosBusiness, IFinanciadoresBusiness financiadoresBusiness, IPacientesBusiness pacientesBusiness)
        {
            _presupuestosBusiness = presupuestosBusiness;
            _financiadoresBusiness = financiadoresBusiness;
            _pacientesBusiness = pacientesBusiness;
        }

        [Authorize(Roles = "ADMIN, RECEPCION, PROFESIONAL")]
        public async Task<ActionResult> Index()
        {
            var pacientes = await _lookupsBusiness.GetAllPacientes();
            var pacientesFormatted = pacientes.Select(s => new { IdPaciente = s.IdPaciente, Paciente = string.Format("{0} - {1}", s.Documento, s.Nombre) });

            var prestacionesFinanciador = new List<Domain.Entities.FinanciadorPrestacion>(); //await _financiadoresBusiness.GetAllPrestacionesByFinanciadorId(paciente.IdFinanciador ?? 0);
            var otrasPrestaciones = await _lookupsBusiness.GetAllPrestaciones();

            var model = new PresupuestoViewModel {

                MaestroPacientes = pacientesFormatted
                .ToDropDownList(k => k.IdPaciente, t => t.Paciente, descriptionIncludesKey: false),

                MaestroPrestaciones = prestacionesFinanciador
                .ToDropDownList(k => k.IdFinanciadorPrestacion, t => t.Descripcion, descriptionIncludesKey: false),

                MaestroOtrasPrestaciones = otrasPrestaciones
                .ToDropDownList(k => k.IdPrestacion, t => t.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, PROFESIONAL")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _presupuestosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, PROFESIONAL")]
        public async Task<JsonResult> Nuevo(PresupuestoViewModel model)
        {
            await _presupuestosBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, RECEPCION, PROFESIONAL")]
        public async Task<JsonResult> Obtener(int id)
        {
            var presupuesto = await _presupuestosBusiness.Get(id);
            return Json(() => presupuesto);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, PROFESIONAL")]
        public async Task<JsonResult> Modificar(PresupuestoViewModel model)
        {
            await _presupuestosBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, PROFESIONAL")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _presupuestosBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, PROFESIONAL")]
        public async Task<JsonResult> Rechazar(int idPresupuesto)
        {
            await _presupuestosBusiness.Rechazar(idPresupuesto);
            return Json(true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, PROFESIONAL")]
        public async Task<JsonResult> Aprobar(int idPresupuesto)
        {
            await _presupuestosBusiness.Aprobar(idPresupuesto);
            return Json(true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, PROFESIONAL")]
        public async Task<JsonResult> Cerrar(int idPresupuesto)
        {
            await _presupuestosBusiness.Cerrar(idPresupuesto);
            return Json(true);
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL")]
        public async Task<ActionResult> PresupuestosAprobadosView(int idPaciente)
        {
            var presupuestos = await _presupuestosBusiness.GetPresupuestosAprobados(idPaciente);

            var model = new PresupuestoAprobadoViewModel
            {
                Presupuestos = presupuestos.Select(s => new PresupuestoAprobadoDetalleViewModel { 
                    Fecha = s.Fecha,
                    IdPresupuesto = s.IdPresupuesto,
                    Total = s.Total,
                    TotalCoPagos = s.TotalCoPagos,
                    TotalPrestaciones = s.TotalPrestaciones
                }).ToList()
            };

            return View("PresupuestosAprobados", model);
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL")]
        public async Task<ActionResult> PresupuestosAprobadosDetalleView(int idPresupuesto)
        {
            var presupuesto = await _presupuestosBusiness.Get(idPresupuesto);

            var model = new PresupuestosAprobadosDetalleViewModel
            {
                Prestaciones = presupuesto.Prestaciones,
                OtrasPrestaciones = presupuesto.OtrasPrestaciones
            };

            return View("PresupuestosAprobadosDetalle", model);
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL")]
        public async Task<JsonResult> GetAllPrestacionesByPresupuestoIds(List<int> ids)
        {
            var prestaciones = await _presupuestosBusiness.GetAllPrestacionesByPresupuestoIds(ids);

            return Json(() => prestaciones);
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL")]
        public async Task<JsonResult> GetAllOtrasPrestacionesByPresupuestoIds(List<int> ids)
        {
            var prestaciones = await _presupuestosBusiness.GetAllOtrasPrestacionesByPresupuestoIds(ids);

            return Json(() => prestaciones);
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL, RECEPCION")]
        public async Task<JsonResult> GetPrestacionesByFinanciadorPaciente(int idPaciente)
        {
            var paciente = await _pacientesBusiness.Get(idPaciente);
            var prestacionesFinanciador = await _financiadoresBusiness.GetAllPrestacionesByIdFinanciadorAndIdPlan(paciente?.IdFinanciador ?? 0, paciente.IdFinanciadorPlan ?? 0);

            return Json(() => prestacionesFinanciador.ToDropDownList(k => k.IdFinanciadorPrestacion, t => t.Descripcion, descriptionIncludesKey: false));
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL, RECEPCION")]
        public async Task<JsonResult> GetPresupuestosAprobadosDisponibles()
        {
            var presupuestos = await _presupuestosBusiness.GetPresupuestosAprobadosDisponibles();
            var presupuestosFormatted = presupuestos.Select(s => new { IdPresupuesto = s.IdPresupuesto, Presupuesto = string.Format("Id.: {0} - {1} - {2}", s.IdPresupuesto, s.PacienteDocumento, s.Paciente) });
            
            return Json(() => presupuestosFormatted.Select(s => new { value = s.IdPresupuesto, text = s.Presupuesto }));
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL, RECEPCION")]
        public async Task<JsonResult> ObtenerPedidos(int id)
        {
            var pedidos = await _presupuestosBusiness.GetPedidosPorPresupuesto(id);            

            return Json(() => pedidos);
        }
    }
}
