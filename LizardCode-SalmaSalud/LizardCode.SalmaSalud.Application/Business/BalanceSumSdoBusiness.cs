using Dapper.DataTables.Models;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class BalanceSumSdoBusiness : BaseBusiness, IBalanceSumSdoBusiness
    {
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly IRubrosContablesRepository _rubrosContablesRepository;

        public BalanceSumSdoBusiness(
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

        public async Task<List<Custom.BalanceSumSdo>> GetAll(DataTablesRequest request)
        {
            var filters = request.ParseFilters();
            var idEjercicio = 0;
            int? idCuentaDesde = default;
            int? idCuentaHasta = default;
            DateTime fechaHasta;
            DateTime fechaDesde;

            if (!filters.ContainsKey("IdEjercicio"))
                throw new BusinessException("Seleccione un Ejercicio");
            else
                idEjercicio = int.Parse(filters["IdEjercicio"].ToString());

            if (filters.ContainsKey("IdCuentaDesde"))
                idCuentaDesde = int.Parse(filters["IdCuentaDesde"].ToString());

            if (filters.ContainsKey("IdCuentaHasta"))
                idCuentaHasta = int.Parse(filters["IdCuentaHasta"].ToString());

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                fechaDesde = DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null);
            else
                throw new BusinessException("Ingrese Fecha Desde");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
                fechaHasta = DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null);
            else
                throw new BusinessException("Ingrese Fecha Hasta");

            var cuentasBalance = await _asientosDetalleRepository.GetBalanceSumSdoByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa, idEjercicio, fechaDesde, fechaHasta, idCuentaDesde, idCuentaHasta);

            List<Custom.BalanceSumSdo> balanceSumaSaldo = new();

            var item = 1;
            foreach (var cuenta in cuentasBalance)
            {
                balanceSumaSaldo.Add(new Custom.BalanceSumSdo
                {
                    Item = item++,
                    IdCuentaContable = cuenta.IdCuentaContable,
                    CodigoIntegracion = cuenta.CodigoIntegracion,
                    Descripcion = cuenta.Descripcion,
                    Debe = cuenta.Debe,
                    Haber = cuenta.Haber,
                    Acredor = cuenta.Debe > cuenta.Haber ? cuenta.Debe - cuenta.Haber : 0,
                    Deudor = cuenta.Debe < cuenta.Haber ? cuenta.Haber - cuenta.Debe : 0
                });
            }

            balanceSumaSaldo.Add(new Custom.BalanceSumSdo
            {
                Item = item++,
                IdCuentaContable = default,
                CodigoIntegracion = "",
                Descripcion = "Total",
                Debe = balanceSumaSaldo.Sum(s => s.Debe),
                Haber = balanceSumaSaldo.Sum(s => s.Haber),
                Acredor = balanceSumaSaldo.Sum(s => s.Acredor),
                Deudor = balanceSumaSaldo.Sum(s => s.Deudor)
            });
            
            return balanceSumaSaldo;
        }
    }
}
