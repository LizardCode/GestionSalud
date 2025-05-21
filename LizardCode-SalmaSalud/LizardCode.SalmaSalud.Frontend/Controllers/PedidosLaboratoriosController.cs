using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.PedidosLaboratorios;
using LizardCode.SalmaSalud.Application.Models.Presupuestos;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class PedidosLaboratoriosController : BusinessController
    {
        private readonly IPedidosLaboratoriosBusiness _pedidosLaboratoriosBusiness;
        private readonly IPresupuestosBusiness _presupuestosBusiness;

        public PedidosLaboratoriosController(IPresupuestosBusiness presupuestosBusiness, IPedidosLaboratoriosBusiness pedidosLaboratoriosBusiness)
        {
            _presupuestosBusiness = presupuestosBusiness;
            _pedidosLaboratoriosBusiness = pedidosLaboratoriosBusiness;
        }

        [Authorize(Roles = "ADMIN, RECEPCION, CUENTAS, TESORERIA")]
        public async Task<ActionResult> Index()
        {
            var presupuestos = await _presupuestosBusiness.GetPresupuestosAprobadosDisponibles();
            var presupuestosFormatted = presupuestos.Select(s => new { IdPresupuesto = s.IdPresupuesto, Presupuesto = string.Format("Id.: {0} - {1} - {2}", s.IdPresupuesto, s.PacienteDocumento, s.Paciente) });

            var servicios = new List<Domain.Entities.PedidoLaboratorioServicio>();


            var pacientes = (await _lookupsBusiness.GetAllPacientes()).ToList();
            var laboratorios = (await _lookupsBusiness.GetAllProveedoresLookup()).Where(w => w.EsLaboratorio == true).ToList();
            var estados = Utilities.EnumToDictionary<EstadoPedidoLaboratorio>();

            var model = new PedidoLaboratorioViewModel
            {
                MaestroServicios = servicios
                .ToDropDownList(k => k.IdPedidoLaboratorio, t => t.Servicio, descriptionIncludesKey: false),

                MaestroPresupuestos = presupuestosFormatted
                .ToDropDownList(k => k.IdPresupuesto, t => t.Presupuesto, descriptionIncludesKey: false),

                MaestroLaboratorios = laboratorios
                .ToDropDownList(k => k.IdProveedor, t => t.RazonSocial, descriptionIncludesKey: false),

                MaestroPacientes = pacientes?
                .ToDropDownList(k => k.IdPaciente, t => t.Nombre, descriptionIncludesKey: false) ?? default,

                MaestroEstados = estados
                .ToDropDownList(descriptionIncludesKey: false)                
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _pedidosLaboratoriosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Nuevo(PedidoLaboratorioViewModel model)
        {
            await _pedidosLaboratoriosBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, RECEPCION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Obtener(int id)
        {
            var presupuesto = await _pedidosLaboratoriosBusiness.Get(id);
            return Json(() => presupuesto);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _pedidosLaboratoriosBusiness.Remove(id);
            return Json(() => true);
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA, RECEPCION")]
        public async Task<ActionResult> MarcarEnviadoView(string idsPedidos)
        {
            ViewBag.PostAction = "MarcarEnviado";
            var label = "Marcar Enviado";

            var model = new EnviarItemViewModel { IdsPedidos = idsPedidos, Label = label };

            return View("Enviar", model);
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA, RECEPCION")]
        public async Task<ActionResult> MarcarRecibidoView(string idsPedidos)
        {
            ViewBag.PostAction = "MarcarRecibido";
            var label = "Marcar Recibido";

            var model = new EnviarItemViewModel { IdsPedidos = idsPedidos, Label = label };

            return View("Marcar", model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA, RECEPCION")]
        public async Task<JsonResult> MarcarEnviado(EnviarItemViewModel model)
        {
            await _pedidosLaboratoriosBusiness.Marcar(model, EstadoPedidoLaboratorio.Enviado);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA, RECEPCION")]
        public async Task<JsonResult> MarcarRecibido(EnviarItemViewModel model)
        {
            await _pedidosLaboratoriosBusiness.Marcar(model, EstadoPedidoLaboratorio.Recibido);
            return Json(() => true);
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA, RECEPCION")]
        public async Task<IActionResult> PedidoLaboratorioHistorialView(int idPedidoLaboratorio)
        {
            var model = new PedidoLaboratorioHistorialViewModel
            {
                IdPedidoLaboratorio = idPedidoLaboratorio,
                Item = "PEDIDO X"
            };

            return View("Historial", model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA, RECEPCION")]
        public async Task<JsonResult> PedidoLaboratorioHistorial(int idPedidoLaboratorio, [FromForm] DataTablesRequest request)
        {
            var results = await _pedidosLaboratoriosBusiness.GetHistorial(idPedidoLaboratorio, request);
            return Json(results);
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, RECEPCION")]
        public async Task<ActionResult> Gestion()
        {
            var pacientes = (await _lookupsBusiness.GetAllPacientes()).ToList();
            var laboratorios = (await _lookupsBusiness.GetAllProveedoresLookup()).Where(w => w.EsLaboratorio == true).ToList();
            var estados = Utilities.EnumToDictionary<EstadoPedidoLaboratorio>();

            var model = new GestionPedidosViewModel
            {
                MaestroLaboratorios = laboratorios?.ToDropDownList(k => k.IdProveedor, d => d.RazonSocial, descriptionIncludesKey: false) ?? default,
                MaestroPacientes = pacientes?.ToDropDownList(k => k.IdPaciente, t => t.Nombre, descriptionIncludesKey: false) ?? default,
                MaestroEstados = estados.ToDropDownList(descriptionIncludesKey: false)
            };

            return View("Gestion", model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA, RECEPCION")]
        public async Task<JsonResult> ObtenerPedidos([FromForm] DataTablesRequest request)
        {
            var results = await _pedidosLaboratoriosBusiness.GetAll(request);
            return Json(results);
        }

    }
}
