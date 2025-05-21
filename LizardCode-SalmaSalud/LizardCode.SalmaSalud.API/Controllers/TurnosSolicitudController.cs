using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.API.Models;
using LizardCode.SalmaSalud.Application.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LizardCode.SalmaSalud.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TurnosSolicitudController : Controller
    {
        private IEspecialidadesRepository _especialidadesRepository;
        private ITurnosSolicitudBusiness _turnosSolicitudBusiness;
        private IPacientesBusiness _pacientesBusiness;

        public TurnosSolicitudController(IEspecialidadesRepository especialidadesRepository, ITurnosSolicitudBusiness turnosSolicitudBusiness, IPacientesBusiness pacientesBusiness)
        {
            this._especialidadesRepository = especialidadesRepository;
            this._turnosSolicitudBusiness = turnosSolicitudBusiness;
            this._pacientesBusiness = pacientesBusiness;
        }

        [HttpGet("~/especialidades")]
        [Authorize]
        public async Task<IActionResult> Especialidades()
        {
            var especialidades = await _especialidadesRepository.GetAll<Especialidades>();

            return Json(especialidades);
        }

        [HttpGet("~/dias")]
        [Authorize]
        public async Task<IActionResult> Dias()
        {
            var dias = Utilities.EnumToDictionary<Dias>();

            return Json(dias);
        }

        [HttpGet("~/rangos-horarios")]
        [Authorize]
        public async Task<IActionResult> RangosHorarios()
        {
            var rangos = Utilities.EnumToDictionary<RangoHorario>();

            return Json(rangos);
        }

        [HttpPost("~/solicitar")]
        [Authorize]
        public async Task<IActionResult> Solicitar(SolicitarModel model)
        {
            var idSolicitud = 0;

            try
            {
                var paciente = await GetPaciente(model.Documento, model.Telefono);

                idSolicitud = await _turnosSolicitudBusiness.Solicitar(new Application.Models.PortalPacientes.NuevaSolicitudViewModel
                {
                    IdPaciente = paciente.IdPaciente,
                    IdEspecialidad = model.IdEspecialidad,
                    Dias = model.Dias,
                    RangosHorarios = model.RangosHorarios
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(idSolicitud);
        }

        [HttpPost("~/cancelar")]
        [Authorize]
        public async Task<IActionResult> Cancelar(CancelarModel model)
        {
            try 
            { 
                var paciente = await GetPaciente(model.Documento, model.Telefono);
            
                await _turnosSolicitudBusiness.Cancelar(new Application.Models.TurnosSolicitud.CancelarViewModel
                {
                    IdTurnoSolicitud = model.IdTurnoSolicitud                
                }, paciente);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        private async Task<Paciente> GetPaciente(string documento, string telefono)
        {
            var paciente = await _pacientesBusiness.GetLikeDocument(documento);

            if (paciente == null)
                throw new UnauthorizedAccessException("Paciente no encontrado.");

            if (paciente.Telefono != telefono)
                throw new UnauthorizedAccessException("Paciente incorrecto.");

            return paciente;
        }
    }
}
