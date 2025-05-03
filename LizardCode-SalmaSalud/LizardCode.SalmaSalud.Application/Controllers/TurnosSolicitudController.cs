using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using LizardCode.SalmaSalud.Application.Business;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.TurnosSolicitud;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class TurnosSolicitudController : BusinessController
    {
        private readonly ITurnosSolicitudBusiness _turnosSolicitudBusiness;
        private readonly IMemoryCache _memoryCache;
        private readonly string _cacheKey_TURNOS = "Paciente_Turnos_";

        public TurnosSolicitudController(ITurnosSolicitudBusiness turnosSolicitudBusiness, IMemoryCache memoryCache)
        {
            _turnosSolicitudBusiness = turnosSolicitudBusiness;
            _memoryCache = memoryCache;
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<ActionResult> Index()
        {
            var dias = Utilities.EnumToDictionary<Dias>();
            var rangos = Utilities.EnumToDictionary<RangoHorario>();

            var especialidades= (await _lookupsBusiness.GetAllEspecialidades()).ToList();
            var pacientes = (await _lookupsBusiness.GetAllPacientes()).ToList();

            var model = new TurnoSolicitudViewModel
            {
                MaestroDias = dias
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroRangosHorarios = rangos
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroPacientes = pacientes
                    .ToDropDownList(k => k.IdPaciente, t => t.Nombre, descriptionIncludesKey: false),

                MaestroEspecialidades = especialidades
                    .ToDropDownList(k => k.IdEspecialidad, t => t.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _turnosSolicitudBusiness.GetAll(request);
            return Json(results);
        }

        //[HttpPost]
        //[Authorize(Roles = "ADMIN, RECEPCION")]
        //public async Task<JsonResult> Nuevo(TurnoSolicitudViewModel model)
        //{
        //    await _turnosSolicitudBusiness.New(model);
        //    return Json(() => true);
        //}

        [HttpGet]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Obtener(int id)
        {
            var usuario = await _turnosSolicitudBusiness.Get(id);
            return Json(() => usuario);
        }

        //[HttpPost]
        //[Authorize(Roles = "ADMIN, RECEPCION")]
        //public async Task<JsonResult> Modificar(TurnoSolicitudViewModel model)
        //{
        //    await _turnosSolicitudBusiness.Update(model);
        //    return Json(() => true);
        //}

        //[HttpPost]
        //[Authorize(Roles = "ADMIN, RECEPCION")]
        //public async Task<JsonResult> Eliminar(int id)
        //{
        //    await _turnosSolicitudBusiness.Remove(id);
        //    return Json(() => true);
        //}

        [Authorize(Roles = "ADMIN, RECEPCION, PACIENTE")]
        public async Task<IActionResult> CancelarView(int idTurnoSolicitud)
        {
            var model = new CancelarViewModel
            {
                IdTurnoSolicitud = idTurnoSolicitud
            };

            return View("Cancelar", model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION, PACIENTE")]
        public async Task<JsonResult> Cancelar(CancelarViewModel model)
        {
            await _turnosSolicitudBusiness.Cancelar(model);

            var cacheKey = _cacheKey_TURNOS + _permisosBusiness.User.Login;
            _memoryCache.Remove(cacheKey);
            //await RemoveTurnosCache(model.IdTurnoSolicitud);

            return Json(true);
        }

        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<IActionResult> AsignarView(int idTurnoSolicitud)
        {
            var model = new AsignarViewModel
            {
                IdTurnoSolicitud = idTurnoSolicitud
            };

            return View("Asignar", model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, RECEPCION")]
        public async Task<JsonResult> Asignar(AsignarViewModel model)
        {
            await _turnosSolicitudBusiness.Asignar(model);

            var cacheKey = _cacheKey_TURNOS + _permisosBusiness.User.Login;
            _memoryCache.Remove(cacheKey);
            //await RemoveTurnosCache(model.IdTurnoSolicitud);

            return Json(true);
        }
    }
}
