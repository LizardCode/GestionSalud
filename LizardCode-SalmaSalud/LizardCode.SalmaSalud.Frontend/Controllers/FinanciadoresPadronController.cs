using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.FinanciadoresPadron;
using LizardCode.SalmaSalud.Application.Models.PrestacionesFinanciador;
using LizardCode.SalmaSalud.Application.Models.Turnos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Controllers
{
    public class FinanciadoresPadronController : BusinessController
    {
        private readonly IFinanciadoresBusiness _financiadoresBusiness;
        private readonly IFinanciadoresPadronBusiness _financiadoresPadronBusiness;
        private readonly IPacientesBusiness _pacientesBusiness;

        public FinanciadoresPadronController(IFinanciadoresBusiness financiadoresBusiness,
                                                    IFinanciadoresPadronBusiness financiadoresPadronBusiness,
                                                    IPacientesBusiness pacientesBusiness)
        {
            _financiadoresBusiness = financiadoresBusiness;
            _financiadoresPadronBusiness = financiadoresPadronBusiness;
            _pacientesBusiness = pacientesBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index(int idFinanciador)
        {
            if (idFinanciador == 0)
                throw new System.Exception("No se ha especificado el Financiador");

            var financiador = await _financiadoresBusiness.Get(idFinanciador);
            if (financiador == null)
                throw new System.Exception("No encontró el Financiador");

            ViewData["Financiador"] = financiador.Nombre.ToUpperInvariant();
            HttpContext.Session.SetInt32("ID_FINANCIADOR", idFinanciador);

            var model = new FinanciadorPadronViewModel();

            return ActivateMenuItem(model: model, menuItem: "Financiadores");
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var idFinanciador = HttpContext.Session.GetInt32("ID_FINANCIADOR");

            var results = await _financiadoresPadronBusiness.GetAll(request, idFinanciador ?? 0);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Nuevo(FinanciadorPadronViewModel model)
        {
            var idFinanciador = HttpContext.Session.GetInt32("ID_FINANCIADOR");

            await _financiadoresPadronBusiness.New(model, idFinanciador ?? 0);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, PROFESIONAL")]
        public async Task<JsonResult> Obtener(int id)
        {
            var prestacion = await _financiadoresPadronBusiness.Get(id);
            return Json(() => prestacion);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Modificar(FinanciadorPadronViewModel model)
        {
            var idFinanciador = HttpContext.Session.GetInt32("ID_FINANCIADOR");

            await _financiadoresPadronBusiness.Update(model, idFinanciador ?? 0);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _financiadoresPadronBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> ValidarDocumento(string documento, int? idFinanaciadorPadron, int? idFinanciadorPlan)
        {
            var idFinanciador = HttpContext.Session.GetInt32("ID_FINANCIADOR");

            var result = false;
            var afiliado = await _financiadoresPadronBusiness.GetByDocumento(documento, idFinanciador ?? 0);

            if (afiliado != null)
            {
                if (afiliado.IdFinanciadorPadron == 0)
                    result = false;
                else 
                {
                    if (idFinanaciadorPadron > 0)
                    {
                        result = afiliado.IdFinanciadorPadron == idFinanaciadorPadron;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }

            return Json(!result);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> ImportarPadronFinanciadorExcel(PrestacionViewModel model)
        {
            var idFinanciador = HttpContext.Session.GetInt32("ID_FINANCIADOR");

            var results = await _financiadoresPadronBusiness.ImportarPrestacionesExcel(model.FileExcel, idFinanciador ?? 0);
            return Json(() => results);
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, PROFESIONAL, RECEPCION")]
        public async Task<JsonResult> ValidarPadron(int idPaciente, string documento, int idFinanciador, string financiadorNro)
        {
            documento = documento ?? "";
            documento = documento.Replace(".", string.Empty);

            if (idPaciente > 0)
            {
                var paciente = await _pacientesBusiness.Get(idPaciente);
                if (paciente != null)
                {
                    documento = paciente.Documento;
                    idFinanciador = paciente.IdFinanciador ?? 0;
                    financiadorNro = paciente.FinanciadorNro;
                }
                else
                    throw new BusinessException("No se encontró el paciente.");              
            }

            if (idFinanciador == 0)
                return Json(new { afiliadoValido = true});

            var padron = await _financiadoresPadronBusiness.GetByDocumento(documento, idFinanciador);
            if (padron != null)
            {
                if (padron.IdFinanciadorPadron == 0)
                    return Json(new { afiliadoValido = true });
                else
                    return Json(new { afiliadoValido = padron.FinanciadorNro == financiadorNro });
            }
            else
                return Json(new { afiliadoValido = false });
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION, PROFESIONAL, RECEPCION")]
        public async Task<IActionResult> ValidarPadronView(int idPaciente, string documento, int idFinanciador, string financiadorNro)
        {
            documento = documento ?? "";
            documento = documento.Replace(".", string.Empty);

            var afiliadoValido = false;

            if (idPaciente > 0)
            {
                var paciente = await _pacientesBusiness.Get(idPaciente);
                if (paciente != null)
                {
                    documento = paciente.Documento;
                    idFinanciador = paciente.IdFinanciador ?? 0;
                    financiadorNro = paciente.FinanciadorNro;
                }
                else
                    throw new BusinessException("No se encontró el paciente.");
            }

            if (idFinanciador == 0)
                afiliadoValido = true;

            var padron = await _financiadoresPadronBusiness.GetByDocumento(documento, idFinanciador);
            if (padron != null)
            {
                afiliadoValido = true;
            }

            var model = new MensajeViewModel
            {
                MensajeForzarParticular = !afiliadoValido,
                MensajePacienteFinanciadorNro = financiadorNro,
                MensajePadronFinanciadorNro = padron?.FinanciadorNro             

            };

            return View("Mensaje", model);

        }
    }
}