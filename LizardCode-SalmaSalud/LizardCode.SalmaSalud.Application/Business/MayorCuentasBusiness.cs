using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class MayorCuentasBusiness : BaseBusiness, IMayorCuentasBusiness
    {
        private readonly IAsientosRepository _asientosRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;

        public MayorCuentasBusiness(IAsientosRepository asientosRepository, IAsientosDetalleRepository asientosDetalleRepository, ICuentasContablesRepository cuentasContablesRepository)
        {
            _asientosRepository = asientosRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _cuentasContablesRepository = cuentasContablesRepository;
        }

        public async Task<List<Custom.MayorCuentas>> GetAll(DataTablesRequest request)
        {
            var filters = request.ParseFilters();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            return await _cuentasContablesRepository.GetMayorCuentas(filters);
        }

        public async Task<List<MayorCuentasDetalle>> GetMayorCuentaDetalle(int idCuentaContable, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var saldoInicio = await _asientosDetalleRepository.GetMayorCuentasDetalleSdoInicio(idCuentaContable, fechaDesde, _permissionsBusiness.Value.User.IdEmpresa);
            var mayorCuentas = await _asientosDetalleRepository.GetMayorCuentasDetalle(idCuentaContable, fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);

            mayorCuentas.Insert(0, new MayorCuentasDetalle
            {
                Descripcion = "Saldo Inicio a la Fecha:",
                Creditos = 0D,
                Debitos = 0D,
                Saldo = saldoInicio
            });

            var saldo = 0D;
            foreach(var mayorCuenta in mayorCuentas)
            {
                saldo += mayorCuenta.Debitos - mayorCuenta.Creditos;
                mayorCuenta.Saldo = saldo;
            }

            return mayorCuentas;
        }
    }
}
