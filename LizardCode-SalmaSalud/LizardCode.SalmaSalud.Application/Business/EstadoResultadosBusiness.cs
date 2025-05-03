using Dapper.DataTables.Models;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class EstadoResultadosBusiness : BaseBusiness, IEstadoResultadosBusiness
    {
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly IRubrosContablesRepository _rubrosContablesRepository;

        public EstadoResultadosBusiness(
            IAsientosDetalleRepository asientosDetalleRepository,
            IRubrosContablesRepository rubrosContablesRepository,
            ICuentasContablesRepository cuentasContablesRepository)
        {
            _asientosDetalleRepository = asientosDetalleRepository;
            _cuentasContablesRepository = cuentasContablesRepository;
            _rubrosContablesRepository = rubrosContablesRepository;
        }

        public async Task<List<Custom.EstadoResultado>> GetAll(DataTablesRequest request)
        {
            var filters = request.ParseFilters();
            var idEjercicio = 0;
            DateTime fechaHasta;

            if (!filters.ContainsKey("IdEjercicio"))
                throw new BusinessException("Seleccione un Ejercicio");
            else
                idEjercicio = int.Parse(filters["IdEjercicio"].ToString());


            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
                fechaHasta = DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null);
            else
                throw new BusinessException("Ingrese Fecha Hasta");

            List<Custom.EstadoResultado> estadoResultados = new();

            var rubrosContables = (await _rubrosContablesRepository.GetRubrosContablesByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa))
                    .Where(c => c.IdEstadoRegistro != (int)Domain.Enums.EstadoRegistro.Eliminado)
                    .ToList();
            var cuentasContables = (await _cuentasContablesRepository.GetCuentasContablesByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa))
                    .Where(c => c.IdEstadoRegistro != (int)Domain.Enums.EstadoRegistro.Eliminado)
                    .ToList();

            var rubrosBalance = rubrosContables
                .Where(r => r.IdRubroPadre == default)
                .Where(r => r.CodigoRubro == ((int)Domain.Enums.RubrosContablesMaestro.EstadoDeResultados).ToString())
                .ToList();

            var item = 1;
            foreach (var rubroBalance in rubrosBalance)
            {
                List<int> IdRubros = new();
                estadoResultados.Add(new Custom.EstadoResultado
                {
                    Item = item++,
                    Rubro = rubroBalance.Descripcion,
                    CodigoIntegracion = string.Empty,
                    Descripcion = string.Empty,
                    NumeroCuenta = string.Empty,
                    Saldo = default,
                    Total = default
                });

                IdRubros.AddRange(GetRubrosHijo(rubrosContables, rubroBalance.IdRubroContable));
                var cuentasBalance = await _asientosDetalleRepository.GetEstadoResultadosByRubros(IdRubros, _permissionsBusiness.Value.User.IdEmpresa, idEjercicio, fechaHasta);
                foreach(var cuentaBalance in cuentasBalance)
                {
                    cuentaBalance.Item = item++;
                    estadoResultados.Add(cuentaBalance);
                }

                estadoResultados.Add(new Custom.EstadoResultado
                {
                    Item = item++,
                    Rubro = string.Empty,
                    CodigoIntegracion = string.Empty,
                    Descripcion = "Total Rubro",
                    NumeroCuenta = string.Empty,
                    Saldo = default,
                    Total = cuentasBalance.Sum(c => c.Saldo)
                });
            }

            return estadoResultados;
        }

        private List<int> GetRubrosHijo(List<RubroContable> rubros, int? rubroPadre)
        {
            List<int> idRubros = new();

            var rubrosHijos = rubros.Where(r => r.IdRubroPadre == rubroPadre).ToList();
            if (rubrosHijos.Count > 0)
            {
                idRubros.AddRange(rubrosHijos.Select(r => r.IdRubroContable));
                foreach (var rubro in rubrosHijos)
                {
                    idRubros.AddRange(GetRubrosHijo(rubros, rubro.IdRubroContable));
                }
            }

            return idRubros;
        }
    }
}
