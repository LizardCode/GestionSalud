using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Business;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.AuditoriasChatApiViewModel;
using LizardCode.SalmaSalud.Application.Models.Evoluciones;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class AuditoriasChatApiController : BusinessController
    {
        private readonly IChatApiBusiness _chatApiBusiness;

        public AuditoriasChatApiController(IChatApiBusiness chatApiBusiness)
        {
            _chatApiBusiness = chatApiBusiness;
        }


        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Index()
        {
            var estados = Utilities.EnumToDictionary<EstadoAuditoriaChatApi>();
            var eventos = Utilities.EnumToDictionary<EventosChatApi>();

            var model = new AuditoriaChatApiViewModel
            {
                MaestroEstados = estados
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroEventos = eventos
                    .ToDropDownList(descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _chatApiBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Nuevo(AuditoriaChatApiViewModel model)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Obtener(int id)
        {
            var auditoria = await _chatApiBusiness.Get(id);
            return Json(() => auditoria);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Modificar(AuditoriaChatApiViewModel model)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerUltimosMensajes([FromForm] DataTablesRequest request)
        {
            var results = await _chatApiBusiness.GetLast(request);
            return Json(results);
        }

        [Authorize]
        public async Task<IActionResult> ObtenerTotalesByEstado()
        {
            var totales = await _chatApiBusiness.ObtenerTotalesByEstado();

            //Solo para ver los loading bonitos...
            System.Threading.Thread.Sleep(1200);
            //Solo para ver los loading bonitos...

            return Json(() => new {
                auditoriaChatApiErrorHoy = totales.AuditoriaChatApiErrorHoy,
                auditoriaChatApiEnviadosHoy = totales.AuditoriaChatApiEnviadosHoy
            });
        }
    }
}
