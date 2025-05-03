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
    public class ResumenCtaCteCliBusiness: BaseBusiness, IResumenCtaCteCliBusiness
    {
		private readonly IComprobantesVentasRepository _comprobantesVentasRepository;

		public ResumenCtaCteCliBusiness(IComprobantesVentasRepository comprobantesVentasRepository)
		{
			_comprobantesVentasRepository = comprobantesVentasRepository;
		}

		public async Task<List<Custom.ResumenCtaCteCli>> GetAll(DataTablesRequest request)
		{
			var filters = request.ParseFilters();
			filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

			return await _comprobantesVentasRepository.GetResumenCtaCteCli(filters);
		}

		public async Task<List<ResumenCtaCteCliDetalle>> GetCtaCteDetalle(int idCliente, DateTime? fechaDesde, DateTime? fechaHasta)
		{
			var saldoInicio = await _comprobantesVentasRepository.GetCtaCteDetalleSdoInicio(idCliente, fechaDesde, _permissionsBusiness.Value.User.IdEmpresa);
			var cuentasCobrar = await _comprobantesVentasRepository.GetCtaCteDetalle(idCliente, fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);

			if (cuentasCobrar == default)
				cuentasCobrar = new List<ResumenCtaCteCliDetalle>();

			cuentasCobrar.Insert(0, new ResumenCtaCteCliDetalle
			{
				IdDocumento = default,
				IdTipo = default,
				IdComprobante = -1,
				Comprobante = "Saldo Inicio a la Fecha:",
				Sucursal = String.Empty,
				Numero = String.Empty,
				Credito = 0D,
				Debito = 0D,
				Saldo = saldoInicio
			});

			var saldo = 0D;
			foreach (var ctacte in cuentasCobrar)
			{
				if (ctacte.IdEjercicio == default)
					ctacte.IdDocumento = default;

				if (ctacte.IdComprobante != -1)
				{
					if (ctacte.IdComprobante == 0)
					{
						ctacte.Debito = 0D;
						ctacte.Credito = ctacte.Total;
						saldo -= ctacte.Total;
						ctacte.Saldo = saldo;
					}
					else
					{
						if (ctacte.EsCredito)
						{
							ctacte.Debito = 0D;
							ctacte.Credito = ctacte.Total;
							saldo -= ctacte.Total;
							ctacte.Saldo = saldo;
						}
						else
						{
							ctacte.Debito = ctacte.Total;
							ctacte.Credito = 0D;
							saldo += ctacte.Total;
							ctacte.Saldo = saldo;
						}
					}
				}
			}

			return cuentasCobrar;
		}

		public async Task<List<ResumenCtaCteCliPendiente>> GetCtasCobrar(int idCliente, DateTime? fechaDesde, DateTime? fechaHasta)
		{
			var saldoInicio = await _comprobantesVentasRepository.GetCtaCteCliPendienteSdoInicio(idCliente, fechaDesde, _permissionsBusiness.Value.User.IdEmpresa);
			var cuentasPendientes = await _comprobantesVentasRepository.GetCtasCobrar(idCliente, fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);

			if (cuentasPendientes == default)
				cuentasPendientes = new List<ResumenCtaCteCliPendiente>();

			cuentasPendientes.Insert(0, new ResumenCtaCteCliPendiente
			{
				IdComprobanteVenta = default,
				IdTipoComprobante = default,
				IdComprobante = -1,
				Comprobante = "Saldo Inicio a la Fecha:",
				Sucursal = String.Empty,
				Numero = String.Empty,
				Saldo = saldoInicio,
			});

			var saldoAcumulado = 0D;
			var saldoVencido = 0D;
			var saldoVencer = 0D;

			foreach (var ctacte in cuentasPendientes)
			{
				if (ctacte.IdEjercicio == default)
					ctacte.IdComprobanteVenta = default;

				if (ctacte.IdComprobante != -1)
				{
					if (ctacte.EsCredito)
						ctacte.Saldo *= -1;
					saldoAcumulado += ctacte.Saldo;

					ctacte.SaldoAcumulado = saldoAcumulado;

					if (ctacte.FechaVto.HasValue)
					{
						if (ctacte.FechaVto.Value < DateTime.Now)
							saldoVencido += ctacte.Saldo;
						else
							saldoVencer += ctacte.Saldo;
					}
					else
						saldoVencer += ctacte.Saldo;
				}
			}

			cuentasPendientes.Add(new ResumenCtaCteCliPendiente
			{
				IdComprobanteVenta = default,
				IdTipoComprobante = default,
				IdComprobante = -1,
				Comprobante = "Total Vencido a la Fecha:",
				Sucursal = String.Empty,
				Numero = String.Empty,
				Saldo = saldoInicio + saldoVencido
			});

			cuentasPendientes.Add(new ResumenCtaCteCliPendiente
			{
				IdComprobanteVenta = default,
				IdTipoComprobante = default,
				IdComprobante = -1,
				Comprobante = "Total a Vencer a la Fecha:",
				Sucursal = String.Empty,
				Numero = String.Empty,
				Saldo = saldoVencer
			});

			cuentasPendientes.Add(new ResumenCtaCteCliPendiente
			{
				IdComprobanteVenta = default,
				IdTipoComprobante = default,
				IdComprobante = -1,
				Comprobante = "Total General:",
				Sucursal = String.Empty,
				Numero = String.Empty,
				Saldo = saldoInicio + saldoVencido + saldoVencer
			});

			return cuentasPendientes;
		}

		public async Task<double> GetResumenCtaCteCliDashboard()
		{
			var importe = await _comprobantesVentasRepository.GetResumenCtaCteCliDashboard(_permissionsBusiness.Value.User.IdEmpresa);

			return importe;
		}

		public async Task<List<ResumenCtaCteCliPendienteGeneral>> DetalleGeneralCtasCobrar(DateTime? fechaDesde, DateTime? fechaHasta)
		{
			var cuentasPendientes = await _comprobantesVentasRepository.GetDetalleGeneralCtasCobrar(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);

			if (cuentasPendientes == default)
				cuentasPendientes = new List<ResumenCtaCteCliPendienteGeneral>();

			return cuentasPendientes;

		}
	}
}
