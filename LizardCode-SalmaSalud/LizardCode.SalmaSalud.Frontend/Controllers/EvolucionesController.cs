using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using LizardCode.SalmaSalud.Application.Business;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Evoluciones;
using LizardCode.SalmaSalud.Application.Models.Turnos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Application.Models.Evoluciones.Odontograma;

namespace LizardCode.SalmaSalud.Controllers
{
    public class EvolucionesController : BusinessController
    {
        private readonly IEvolucionesBusiness _evolucionesBusiness;
        private readonly ITurnosBusiness _turnosBusiness;
        private readonly IPacientesBusiness _pacientesBussiness;
        private readonly IFinanciadoresBusiness _financiadoresBusiness;
        private readonly IMemoryCache _memoryCache;

        private readonly string _cacheKey_EVOLUCIONES = "Paciente_Evoluciones_";

        public EvolucionesController(IEvolucionesBusiness EvolucionesBusiness, ITurnosBusiness turnosBusiness,
                                    IPacientesBusiness pacientesBuesiness, IFinanciadoresBusiness financiadoresBusiness, IMemoryCache memoryCache)
        {
            _evolucionesBusiness = EvolucionesBusiness;
            _turnosBusiness = turnosBusiness;
            _pacientesBussiness = pacientesBuesiness;
            _financiadoresBusiness = financiadoresBusiness;
            _memoryCache = memoryCache;
        }


        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Index()
        {
            var model = new EvolucionViewModel();

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _evolucionesBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, PROFESIONAL")]
        public async Task<JsonResult> Nuevo(EvolucionViewModel model)
        {
            if (_permisosBusiness.User.IdProfesional == 0)
                throw new BusinessException("Usuario sin profesional asignado.");

            await _evolucionesBusiness.New(model);

            await RemoveEvolucionesCache(model.IdPaciente);

            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Obtener(int id)
        {
            var usuario = await _evolucionesBusiness.Get(id);
            return Json(() => usuario);
        }

        [Authorize(Roles = "PROFESIONAL, ADMIN")]
        public async Task<IActionResult> NuevaPage(int idTurno)
        {
            if (_permisosBusiness.User.IdProfesional == 0)
                throw new BusinessException("Usuario sin profesional asignado.");

            var turno = await _turnosBusiness.GetCustomById(idTurno);
            if (turno == null)
                throw new BusinessException("Turno no encontrado.");

            if (turno.IdEmpresa != _permisosBusiness.User.IdEmpresa)
                throw new BusinessException("Acceso no autorizado.");

            if (turno.IdEstadoTurno != (int)EstadoTurno.Recepcionado)
                throw new BusinessException("Estado de turno inválido.");

            var paciente = await _pacientesBussiness.GetCustomById(turno.IdPaciente);
            if (paciente == null)
                throw new BusinessException("Paciente no encontrado.");

            var prestacionesFinanciador = await _financiadoresBusiness.GetAllPrestacionesByIdFinanciadorAndIdPlan(turno.ForzarParticular ?  0 : (paciente.IdFinanciador ?? 0), turno.ForzarParticular ? 0 : (paciente.IdFinanciadorPlan ?? 0));
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

            var model = new EvolucionViewModel
            {
                IdTurno = idTurno,
                TipoTurno = turno.TipoTurno,
                IdPaciente = turno.IdPaciente,
                ForzarParticular = turno.ForzarParticular,

                MaestroPrestaciones = prestacionesFinanciador
                    .ToDropDownList(k => k.IdFinanciadorPrestacion, t => t.Descripcion, descriptionIncludesKey: false),

                MaestroOtrasPrestaciones = otrasPrestaciones
                    .ToDropDownList(k => k.IdPrestacion, t => t.Descripcion, descriptionIncludesKey: false),

                MaestroPiezas = piezas.ToDropDownList(k => k.Key, k => k.Value, descriptionIncludesKey: false),

                MaestroVademecum = dVademecum.ToDropDownList(k => k.Key, k => k.Value, descriptionIncludesKey: false)
            };

            return View("Nueva", model);
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL, PACIENTE, PROFESIONAL_EXTERNO")]
        public async Task<IActionResult> ResumenView(int id)
        {
            var evolucion = await _evolucionesBusiness.GetCustomById(id);
            if (evolucion == null)
                throw new BusinessException("No se encontró la evolución.");

            if (_permisosBusiness.User.IdEmpresa > 0 && evolucion.IdEmpresa != _permisosBusiness.User.IdEmpresa)
                throw new BusinessException("Acceso no autorizado.");

            if (_permisosBusiness.User.IdPaciente > 0 && evolucion.IdPaciente != _permisosBusiness.User.IdPaciente)
                throw new BusinessException("Acceso no autorizado.");

            var prestaciones = await _evolucionesBusiness.GetResumenPrestaciones(id);
            var imagenes = await _evolucionesBusiness.GetResumenImagenes(id);
            var recetas = await _evolucionesBusiness.GetResumenRecetas(id);
            var ordenes = await _evolucionesBusiness.GetResumenOrdenes(id);

            var model = new ResumenViewModel
            {
                IdEvolucion = evolucion.IdEvolucion,

                TipoTurno = evolucion.TipoTurno,
                TipoTurnoDescripcion = evolucion.TipoTurnoDescripcion,
                TurnoHora = evolucion.FechaTurno.ToString("HH:mm"),

                Especialidad = evolucion.Especialidad,
                Profesional = evolucion.Profesional,

                Financiador = evolucion.Financiador,
                FinanciadorPlan = evolucion.FinanciadorPlan,
                NroAfiliadoSocio = evolucion.FinanciadorNro,

                Diagnostico = evolucion.Diagnostico,

                Prestaciones = prestaciones,
                Imagenes = imagenes,
                Recetas = recetas,
                Ordenes = ordenes,

                IdTipoUsuario = _permisosBusiness.User.IdTipoUsuario
            };

            return View("_Resumen", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> ObtenerEvolucionesPorPaciente(int idPaciente, [FromForm] DataTablesRequest request)
        {
            var results = await _evolucionesBusiness.GetAll(request, idPaciente);
            return Json(results);
        }
        private async Task RemoveEvolucionesCache(int idPaciente)
        {
            try
            {
                var paciente = await _pacientesBussiness.Get(idPaciente);
                var cacheKey = _cacheKey_EVOLUCIONES + paciente.Documento;
                _memoryCache.Remove(cacheKey);
            }
            catch { }
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL, PROFESIONAL_EXTERNO")]
        public async Task<IActionResult> OdontogramaView(int id, int idPaciente)
        {
            OdontogramaViewModel odontogramaModel = null;

            if (id > 0)
            {
                var evolucion = await _evolucionesBusiness.GetCustomById(id);
                if (evolucion == null)
                    throw new BusinessException("No se encontró la evolución.");

                if (_permisosBusiness.User.IdEmpresa > 0 && evolucion.IdEmpresa != _permisosBusiness.User.IdEmpresa)
                    throw new BusinessException("Acceso no autorizado.");

                if (_permisosBusiness.User.IdPaciente > 0 && evolucion.IdPaciente != _permisosBusiness.User.IdPaciente)
                    throw new BusinessException("Acceso no autorizado.");

                odontogramaModel = await _evolucionesBusiness.GetOdontograma(id);
            }
            else
            {
                odontogramaModel = await _evolucionesBusiness.GetUltimoOdontograma(idPaciente);
            }


            return View("_Odontograma", odontogramaModel);
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL, PROFESIONAL_EXTERNO")]
        public async Task<IActionResult> OdontogramaDetalleView(int id, int pieza)
        {
            if (id > 0)
            {
                var evolucion = await _evolucionesBusiness.GetCustomById(id);
                if (evolucion == null)
                    throw new BusinessException("No se encontró la evolución.");

                if (_permisosBusiness.User.IdEmpresa > 0 && evolucion.IdEmpresa != _permisosBusiness.User.IdEmpresa)
                    throw new BusinessException("Acceso no autorizado.");

                if (_permisosBusiness.User.IdPaciente > 0 && evolucion.IdPaciente != _permisosBusiness.User.IdPaciente)
                    throw new BusinessException("Acceso no autorizado.");
            }

            return View("_OdontogramaDetalle", new OdontogramaPiezaViewModel { IdEvolucion = id, Pieza = pieza });
        }

        [Authorize] 
        public async Task<bool> ValidarPieza(int? pieza)
		{
			if (!pieza.HasValue || pieza == 0 || (pieza >= 11 && pieza <= 18) || (pieza >= 21 && pieza <= 28)
				|| (pieza >= 31 && pieza <= 38) || (pieza >= 41 && pieza <= 48)
				|| (pieza >= 51 && pieza <= 55) || (pieza >= 61 && pieza <= 65)
				|| (pieza >= 71 && pieza <= 75) || (pieza >= 81 && pieza <= 85))
			{
				return await Task.FromResult(true);
			}
			else
			{
				return await Task.FromResult(false);
			}
		}

        [Authorize(Roles = "ADMIN, PROFESIONAL, PACIENTE")]
        public async Task<ActionResult> ImprimirReceta(int idEvolucion, int idEvolucionReceta)
        {
            var pdf = await _impresionesBusiness.GenerarImpresionReceta(idEvolucionReceta, idEvolucion);

            return File(pdf.Content, "application/pdf", pdf.Filename);
        }

        [Authorize(Roles = "ADMIN, PROFESIONAL, PACIENTE")]
        public async Task<ActionResult> ImprimirOrden(int idEvolucion, int idEvolucionOrden)
        {
            var pdf = await _impresionesBusiness.GenerarImpresionOrden(idEvolucionOrden, idEvolucion);

            return File(pdf.Content, "application/pdf", pdf.Filename);
        }
    }
}