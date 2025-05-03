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
    public class ResumenCtaCteProBusiness: BaseBusiness, IResumenCtaCteProBusiness
    {
		private readonly IComprobantesComprasRepository _comprobantesComprasRepository;

		public ResumenCtaCteProBusiness(IComprobantesComprasRepository comprobantesComprasRepository)
		{
			_comprobantesComprasRepository = comprobantesComprasRepository;
		}

		public async Task<List<Custom.ResumenCtaCtePro>> GetAll(DataTablesRequest request)
		{
			var filters = request.ParseFilters();
			filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

			return await _comprobantesComprasRepository.GetResumenCtaCtePro(filters);
		}

		public async Task<List<ResumenCtaCteProPendiente>> GetCtasPagar(int idProveedor, DateTime? fechaDesde, DateTime? fechaHasta)
		{
			var saldoInicio = await _comprobantesComprasRepository.GetCtaCteProPendienteSdoInicio(idProveedor, fechaDesde, _permissionsBusiness.Value.User.IdEmpresa);
			var cuentasPendientes = await _comprobantesComprasRepository.GetCtasPagar(idProveedor, fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);

			if (cuentasPendientes == default)
				cuentasPendientes = new List<ResumenCtaCteProPendiente>();

			cuentasPendientes.Insert(0, new ResumenCtaCteProPendiente
			{
				IdComprobante = -1,
				Comprobante = "Saldo Inicio a la Fecha:",
				Sucursal = String.Empty,
				Numero = String.Empty,
				Saldo = saldoInicio
			});

			var saldoVencido = 0D;
			var saldoVencer = 0D;
			foreach (var ctacte in cuentasPendientes)
			{
				if (ctacte.IdComprobante != -1)
				{
					if (ctacte.EsCredito)
						ctacte.Saldo *= -1;

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

			cuentasPendientes.Add(new ResumenCtaCteProPendiente
			{
				IdComprobante = -1,
				Comprobante = "Total Vencido a la Fecha:",
				Sucursal = String.Empty,
				Numero = String.Empty,
				Saldo = saldoInicio + saldoVencido
			});

			cuentasPendientes.Add(new ResumenCtaCteProPendiente
			{
				IdComprobante = -1,
				Comprobante = "Total a Vencer a la Fecha:",
				Sucursal = String.Empty,
				Numero = String.Empty,
				Saldo = saldoVencer
			});

			cuentasPendientes.Add(new ResumenCtaCteProPendiente
			{
				IdComprobante = -1,
				Comprobante = "Total General:",
				Sucursal = String.Empty,
				Numero = String.Empty,
				Saldo = saldoInicio + saldoVencido + saldoVencer
			});

			return cuentasPendientes;
		}

		public async Task<List<ResumenCtaCteProDetalle>> GetCtaCteDetalle(int idProveedor, DateTime? fechaDesde, DateTime? fechaHasta)
		{
			var saldoInicio = await _comprobantesComprasRepository.GetCtaCteDetalleSdoInicio(idProveedor, fechaDesde, _permissionsBusiness.Value.User.IdEmpresa);
			var cuentasPagar = await _comprobantesComprasRepository.GetCtaCteDetalle(idProveedor, fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);

			if (cuentasPagar == default)
				cuentasPagar = new List<ResumenCtaCteProDetalle> { };

			cuentasPagar.Insert(0, new ResumenCtaCteProDetalle
			{
				IdDocumento = default,
				IdComprobante = -1,
				Comprobante = "Saldo Inicio a la Fecha:",
				Sucursal = String.Empty,
				Numero = String.Empty,
				Credito = 0D,
				Debito = 0D,
				Saldo = saldoInicio
			});

			var saldo = saldoInicio;
			foreach (var ctacte in cuentasPagar)
			{
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
							ctacte.Saldo -= saldo;
						}
						else
						{
							ctacte.Debito = ctacte.Total;
							ctacte.Credito = 0D;
							saldo += ctacte.Total;
							ctacte.Saldo += saldo;
						}
					}

				}
			}

			return cuentasPagar;
		}

		public async Task<List<ResumenCtaCteProPendienteGeneral>> DetalleGeneralCtasPagar(DateTime? fechaDesde, DateTime? fechaHasta)
		{
			var cuentasPagar = await _comprobantesComprasRepository.GetDetalleGeneralCtasPagar(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);

			if (cuentasPagar == default)
				cuentasPagar = new List<ResumenCtaCteProPendienteGeneral>();

			return cuentasPagar;
		}
	}
}
