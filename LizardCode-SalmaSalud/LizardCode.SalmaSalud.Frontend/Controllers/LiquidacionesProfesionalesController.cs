using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Models.LiquidacionesProfesionales;
using LizardCode.SalmaSalud.Application.Models.Turnos;
using System.Linq;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Business;
using System;
using System.Collections.Generic;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Helpers.Utilities;

namespace LizardCode.SalmaSalud.Controllers
{
    public class LiquidacionesProfesionalesController : BusinessController
    {
        private readonly ILiquidacionesProfesionalesBusiness _liquidacionesProfesionalesBusiness;
        private readonly IEvolucionesBusiness _evolucionesBusiness;

        public LiquidacionesProfesionalesController(ILiquidacionesProfesionalesBusiness liquidacionesProfesionalesBusiness, IEvolucionesBusiness evolucionesBusiness)
        {
            _evolucionesBusiness = evolucionesBusiness;
            _liquidacionesProfesionalesBusiness = liquidacionesProfesionalesBusiness;
        }


        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<ActionResult> Index()
        {
            var instituciones = (await _lookupsBusiness.GetAllEmpresasLookup()).ToList(); //_permisosBusiness.User.IdEmpresa
            //var profesionales = (await _lookupsBusiness.GetAllProfesionales(0)).ToList(); //_permisosBusiness.User.IdEmpresa
            var estados = Utilities.EnumToDictionary<EstadoLiquidacionProfesional>();

            var model = new LiquidacionProfesionalViewModel
            {
                MaestroInstituciones = instituciones
                    .ToDropDownList(k => k.IdEmpresa, t => t.NombreFantasia, descriptionIncludesKey: false),

                //MaestroProfesionales = profesionales
                //    .ToDropDownList(k => k.IdProfesional, t => t.Nombre, descriptionIncludesKey: false),

                MaestroEstados = estados.ToDropDownList(descriptionIncludesKey: false)
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _liquidacionesProfesionalesBusiness.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Nuevo(LiquidacionProfesionalViewModel model)
        {
            await _liquidacionesProfesionalesBusiness.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Obtener(int id)
        {
            var liquidacion = await _liquidacionesProfesionalesBusiness.Get(id);
            return Json(() => liquidacion);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> GetPrestaciones(DateTime desde, DateTime hasta, int idProfesional)
        {
            var prestaciones = await _liquidacionesProfesionalesBusiness.GetPrestacionesALiquidar(desde, hasta, idProfesional);
            if (prestaciones == null)
                return Json(() => null);

            return Json(() => prestaciones);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION, CUENTAS, TESORERIA")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _liquidacionesProfesionalesBusiness.Remove(id);
            return Json(() => true);
        }

        private List<LiquidacionProfesionalPrestacionViewModel> MOCK_Prestaciones()
        {
            var prestaciones = new List<LiquidacionProfesionalPrestacionViewModel>();

            prestaciones.Add(new LiquidacionProfesionalPrestacionViewModel
            {
                Descripcion = "01/06/2024 - A0001 - PRESTACION A01 - 31.111.111 PACIENTE 000001",
                Valor = 20000,
                Fijo = 5000,
                Porcentaje = 10,
                ValorPorcentaje = 2000,
                Total = 7000
            });

            prestaciones.Add(new LiquidacionProfesionalPrestacionViewModel
            {
                Descripcion = "04/06/2024 - A0001 - PRESTACION A01 - 31.111.111 PACIENTE 000002",
                Valor = 20000,
                Fijo = 5000,
                Porcentaje = 10,
                ValorPorcentaje = 2000,
                Total = 7000
            });

            prestaciones.Add(new LiquidacionProfesionalPrestacionViewModel
            {
                Descripcion = "05/06/2024 - A0005 - PRESTACION A05 - 31.111.111 PACIENTE 000003",
                Valor = 70000,
                Fijo = 20000,
                Porcentaje = 10,
                ValorPorcentaje = 7000,
                Total = 27000
            });

            prestaciones.Add(new LiquidacionProfesionalPrestacionViewModel
            {
                Descripcion = "06/06/2024 - A0006 - PRESTACION A06 - 31.111.111 PACIENTE 000001",
                Valor = 50000,
                Fijo = 15000,
                Porcentaje = 50,
                ValorPorcentaje = 25000,
                Total = 40000
            });

            return prestaciones;
        }
    }
}
