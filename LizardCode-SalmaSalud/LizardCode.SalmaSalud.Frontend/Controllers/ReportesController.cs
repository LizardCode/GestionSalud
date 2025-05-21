using Dapper;
using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.BalancePatrimonial;
using System;
using System.Linq;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Application.Models.Evoluciones;
using LizardCode.SalmaSalud.Application.Models.Reportes;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Helpers.Utilities;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Controllers
{
    public class ReportesController : BusinessController
    {
        private readonly IEvolucionesBusiness _evolucionesBusiness;
        private readonly ITurnosBusiness _turnosBusiness;

        public ReportesController(IEvolucionesBusiness evolucionesBusiness,
                                    ITurnosBusiness turnosBusiness)
        {
            _evolucionesBusiness = evolucionesBusiness;
            _turnosBusiness = turnosBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> PrestacionesProfesional()
        {
            var profesionales = (await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa)).ToList();

            var model = new PrestacionesProfesionalViewModel
            {
                FechaDesde = GetPreviousMonthDateFrom().ToString("dd/MM/yyyy"),
                FechaHasta = GetPreviousMonthDateTo().ToString("dd/MM/yyyy"),

                MaestroProfesionales = new SelectList(profesionales, "IdProfesional", "Nombre", 0)
            };

            return ActivateMenuItem(view: "~/Views/Reportes/PrestacionesProfesional.cshtml", model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerPrestacionesProfesional([FromForm] DataTablesRequest request)
        {
            var results = await _evolucionesBusiness.GetPrestacionesProfesional(request);
            return Json(results);
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> PrestacionesFinanciador()
        {
            var financiadores = (await _lookupsBusiness.GetAllFinanciadores()).ToList();
            var tiposPrestacion = Utilities.EnumToDictionary<TipoPrestacion>();

            var model = new PrestacionesFinanciadorViewModel
            {
                FechaDesde = GetPreviousMonthDateFrom().ToString("dd/MM/yyyy"),
                FechaHasta = GetPreviousMonthDateTo().ToString("dd/MM/yyyy"),

                MaestroFinanciadores = new SelectList(financiadores, "IdFinanciador", "Nombre", 0),
                MaestroTiposPrestacion = tiposPrestacion
                    .ToDropDownList(descriptionIncludesKey: false)
            };

            return ActivateMenuItem(view: "~/Views/Reportes/PrestacionesFinanciador.cshtml", model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerPrestacionesFinanciador([FromForm] DataTablesRequest request)
        {
            var results = await _evolucionesBusiness.GetPrestacionesFinanciador(request);
            return Json(results);
        }

        [Authorize(Roles = "ADMIN, RECEPCION, ADMINISTRACION")]
        public async Task<ActionResult> Turnos()
        {
            var tiposTurno = (await _lookupsBusiness.GetTiposTurno()).ToList();
            //var estadosFiltro = new List<int> { (int)EstadoTurno.Atendido, (int)EstadoTurno.AusenteConAviso, (int)EstadoTurno.AusenteSinAviso, (int)EstadoTurno.Agendado, (int)EstadoTurno.ReAgendado };
            //var estadosTurno = Utilities.EnumToDictionary<EstadoTurno>().Where(w => estadosFiltro.Contains((int)w.Key)).ToDictionary();
            var estadosTurno = Utilities.EnumToDictionary<EstadoTurno>();

            var financiadores = (await _lookupsBusiness.GetAllFinanciadores()).ToList();
            var profesionales = (await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa)).ToList();

            var model = new TurnosViewModel
            {
                FechaDesde = GetPreviousMonthDateFrom().ToString("dd/MM/yyyy"),
                FechaHasta = GetPreviousMonthDateTo().ToString("dd/MM/yyyy"),


                MaestroTiposTurno = tiposTurno
                    .ToDropDownList(k => k.IdTipoTurno, t => t.Descripcion, descriptionIncludesKey: false),

                MaestroEstadosTurno = estadosTurno
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroFinanciadores = new SelectList(financiadores, "IdFinanciador", "Nombre", 0),
                MaestroProfesionales = new SelectList(profesionales, "IdProfesional", "Nombre", 0)
            };

            return ActivateMenuItem(view: "~/Views/Reportes/Turnos.cshtml", model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTurnos([FromForm] DataTablesRequest request)
        {
            var results = await _turnosBusiness.GetReporteTurnos(request);
            return Json(results);
        }

        [Authorize(Roles = "ADMIN, RECEPCION, ADMINISTRACION")]
        public async Task<ActionResult> TurnosEstadisticas()
        {
            var profesionales = (await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa)).ToList();
            var especialidades = (await _lookupsBusiness.GetAllEspecialidades()).ToList();

            var model = new TurnosEstadisticasViewModel
            {
                FechaDesde = GetPreviousMonthDateFrom().ToString("dd/MM/yyyy"),
                FechaHasta = GetPreviousMonthDateTo().ToString("dd/MM/yyyy"),

                MaestroProfesionales = new SelectList(profesionales, "IdProfesional", "Nombre", 0),
                MaestroEspecialidades = new SelectList(especialidades, "IdEspecialidad", "Descripcion", 0)
            };

            return ActivateMenuItem(view: "~/Views/Reportes/TurnosEstadisticas.cshtml", model: model);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, RECEPCION, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTurnosEstadisticasEstados(TurnosEstadisticasViewModel filters)
        {
            var results = await _turnosBusiness.GetEstadisticasEstados(filters);
            return Json(results);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, RECEPCION, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTurnosEstadisticasAusentes(TurnosEstadisticasViewModel filters)
        {
            var results = await _turnosBusiness.GetEstadisticasAusentes(filters);
            return Json(results);
        }

        [Authorize(Roles = "ADMIN, RECEPCION, ADMINISTRACION")]
        public async Task<ActionResult> EvolucionesEstadisticas()
        {
            var profesionales = (await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa)).ToList();
            var especialidades = (await _lookupsBusiness.GetAllEspecialidades()).ToList();

            var model = new EvolucionesEstadisticasViewModel
            {
                FechaDesde = GetPreviousMonthDateFrom().ToString("dd/MM/yyyy"),
                FechaHasta = GetPreviousMonthDateTo().ToString("dd/MM/yyyy"),

                MaestroProfesionales = new SelectList(profesionales, "IdProfesional", "Nombre", 0),
                MaestroEspecialidades = new SelectList(especialidades, "IdEspecialidad", "Descripcion", 0)
            };

            return ActivateMenuItem(view: "~/Views/Reportes/EvolucionesEstadisticas.cshtml", model: model);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, RECEPCION, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerEvolucionesEstadisticas(EvolucionesEstadisticasViewModel filters)
        {
            var results = await _evolucionesBusiness.GetEstadisticas(filters);
            return Json(results);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, RECEPCION, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerEvolucionesEstadisticasFinanciador(EvolucionesEstadisticasViewModel filters)
        {
            var results = await _evolucionesBusiness.GetEstadisticasFinanciador(filters);
            return Json(results);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, RECEPCION, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerEvolucionesEstadisticasEspecialidades(EvolucionesEstadisticasViewModel filters)
        {
            var results = await _evolucionesBusiness.GetEstadisticasEspecialidad(filters);
            return Json(results);
        }

        private DateTime GetPreviousMonthDateFrom()
        {
            var previuosMonthDate = DateTime.Now.AddMonths(-1);

            return previuosMonthDate.AddDays(-previuosMonthDate.Day + 1);
        }

        private DateTime GetPreviousMonthDateTo()
            => DateTime.Now.AddDays(-DateTime.Now.Day);
    }
}
