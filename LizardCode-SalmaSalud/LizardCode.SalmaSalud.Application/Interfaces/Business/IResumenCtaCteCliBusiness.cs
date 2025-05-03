using Dapper.DataTables.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IResumenCtaCteCliBusiness
    {
        Task<List<Custom.ResumenCtaCteCli>> GetAll(DataTablesRequest request);
        Task<List<Custom.ResumenCtaCteCliPendiente>> GetCtasCobrar(int idCliente, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<Custom.ResumenCtaCteCliDetalle>> GetCtaCteDetalle(int idCliente, DateTime? fechaDesde, DateTime? fechaHasta);
		Task<List<Custom.ResumenCtaCteCliPendienteGeneral>> DetalleGeneralCtasCobrar(DateTime? fechaDesde, DateTime? fechaHasta);
		Task<double> GetResumenCtaCteCliDashboard();
    }
}