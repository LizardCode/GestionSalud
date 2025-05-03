using Dapper;
using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Recibos;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class RecibosController : BusinessController
    {
        private readonly IRecibosBusiness _recibosBusiness;

        public RecibosController(IRecibosBusiness recibosBusiness)
        {
            _recibosBusiness = recibosBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<ActionResult> Index()
        {
            var clientes = (await _lookupsBusiness.GetAllClientesLookup()).AsList();
            var estadoRecibo = Utilities.EnumToDictionary<EstadoRecibo>();
            var tipoCobro = Utilities.EnumToDictionary<TipoCobro>();
            var tipoRecibo = Utilities.EnumToDictionary<TipoRecibo>();
            var categoriasRetencion = Utilities.EnumToDictionary<CategoriaRetencion>();
            var monedas = (await _lookupsBusiness.GetAllMonedas()).AsList();
            var bancos = (await _lookupsBusiness.GetBancosByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList();
            var ejercicios = (await _lookupsBusiness.GetEjerciciosByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy"))
                })
                .ToList();

            var cuentas = (await _lookupsBusiness.GetCuentasContablesByIdEmpresa(_permisosBusiness.User.IdEmpresa)).AsList()
                .Where(c => c.IdCodigoObservacion == (int)CodigoObservacion.RETENCION_ING_BRUTOS_CLIENTES)
                .ToList();


            var model = new RecibosViewModel
            {
                Items = new List<RecibosDetalle>(),
                MaestroClientes = clientes.ToDropDownList(k => k.IdCliente, d => d.RazonSocial, descriptionIncludesKey: false),
                MaestroEstadoRecibos = estadoRecibo.ToDropDownList(descriptionIncludesKey: false),
                MaestroBancos = bancos.ToDropDownList(k => k.IdBanco, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
                MaestroTipoCobro = tipoCobro.ToDropDownList(descriptionIncludesKey: false),
                MaestroTipoRecibo = tipoRecibo.ToDropDownList(descriptionIncludesKey: false),
                MaestroMonedas = monedas.ToDropDownList(k => k.Codigo, d => d.Descripcion, descriptionIncludesKey: false),
                MaestroCategoriasRetencion = categoriasRetencion.ToDropDownList(descriptionIncludesKey: false),
                MaestroCuentasContables = cuentas.ToDropDownList(k => k.IdCuentaContable, d => d.Descripcion, descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _recibosBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Nuevo(RecibosViewModel model)
        {
            await _recibosBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Obtener(int id)
        {
            var recibo = await _recibosBusiness.Get(id);
            return Json(() => recibo);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Modificar(RecibosViewModel model)
        {
            await _recibosBusiness.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _recibosBusiness.Remove(id);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> ObtenerAsiento(int id)
        {
            var asiento = await _recibosBusiness.GetAsientoCustom(id);
            return Json(() => asiento);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> ObtenerComprobantesImputar(int id)
        {
            var comprobantes = await _recibosBusiness.GetComprobantesImputar(id);
            return Json(() => comprobantes);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> AddImputaciones(RecibosViewModel model)
        {
            await _recibosBusiness.AddImputaciones(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> GetFechaCambio(string idMoneda1, DateTime fecha)
        {
            var cotizacion = await _lookupsBusiness.GetFechaCambio(idMoneda1, fecha, _permisosBusiness.User.IdEmpresa);
            return Json(() => cotizacion);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<JsonResult> ObtenerAnticiposImputar(int idCliente, string idMoneda)
        {
            var anticipos = await _recibosBusiness.GetAnticiposImputar(idCliente, idMoneda);
            return Json(() => anticipos);
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION, TESORERIA, CUENTAS POR COBRAR")]
        public async Task<ActionResult> Imprimir(int id)
        {
            var pdf = await _impresionesBusiness.GenerarImpresionRecibo(id);
            return File(pdf.Content, "application/pdf");
        }
    }
}
