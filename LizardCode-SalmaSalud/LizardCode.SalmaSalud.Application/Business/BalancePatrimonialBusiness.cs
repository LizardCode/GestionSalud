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
    public class BalancePatrimonialBusiness : BaseBusiness, IBalancePatrimonialBusiness
    {
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly IRubrosContablesRepository _rubrosContablesRepository;

        public BalancePatrimonialBusiness(
            IAsientosRepository asientosRepository, 
            IAsientosDetalleRepository asientosDetalleRepository,
            IRubrosContablesRepository rubrosContablesRepository,
            ICuentasContablesRepository cuentasContablesRepository)
        {
            _asientosRepository = asientosRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _cuentasContablesRepository = cuentasContablesRepository;
            _rubrosContablesRepository = rubrosContablesRepository;
        }

        public async Task<List<Custom.BalancePatrimonial>> GetAll(DataTablesRequest request)
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

            List<Custom.BalancePatrimonial> balancePatrimonial = new();

            var rubrosContables = (await _rubrosContablesRepository.GetRubrosContablesByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa))
                    .Where(c => c.IdEstadoRegistro != (int)Domain.Enums.EstadoRegistro.Eliminado)
                    .ToList();
            var cuentasContables = (await _cuentasContablesRepository.GetCuentasContablesByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa))
                    .Where(c => c.IdEstadoRegistro != (int)Domain.Enums.EstadoRegistro.Eliminado)
                    .ToList();

            var rubrosBalance = rubrosContables
                .Where(r => r.IdRubroPadre == default)
                .ToList();

            var item = 1;
            foreach (var rubroBalance in rubrosBalance)
            {
                List<int> IdRubros = new();
                balancePatrimonial.Add(new Custom.BalancePatrimonial
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
                var cuentasBalance = await _asientosDetalleRepository.GetBalanceCuentasByRubros(IdRubros, _permissionsBusiness.Value.User.IdEmpresa, idEjercicio, fechaHasta);
                foreach(var cuentaBalance in cuentasBalance)
                {
                    cuentaBalance.Item = item++;
                    balancePatrimonial.Add(cuentaBalance);
                }

                balancePatrimonial.Add(new Custom.BalancePatrimonial
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

            return balancePatrimonial;
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
