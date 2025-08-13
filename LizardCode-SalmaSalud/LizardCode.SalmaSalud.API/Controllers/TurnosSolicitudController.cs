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
    [Authorize(AuthenticationSchemes = "ApiKeyScheme,Bearer")]
    public class TurnosSolicitudController : Controller
    {
        private IEspecialidadesRepository _especialidadesRepository;
        private ITurnosSolicitudBusiness _turnosSolicitudBusiness;
        private IPacientesBusiness _pacientesBusiness;
        private IRangosHorariosRepository _rangosHorariosRepository;
        private IDiasRepository _diasRepository;

        public TurnosSolicitudController(IEspecialidadesRepository especialidadesRepository, 
                                        ITurnosSolicitudBusiness turnosSolicitudBusiness, 
                                        IPacientesBusiness pacientesBusiness, 
                                        IRangosHorariosRepository rangosHorariosRepository,
                                        IDiasRepository diasRepository)
        {
            this._especialidadesRepository = especialidadesRepository;
            this._turnosSolicitudBusiness = turnosSolicitudBusiness;
            this._pacientesBusiness = pacientesBusiness;
            this._rangosHorariosRepository = rangosHorariosRepository;
            this._diasRepository = diasRepository;
        }

        [HttpGet("~/especialidades")]        
        public async Task<IActionResult> Especialidades()
        {
            var especialidades = await _especialidadesRepository.GetAll<Especialidades>();

            return Json(especialidades);
        }

        [HttpGet("~/dias")]
        public async Task<IActionResult> Dias(int? idEspecialidad)
        {
            var dias = await _diasRepository.GetAll<TipoDia>();
            if (idEspecialidad > 0)
            {
                dias = dias?.Where(r => r.IdEspecialidad == idEspecialidad.Value)?.ToList();
            }

            return Json(dias);
        }

        [HttpGet("~/rangos-horarios")]
        public async Task<IActionResult> RangosHorarios(int? idEspecialidad)
        {
            var rangos = await _rangosHorariosRepository.GetAll<TipoRangoHorario>();
            if (idEspecialidad > 0)
            {
                rangos = rangos?.Where(r => r.IdEspecialidad == idEspecialidad.Value)?.ToList();
            }

            return Json(rangos);
        }

        [HttpPost("~/solicitar")]
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

            if (!paciente.Habilitado)
                throw new UnauthorizedAccessException("Paciente se encuentra deshabilitado.");

            if (paciente.Telefono != telefono)
                throw new UnauthorizedAccessException("Paciente incorrecto.");

            return paciente;
        }
    }
}
