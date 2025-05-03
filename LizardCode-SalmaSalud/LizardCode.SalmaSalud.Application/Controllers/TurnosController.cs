using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Turnos;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.Framework.Application.Common.Extensions;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Security;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Security.Cryptography.Xml;
using LizardCode.SalmaSalud.Application.Models.Pacientes;
using Mysqlx.Cursor;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Application.Models.Profesionales;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using Microsoft.Extensions.Caching.Memory;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class TurnosController : BusinessController
    {
        private readonly ITurnosBusiness _turnosBusiness;
        private readonly IProfesionalesBusiness _profesionalesBusiness;
        private readonly IMemoryCache _memoryCache;
        private readonly IPacientesBusiness _pacientesBusiness;

        private readonly string _cacheKey_TURNOS = "Paciente_Turnos_";
        private readonly string _cacheKey_LLAMADO_GUARDIA = "Paciente_Llamado_Guardia_";

        public TurnosController(ITurnosBusiness turnosBusiness, IProfesionalesBusiness profesionalesBusiness, IMemoryCache memoryCache, IPacientesBusiness pacientesBusiness)
        {
            _turnosBusiness = turnosBusiness;
            _profesionalesBusiness = profesionalesBusiness;
            _memoryCache = memoryCache;
            _pacientesBusiness = pacientesBusiness;
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<ActionResult> Index()
        {
            var especialidades = (await _lookupsBusiness.GetAllEspecialidades()).ToList();
            var profesionales = (await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa)).ToList();

            var model = new TurnoViewModel
            {
                MaestroEspecialidades = especialidades
                    .ToDropDownList(k => k.IdEspecialidad, t => t.Descripcion, descriptionIncludesKey: false),

                MaestroProfesionales = profesionales
                    .ToDropDownList(k => k.IdProfesional, t => t.Nombre, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model, menuItem: "TurnosGestion");
        }

        [Authorize]
        public async Task<IActionResult> ObtenerTotalesByEstado()
        {
            var totales = await _turnosBusiness.ObtenerTotalesByEstado();

            //Solo para ver los loading bonitos...
            System.Threading.Thread.Sleep(1200);
            //Solo para ver los loading bonitos...

            return Json(() => new {
                canceladosHoy = totales.CanceladosHoy,
                canceladosMensual = totales.CanceladosMensual,
                recepcionadosHoy = totales.RecepcionadosHoy,
                recepcionadosMensual = totales.RecepcionadosMensual,
                atendidosHoy = totales.AtendidosHoy,
                atendidosMensual = totales.AtendidosMensual,
                sobreTurnosHoy = totales.SobreTurnosHoy,
                sobreTurnosMensual = totales.SobreTurnosMensual,
                agendadosHoy = totales.AgendadosHoy,
                agendadosMensual = totales.AgendadosMensual
            });
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<IActionResult> GetTurnosDisponiblesPorDia(DateTime start, DateTime end, int idEspecialidad, int idProfesional)
        {
            if (idEspecialidad == 0 && idProfesional == 0)
                throw new BusinessException("Seleccione al menos un filtro.");

            var eventos = await _turnosBusiness.GetTurnosDisponiblesPorDia(start, end, idEspecialidad, idProfesional);

            return Json(eventos);
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<IActionResult> ObtenerPrimerTurnoDisponible(DateTime start, DateTime end, int idEspecialidad, int idProfesional)
        {
            var turnos = await _turnosBusiness.GetPrimerosTurnosDisponibles(idEspecialidad, idProfesional);
            if (turnos != null && turnos.Count > 0)
            {
                return Json(new { 
                    idProfesionalTurno = turnos.FirstOrDefault().IdProfesionalTurno, 
                    fecha = turnos.FirstOrDefault().Fecha,
                    hora = turnos.FirstOrDefault().Hora,
                    especialidad = turnos.FirstOrDefault().Especialidad,
                    profesional = turnos.FirstOrDefault().Profesional
                });
            }

            return Json(false);
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<IActionResult> ObtenerInfoPrimerTurnoDisponible(int idEspecialidad, int idProfesional)
        {
            var turnos = await _turnosBusiness.GetPrimerosTurnosDisponibles(idEspecialidad, idProfesional);

            return Json(turnos);
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<IActionResult> DetallePage(DateTime fecha, int idEspecialidad, int idProfesional)
        {
            var especialidad = string.Empty;
            var especialidades = (await _lookupsBusiness.GetAllEspecialidades()).ToList();
            var turnos = await _turnosBusiness.GetTurnosDisponibles(fecha, idEspecialidad, idProfesional);

            if (idEspecialidad == 0)
            {
                var profesional = await _profesionalesBusiness.Get(idProfesional);

                if (profesional != null)
                    especialidad = especialidades.FirstOrDefault(e => e.IdEspecialidad == profesional.IdEspecialidad)?.Descripcion.ToUpperInvariant();
            }
            else
                especialidad = especialidades.FirstOrDefault(e => e.IdEspecialidad == idEspecialidad)?.Descripcion.ToUpperInvariant();

            var model = new DetalleViewModel
            {
                Fecha = fecha.ToString("dd/MM/yyyy"),
                Especialidad = especialidad,
                Turnos = turnos
            };

            return View("Detalle", model);
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<IActionResult> PrimerosTurnosDisponibles(int idEspecialidad, int idProfesional)
        {
            var especialidad = string.Empty;
            var especialidades = (await _lookupsBusiness.GetAllEspecialidades()).ToList();

            if (idEspecialidad == 0)
            {
                var profesional = await _profesionalesBusiness.Get(idProfesional);

                if (profesional != null)
                    especialidad = especialidades.FirstOrDefault(e => e.IdEspecialidad == profesional.IdEspecialidad)?.Descripcion.ToUpperInvariant();
            }
            else
                especialidad = especialidades.FirstOrDefault(e => e.IdEspecialidad == idEspecialidad)?.Descripcion.ToUpperInvariant();

            var turnos = await _turnosBusiness.GetPrimerosTurnosDisponibles(idEspecialidad, idProfesional);

            var model = new DetalleViewModel
            {
                Cantidad = turnos.Count,
                Especialidad = especialidad,
                Turnos = turnos
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> ObtenerTurnosHoy([FromForm] DataTablesRequest request)
        {
            var results = await _turnosBusiness.GetTurnosHoy(request);

            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> ObtenerProximosTurnos([FromForm] DataTablesRequest request)
        {
            var results = await _turnosBusiness.GetTurnosProximos(request);

            if (results != null && results.Data != null && results.Data.Count() > 0)
            {
                results.Data = results.Data.Where(w => w.FechaInicio >= DateTime.Now);
            }

            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> ObtenerTurnosReagendar([FromQuery] int idEspecialidad, [FromQuery] int idProfesional, [FromForm] DataTablesRequest request)
        {
            var results = await _turnosBusiness.GetTurnosReAgendar(idEspecialidad, idProfesional, request);

            return Json(results);
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<IActionResult> AsignarPage(int idProfesionalTurno)
        {
            var profesionalTurno = await _profesionalesBusiness.GetProfesionalTurnoById(idProfesionalTurno);
            if (profesionalTurno == null)
                throw new Exception("Turno no encontrado");

            var especialidad = (await _lookupsBusiness.GetAllEspecialidades()).FirstOrDefault(f => f.IdEspecialidad == profesionalTurno.IdEspecialidad);
            var profesional = (await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa)).FirstOrDefault(f => f.IdProfesional == profesionalTurno.IdProfesional);

            var financiadores = (await _lookupsBusiness.GetAllFinanciadores()).ToList();


            var fecha = profesionalTurno.FechaInicio.ToString("dd/MM/yyyy");
            var hora = profesionalTurno.FechaInicio.ToString("HH:mm");

            var model = new AsignarViewModel
            {
                Fecha = fecha + " " + hora,
                Especialidad = especialidad.Descripcion.ToUpperInvariant(),
                Profesional = profesional.Nombre.ToUpperInvariant(),
                IdProfesionalTurno = idProfesionalTurno,

                MaestroFinanciadores = financiadores
                    .ToDropDownList(k => k.IdFinanciador, t => t.Nombre, descriptionIncludesKey: false)
            };

            return View("Asignar", model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Asignar(AsignarViewModel model)
        {
            await _turnosBusiness.Asignar(model);
            return Json(true);
        }

        [Authorize(Roles = "ADMIN, RECEPCION, PACIENTE")]
        public async Task<IActionResult> CancelarView(int idTurno)
        {
            var model = new CancelarViewModel
            {
                IdTurno = idTurno
            };

            return View("Cancelar", model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, PACIENTE")]
        public async Task<JsonResult> Cancelar(CancelarViewModel model)
        {
            await _turnosBusiness.Cancelar(model);

            await RemoveTurnosCache(model.IdTurno);

            return Json(true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, PACIENTE")]
        public async Task<JsonResult> Confirmar(int idTurno)
        {
            await _turnosBusiness.Confirmar(idTurno);

            await RemoveTurnosCache(idTurno);

            return Json(true);
        }

        [Authorize]
        public async Task<IActionResult> RecepcionarView(int idTurno)
        {
            var consultorios = (await _lookupsBusiness.GetAllConsultorios(_permisosBusiness.User.IdEmpresa)).ToList();
            var turno = await _turnosBusiness.GetCustomById(idTurno);

            var model = new RecepcionarViewModel
            {
                IdTurno = idTurno,
                IdPaciente = turno.IdPaciente,

                MaestroConsultorios = consultorios
                    .ToDropDownList(k => k.IdConsultorio, t => t.Descripcion, descriptionIncludesKey: false)
            };

            return View("Recepcionar", model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Recepcionar(RecepcionarViewModel model)
        {
            await _turnosBusiness.Recepcionar(model);
            return Json(true);
        }


        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<IActionResult> ReagendarView(int idTurno)
        {
            var turno = await _turnosBusiness.GetCustomById(idTurno);

            if (turno.IdTipoTurno == (int)TipoTurno.Guardia)
                throw new BusinessException("No se puede reagendar un Turno de Guardia.");

            var model = new ReAgendarViewModel
            {
                IdTurno = idTurno,
                Especialidad = turno.Especialidad,
                Profesional = turno.Profesional,
                IdEspecialidad = turno.IdEspecialidad.Value,
                IdProfesional = turno.IdProfesional.Value
            };

            return View("ReAgendar", model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Reagendar(int idTurno, int idProfesionalTurno)
        {
            await _turnosBusiness.ReAgendar(idTurno, idProfesionalTurno);
            return Json(true);
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<IActionResult> ObservacionesView(int idTurno)
        {
            var turno = await _turnosBusiness.GetCustomById(idTurno);

            var model = new ObservacionesViewModel
            {
                Observaciones = turno.Observaciones
            };

            return View("Observaciones", model);
        }

        [Authorize(Roles = "PROFESIONAL")]
        public async Task<IActionResult> AsignarSobreTurnoPage(int idProfesionalTurno)
        {
            var profesionalTurno = await _profesionalesBusiness.GetProfesionalTurnoById(idProfesionalTurno);
            if (profesionalTurno == null)
                throw new Exception("Turno no encontrado");

            var especialidad = (await _lookupsBusiness.GetAllEspecialidades()).FirstOrDefault(f => f.IdEspecialidad == profesionalTurno.IdEspecialidad);
            var profesional = (await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa)).FirstOrDefault(f => f.IdProfesional == profesionalTurno.IdProfesional);

            var financiadores = (await _lookupsBusiness.GetAllFinanciadores()).ToList();

            var fecha = profesionalTurno.FechaInicio.ToString("dd/MM/yyyy");
            var hora = profesionalTurno.FechaInicio.ToString("HH:mm");

            var model = new AsignarSobreTurnoViewModel
            {
                Fecha = fecha + " " + hora,
                Especialidad = especialidad.Descripcion.ToUpperInvariant(),
                Profesional = profesional.Nombre.ToUpperInvariant(),
                IdProfesionalTurno = idProfesionalTurno,

                MaestroFinanciadores = financiadores
                    .ToDropDownList(k => k.IdFinanciador, t => t.Nombre, descriptionIncludesKey: false)
            };

            return View("AsignarSobreTurno", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> AsignarSobreTurno(AsignarSobreTurnoViewModel model)
        {
            await _turnosBusiness.AsignarSobreTurno(model);
            return Json(true);
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<IActionResult> CancelarAgendaView()
        {
            var profesionales = (await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa)).ToList();

            var model = new CancelarAgendaViewModel
            {
                FechaCancelacion = DateTime.Now.Date.AddDays(1),

                MaestroProfesionales = profesionales
                    .ToDropDownList(k => k.IdProfesional, t => t.Nombre, descriptionIncludesKey: false)
            };

            return View("CancelarAgenda", model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> CancelarAgenda(CancelarAgendaViewModel model)
        {
            await _turnosBusiness.CancelarAgenda(model);
            return Json(true);
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<IActionResult> DemandaEspontaneaPage()
        {
            var financiadores = (await _lookupsBusiness.GetAllFinanciadores()).ToList();
            var especialidades = (await _lookupsBusiness.GetAllEspecialidades()).ToList();
            var profesionales = (await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa)).ToList();
            var consultorios = (await _lookupsBusiness.GetAllConsultorios(_permisosBusiness.User.IdEmpresa)).ToList();

            var model = new DemandaEspontaneaViewModel
            {
                MaestroEspecialidades = especialidades
                    .ToDropDownList(k => k.IdEspecialidad, t => t.Descripcion, descriptionIncludesKey: false),

                MaestroProfesionales = profesionales
                    .ToDropDownList(k => k.IdProfesional, t => t.Nombre, descriptionIncludesKey: false),

                MaestroFinanciadores = financiadores
                    .ToDropDownList(k => k.IdFinanciador, t => t.Nombre, descriptionIncludesKey: false),

                MaestroConsultorios = consultorios
                    .ToDropDownList(k => k.IdConsultorio, t => t.Descripcion, descriptionIncludesKey: false)
            };

            //return View("DemandaEspontanea", model);
            return ActivateMenuItem(view: "DemandaEspontanea", model: model, menuItem: "TurnosGestion");
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> DemandaEspontanea(DemandaEspontaneaViewModel model)
        {
            await _turnosBusiness.DemandaEspontanea(model);
            return Json(true);
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<IActionResult> GuardiaPage()
        {
            var financiadores = (await _lookupsBusiness.GetAllFinanciadores()).ToList();

            var model = new GuardiaViewModel
            {
                MaestroFinanciadores = financiadores
                    .ToDropDownList(k => k.IdFinanciador, t => t.Nombre, descriptionIncludesKey: false)
            };

            //return View("Guardia", model);
            return ActivateMenuItem(view: "Guardia", model: model, menuItem: "TurnosGestion");
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Guardia(GuardiaViewModel model)
        {
            await _turnosBusiness.Guardia(model);
            return Json(true);
        }

        [Authorize]
        public async Task<IActionResult> HistorialView(int idTurno)
        {
            var model = new HistorialViewModel
            {
                IdTurno = idTurno
            };

            return View("Historial", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> ObtenerHistorial(int idTurno, [FromForm] DataTablesRequest request)
        {
            var results = await _turnosBusiness.GetHistorial(idTurno, request);
            return Json(results);
        }

        [Authorize(Roles = "PROFESIONAL")]
        public async Task<IActionResult> AgendaSobreTurnosPage()
        {
            var empresas = (await _lookupsBusiness.GetEmpresasByIdUsuarioLookup(_permisosBusiness.User.Id)).ToList();
            var empresa = empresas.FirstOrDefault(f => f.IdEmpresa == _permisosBusiness.User.IdEmpresa);
            var intervalo = await _profesionalesBusiness.HabilitarCargaAgenda(_permisosBusiness.User.IdProfesional);

            var model = new AgendaSobreTurnosViewModel
            {
                HoraInicio = empresa.TurnosHoraInicio,
                HoraFin = empresa.TurnosHoraFin,
                IntervaloTurnos = intervalo.ToString()
            };

            return View("AgendaSobreTurnos", model);
        }

        [Authorize]
        public async Task<IActionResult> ObtenerTotalesDashboard()
        {
            var totales = await _turnosBusiness.ObtenerTotalesDashboard();

            var porcentajeAgendados = 0;
            if (totales.TotalHoy > 0)
            {
                double porcentaje = (totales.AgendadosHoy * 100) / totales.TotalHoy;
                porcentajeAgendados = Convert.ToInt32(Math.Floor(porcentaje));
            }

            var porcentajeConfirmados = 0;
            if (totales.AgendadosManiana > 0)
            {
                double porcentaje = (totales.ConfirmadosManiana * 100) / totales.AgendadosManiana;
                porcentajeConfirmados = Convert.ToInt32(Math.Floor(porcentaje));
            }

            return Json(() => new
            {
                agendadosHoy = totales.AgendadosHoy,
                totalHoy = totales.TotalHoy,
                porcentajeAgendados = porcentajeAgendados,
                confirmadosManiana = totales.ConfirmadosManiana,
                agendadosManiana = totales.AgendadosManiana,
                porcentajeConfirmados = porcentajeConfirmados
            });
        }

        private async Task RemoveTurnosCache(int idTurno)
        {
            try
            {
                var turno = await _turnosBusiness.GetCustomById(idTurno);
                var paciente = await _pacientesBusiness.Get(turno.IdPaciente);
                var cacheKey = _cacheKey_TURNOS + paciente.Documento;
                _memoryCache.Remove(cacheKey);
            }
            catch { }
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL")]
        public async Task<IActionResult> LlamarView(int idTurno)
        {
            var consultorios = (await _lookupsBusiness.GetAllConsultorios(_permisosBusiness.User.IdEmpresa)).ToList();
            var turno = await _turnosBusiness.GetCustomById(idTurno);

            //if (turno == null)

            var model = new LlamarViewModel
            {
                IdTurno = idTurno,
                IdPaciente = turno.IdPaciente,

                MaestroConsultorios = consultorios
                    .ToDropDownList(k => k.IdConsultorio, t => t.Descripcion, descriptionIncludesKey: false)
            };

            return View("Llamar", model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Llamar(LlamarViewModel model)
        {
            var wasCalled = _memoryCache.Get(_cacheKey_LLAMADO_GUARDIA + model.IdPaciente.ToString());
            if (wasCalled == null)
            {
                await _turnosBusiness.Llamar(model);
                _memoryCache.Set(_cacheKey_LLAMADO_GUARDIA + model.IdPaciente.ToString(), 1, DateTime.Now.AddSeconds(60));
                return Json(true);
            }
            else
            {
                return Json(new { message = "Ya se ha enviado un mensaje, espere al menos 1 minuto para reintentar un nuevo llamado." });
            }            
        }
    }
}
