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
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.Framework.Application.Common.Extensions;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Security;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Security.Cryptography.Xml;
using LizardCode.SalmaSalud.Application.Models.Pacientes;
using LizardCode.SalmaSalud.Application.Models.Profesionales;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.FullCalendar;
using LizardCode.SalmaSalud.Domain.Entities;

namespace LizardCode.SalmaSalud.Controllers
{
    public class TurnosSalaEsperaController : BusinessController
    {
        private readonly ITurnosBusiness _turnosBusiness;

        public TurnosSalaEsperaController(ITurnosBusiness turnosBusiness)
        {
            _turnosBusiness = turnosBusiness;
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL, RECEPCION")]
        public async Task<ActionResult> Index()
        {
            var especialidades = (await _lookupsBusiness.GetAllEspecialidades()).ToList();
            var profesionales = (await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa)).ToList();

            var vademecum = await _lookupsBusiness.GetAllVademecum();
            var dVademecum = new Dictionary<int, string>();
            foreach (var item in vademecum)
            {
                dVademecum.Add(item.IdVademecum, string.Format("{0} {1} - ({2})", item.PrincipioActivo, item.Potencia, item.NombreComercial));
            }

            var model = new TurnoSalaEsperaViewModel
            {
                MaestroEspecialidades = especialidades
                    .ToDropDownList(k => k.IdEspecialidad, t => t.Descripcion, descriptionIncludesKey: false),

                MaestroProfesionales = profesionales
                    .ToDropDownList(k => k.IdProfesional, t => t.Nombre, descriptionIncludesKey: false),

                MaestroVademecum = dVademecum.ToDropDownList(k => k.Key, k => k.Value, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, PROFESIONAL, RECEPCION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _turnosBusiness.GetSalaEspera(request);
            return Json(results);
        }

        [Authorize(Roles = "PROFESIONAL, ADMIN")]
        public async Task<IActionResult> GetAgendaSobreTurnos(DateTime start, DateTime end)
        {
            var eventos = await GetAgenda(start, end);

            return Json(eventos);
        }

        [Authorize(Roles = "PROFESIONAL, ADMIN")]
        private async Task<List<ProfesionalTurnoEvent>> GetAgenda(DateTime desde, DateTime hasta)
        {
            var turnos = await _turnosBusiness.GetAgendaSobreTurnos(desde, hasta, _permisosBusiness.User.IdProfesional);

            return turnos;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, PROFESIONAL")]
        public async Task<JsonResult> GetMedicamentos(string q)
        {
            var medicamentos = await _turnosBusiness.GetMedicamentos(q);
            return Json(() => medicamentos);
        }
    }
}
