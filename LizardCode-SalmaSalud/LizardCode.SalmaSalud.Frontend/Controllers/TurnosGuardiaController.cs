using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Turnos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Models.FullCalendar;

namespace LizardCode.SalmaSalud.Controllers
{
    public class TurnosGuardiaController : BusinessController
    {
        private readonly ITurnosBusiness _turnosBusiness;

        public TurnosGuardiaController(ITurnosBusiness turnosBusiness)
        {
            _turnosBusiness = turnosBusiness;
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL, RECEPCION")]
        public async Task<ActionResult> Index()
        {
            var vademecum = await _lookupsBusiness.GetAllVademecum();
            var dVademecum = new Dictionary<int, string>();
            foreach (var item in vademecum)
            {
                dVademecum.Add(item.IdVademecum, string.Format("{0} {1} - ({2})", item.PrincipioActivo, item.Potencia, item.NombreComercial));
            }

            var model = new TurnoGuardiaViewModel
            {
                MaestroVademecum = dVademecum.ToDropDownList(k => k.Key, k => k.Value, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, PROFESIONAL, RECEPCION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _turnosBusiness.GetGuardia(request);
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
