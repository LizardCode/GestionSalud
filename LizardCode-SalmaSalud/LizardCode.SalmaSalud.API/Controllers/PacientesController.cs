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
    public class PacientesController : Controller
    {
        private IPacientesBusiness _pacientesBusiness;

        public PacientesController(IPacientesBusiness pacientesBusiness)
        {            
            this._pacientesBusiness = pacientesBusiness;
        }

        [HttpGet("~/paciente/{idPaciente}")]
        public async Task<IActionResult> Get(int idPaciente)
        {
            var paciente = await _pacientesBusiness.Get(idPaciente);

            if (paciente == null)
                return NotFound("No se encontró el paciente.");

            return Json(new { paciente.Nombre, paciente.Habilitado });
        }

        [HttpGet("~/paciente-by-documento/{documento}")]
        public async Task<IActionResult> GetByDocumento(int documento)
        {
            var paciente = await _pacientesBusiness.GetLikeDocument(documento.ToString());

            if (paciente == null)
                return NotFound("No se encontró el paciente.");

            return Json(new { paciente.IdPaciente, paciente.Nombre, paciente.Habilitado });
        }
    }
}
