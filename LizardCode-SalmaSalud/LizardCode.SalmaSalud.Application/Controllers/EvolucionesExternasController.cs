using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using LizardCode.SalmaSalud.Application.Business;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Evoluciones;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class EvolucionesExternasController : BusinessController
    {
        private readonly IEvolucionesBusiness _evolucionesBusiness;
        private readonly IFinanciadoresBusiness _financiadoresBusiness;

        public EvolucionesExternasController(IEvolucionesBusiness EvolucionesBusiness, IFinanciadoresBusiness financiadoresBusiness)
        {
            _evolucionesBusiness = EvolucionesBusiness;
            _financiadoresBusiness = financiadoresBusiness;
        }


        [Authorize(Roles = "ADMIN, PROFESIONAL_EXTERNO")]
        public async Task<ActionResult> Index()
        {
            var model = new EvolucionViewModel();

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, PROFESIONAL_EXTERNO")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _evolucionesBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, PROFESIONAL_EXTERNO")]
        public async Task<JsonResult> Nuevo(EvolucionExternaViewModel model)
        {
            if (_permisosBusiness.User.IdProfesional == 0)
                throw new BusinessException("Usuario sin profesional asignado.");

            await _evolucionesBusiness.NewExterna(model, null);

            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, PROFESIONAL_EXTERNO")]
        public async Task<JsonResult> Obtener(int id)
        {
            var usuario = await _evolucionesBusiness.Get(id);
            return Json(() => usuario);
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL_EXTERNO")]
        public async Task<IActionResult> NuevaPage()
        {
            if (_permisosBusiness.User.IdProfesional == 0)
                throw new BusinessException("Usuario sin profesional asignado.");

            //var turno = await _turnosBusiness.GetCustomById(idTurno);
            //if (turno == null)
            //    throw new BusinessException("Turno no encontrado.");

            //if (turno.IdEmpresa != _permisosBusiness.User.IdEmpresa)
            //    throw new BusinessException("Acceso no autorizado.");

            //if (turno.IdEstadoTurno != (int)EstadoTurno.Recepcionado)
            //    throw new BusinessException("Estado de turno inválido.");

            //var paciente = await _pacientesBussiness.GetCustomById(turno.IdPaciente);
            //if (paciente == null)
            //    throw new BusinessException("Paciente no encontrado.");

            var financiadores = (await _lookupsBusiness.GetAllFinanciadores()).ToList();
            //var prestacionesFinanciador = await _financiadoresBusiness.GetAllPrestacionesByIdFinanciadorAndIdPlan(turno.ForzarParticular ?  0 : (paciente.IdFinanciador ?? 0), turno.ForzarParticular ? 0 : (paciente.IdFinanciadorPlan ?? 0));
            var otrasPrestaciones = await _lookupsBusiness.GetAllPrestaciones();

            #region Piezas

            var piezas = new Dictionary<int, string>();
            for (int i = 11; i <= 18; i++)
            {
                piezas.Add(i, i.ToString());
            }
            for (int i = 21; i <= 28; i++)
            {
                piezas.Add(i, i.ToString());
            }
            for (int i = 31; i <= 38; i++)
            {
                piezas.Add(i, i.ToString());
            }
            for (int i = 41; i <= 48; i++)
            {
                piezas.Add(i, i.ToString());
            }
            for (int i = 51; i <= 55; i++)
            {
                piezas.Add(i, i.ToString());
            }
            for (int i = 61; i <= 65; i++)
            {
                piezas.Add(i, i.ToString());
            }
            for (int i = 81; i <= 85; i++)
            {
                piezas.Add(i, i.ToString());
            }

            #endregion

            var vademecum = await _lookupsBusiness.GetAllVademecum();
            var dVademecum = new Dictionary<int, string>();
            foreach (var item in vademecum)
            {
                dVademecum.Add(item.IdVademecum, string.Format("{0} {1} - ({2})", item.PrincipioActivo, item.Potencia, item.NombreComercial));
            }

            var model = new EvolucionExternaViewModel
            {
                //IdTurno = idTurno,
                //TipoTurno = turno.TipoTurno,
                //IdPaciente = turno.IdPaciente,
                //ForzarParticular = turno.ForzarParticular,

                //MaestroPrestaciones = prestacionesFinanciador
                //    .ToDropDownList(k => k.IdFinanciadorPrestacion, t => t.Descripcion, descriptionIncludesKey: false),,

                MaestroFinanciadores = financiadores
                    .ToDropDownList(k => k.IdFinanciador, t => t.Nombre, descriptionIncludesKey: false),

                MaestroOtrasPrestaciones = otrasPrestaciones
                    .ToDropDownList(k => k.IdPrestacion, t => t.Descripcion, descriptionIncludesKey: false),

                MaestroPiezas = piezas.ToDropDownList(k => k.Key, k => k.Value, descriptionIncludesKey: false),

                MaestroVademecum = dVademecum.ToDropDownList(k => k.Key, k => k.Value, descriptionIncludesKey: false)
            };

            return View("Nueva", model);
        }

        [Authorize(Roles = "PROFESIONAL_EXTERNO")]
        public async Task<JsonResult> GetPrestacionesByFinanciadorPlan(int idFinanciadorPlan)
        {
            var prestacionesFinanciador = await _financiadoresBusiness.GetAllPrestacionesByIdFinanciadorAndIdPlan(0, idFinanciadorPlan);

            return Json(() => prestacionesFinanciador.ToDropDownList(k => k.IdFinanciadorPrestacion, t => t.Descripcion, descriptionIncludesKey: false));
        }

        [HttpPost]
        [Authorize(Roles = "PROFESIONAL_EXTERNO")]
        public async Task<JsonResult> ImportarEvolucionesExcel(EvolucionExternaViewModel model)
        {
            var results = await _evolucionesBusiness.ImportarEvolucionesExcel(model.FileExcel);
            return Json(() => results);
        }

        [HttpPost]
        [Authorize(Roles = "PROFESIONAL_EXTERNO")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _evolucionesBusiness.Remove(id);
            return Json(() => true);
        }
    }
}