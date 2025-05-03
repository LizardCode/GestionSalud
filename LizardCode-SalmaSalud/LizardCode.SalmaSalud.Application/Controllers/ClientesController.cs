using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Clientes;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class ClientesController : BusinessController
    {
        private readonly IClientesBusiness _clientesBusiness;


        public ClientesController(IClientesBusiness clientesBusiness)
        {
            _clientesBusiness = clientesBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<ActionResult> Index()
        {
            var tiposIVA = Utilities.EnumToDictionary<TipoIVA>();
            var tiposTelefonos = Utilities.EnumToDictionary<TipoTelefono>();
            var tipoDocumentos = (await _lookupsBusiness.GetTipoDocumentos()).ToList().Where(td => td.IdTipoDocumento == (int)TipoDocumento.CUIT);

            var model = new ClienteViewModel
            {
                MaestroTipoIVA = tiposIVA
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroTipoTelefono = tiposTelefonos
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroTipoDocumento = tipoDocumentos?
                    .ToDropDownList(k => k.IdTipoDocumento, t => t.Descripcion, descriptionIncludesKey: false) ?? default,
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _clientesBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Nuevo(ClienteViewModel model)
        {
            await _clientesBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Obtener(int id)
        {
            var usuario = await _clientesBusiness.Get(id);
            return Json(() => usuario);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Modificar(ClienteViewModel model)
        {
            await _clientesBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _clientesBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> ValidarNroCUIT(string cuit, int? idCliente, int? idTipoDocumento)
        {
            var results = await _clientesBusiness.ValidarNroCUIT(cuit, idCliente, (int)TipoDocumento.CUIT);//, idTipoDocumento);
            return Json(results.IsNull() ? true : results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> ValidarNroDocumento(string documento, int? idCliente)
        {
            var results = await _clientesBusiness.ValidarNroDocumento(documento, idCliente);
            return Json(results.IsNull() ? true : results);
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        [HttpPost]
        public async Task<ActionResult> Padron(string cuit)
        {
            var contribuyente = await _clientesBusiness.GetPadron(cuit);
            return Json(() => contribuyente);
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> ObtenerTotalesDashboard()
        {
            var clientes = await _lookupsBusiness.GetAllClientesLookup();
            return Json(() => new { cantidad = clientes.Count });
        }
    }
}
