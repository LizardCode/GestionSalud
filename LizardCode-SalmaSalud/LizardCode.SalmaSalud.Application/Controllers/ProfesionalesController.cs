using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Profesionales;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Application.Models.FullCalendar;
using Mysqlx.Cursor;
using LizardCode.SalmaSalud.Application.Models.Turnos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class ProfesionalesController : BusinessController
    {
        private readonly IProfesionalesBusiness _ProfesionalesBusiness;


        public ProfesionalesController(IProfesionalesBusiness ProfesionalesBusiness)
        {
            _ProfesionalesBusiness = ProfesionalesBusiness;
        }


        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Index()
        {
            var tiposIVA = Utilities.EnumToDictionary<TipoIVA>();
            var tiposTelefonos = Utilities.EnumToDictionary<TipoTelefono>();
            var empresas = (await _lookupsBusiness.GetAllEmpresasLookup()).ToList();
            //var empresas = (await _lookupsBusiness.GetEmpresasByIdUsuarioLookup(_permisosBusiness.User.Id)).ToList();
            var especialidades = (await _lookupsBusiness.GetAllEspecialidades()).ToList();

            var empresa = empresas.FirstOrDefault(f => f.IdEmpresa == _permisosBusiness.User.IdEmpresa);

            var model = new ProfesionalViewModel
            {
                MaestroTipoIVA = tiposIVA
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroTipoTelefono = tiposTelefonos
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroEmpresas = empresas
                    .ToDropDownList(k => k.IdEmpresa, t => t.RazonSocial, descriptionIncludesKey: false),

                MaestroEspecialidades = especialidades
                    .ToDropDownList(k => k.IdEspecialidad, t => t.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _ProfesionalesBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Nuevo(ProfesionalViewModel model)
        {
            await _ProfesionalesBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Obtener(int id)
        {
            var profesional = await _ProfesionalesBusiness.Get(id);
            return Json(() => profesional);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Modificar(ProfesionalViewModel model)
        {
            await _ProfesionalesBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _ProfesionalesBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> ValidarNroCUIT(string cuit, int? idProfesional)
        {
            var results = await _ProfesionalesBusiness.ValidarNroCUIT(cuit, idProfesional);
            return Json(results.IsNull() ? true : results);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<ActionResult> Padron(string cuit)
        {
            var contribuyente = await _ProfesionalesBusiness.GetPadron(cuit);
            return Json(() => contribuyente);
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AgendaPage(int idProfesional)
        {
            var empresas = (await _lookupsBusiness.GetEmpresasByIdUsuarioLookup(_permisosBusiness.User.Id)).ToList();
            var empresa = empresas.FirstOrDefault(f => f.IdEmpresa == _permisosBusiness.User.IdEmpresa);
            var intervalo = await _ProfesionalesBusiness.HabilitarCargaAgenda(idProfesional);

            var profesional = (await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa)).FirstOrDefault(p => p.IdProfesional == idProfesional);

            var model = new AgendaViewModel
            {
                IdProfesional = idProfesional,
                Profesional = profesional?.Nombre.ToUpperInvariant(),
                HoraInicio = empresa.TurnosHoraInicio,
                HoraFin = empresa.TurnosHoraFin,
                IntervaloTurnos = intervalo.ToString()
            };

            return View("Agenda", model);
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAgenda(DateTime start, DateTime end, int idProfesional)
        {
            var eventos = await GetAgendaProfesional(start, end, idProfesional);

            return Json(eventos);
        }

        private async Task<List<ProfesionalTurnoEvent>> GetAgendaProfesional(DateTime desde, DateTime hasta, int idProfesional)
        {
            var turnos = await _ProfesionalesBusiness.GetAgenda(desde, hasta, idProfesional);

            return turnos;
        }

        //public async Task<IActionResult> HabilitaCargaAgenda(int idProfesional)
        //{
        //    var habilita = await _ProfesionalesBusiness.HabilitarCargaAgenda(idProfesional);
        //    return Json(habilita);
        //}

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> HabilitarAgenda(DateTime desde, int idProfesional)
        {
            desde = desde.ToUniversalTime();
            await _ProfesionalesBusiness.AddTurno(idProfesional, desde);
            return Json(true);
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DesHabilitarAgenda(int idProfesionalTurno)
        {
            await _ProfesionalesBusiness.RemoveTurno(idProfesionalTurno);
            return Json(true);
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CopiarSemana(DateTime desde, int idProfesional)
        {
            await _ProfesionalesBusiness.CopiarSemana(idProfesional, desde);
            return Json(true);
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CopiarDia(DateTime desde, int idProfesional)
        {
            await _ProfesionalesBusiness.CopiarDia(idProfesional, desde);
            return Json(true);
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetProfesionalesByEmpresa(int idEmpresa)
        {
            var profesionales = (await _lookupsBusiness.GetAllProfesionales(idEmpresa)).ToList();

            return Json(() => profesionales);
        }
    }
}
