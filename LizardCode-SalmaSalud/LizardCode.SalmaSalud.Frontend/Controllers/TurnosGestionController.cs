using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Turnos;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.Framework.Application.Common.Extensions;
using System;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Helpers.Utilities;

namespace LizardCode.SalmaSalud.Controllers
{
    public class TurnosGestionController : BusinessController
    {
        private readonly ITurnosBusiness _turnosBusiness;

        public TurnosGestionController(ITurnosBusiness turnosBusiness)
        {
            _turnosBusiness = turnosBusiness;
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<ActionResult> IndexFiltrado(string filtro)
        {
            TempData["MVC_FILTRO"] = filtro;

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<ActionResult> Index()
        {
            var especialidades = (await _lookupsBusiness.GetAllEspecialidades()).ToList();
            var profesionales = (await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa)).ToList();

            var tiposTurno = (await _lookupsBusiness.GetTiposTurno()).ToList();

            var model = new TurnoGestionViewModel
            {
                MaestroEspecialidades = especialidades
                    .ToDropDownList(k => k.IdEspecialidad, t => t.Descripcion, descriptionIncludesKey: false),

                MaestroProfesionales = profesionales
                    .ToDropDownList(k => k.IdProfesional, t => t.Nombre, descriptionIncludesKey: false),

                MaestroTiposTurno = tiposTurno
                    .ToDropDownList(k => k.IdTipoTurno, t => t.Descripcion, descriptionIncludesKey: false)
            };

            var MVC_FILTRO = TempData["MVC_FILTRO"];
            if (MVC_FILTRO != null)
            {
                if ((string)MVC_FILTRO == "TH")
                {
                    model.FechaDesde = DateTime.Now.Date;
                    model.FechaHasta = DateTime.Now.AddDays(1).Date;
                } else if ((string)MVC_FILTRO == "TM")
                {
                    model.FechaDesde = DateTime.Now.AddDays(1).Date;
                    model.FechaHasta = DateTime.Now.AddDays(2).Date;
                }
            }

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _turnosBusiness.GetTurnos(request);
            return Json(results);
        }
    }
}
