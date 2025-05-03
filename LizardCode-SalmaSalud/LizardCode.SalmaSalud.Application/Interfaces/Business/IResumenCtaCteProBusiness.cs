using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IResumenCtaCteProBusiness
    {
        Task<List<Custom.ResumenCtaCtePro>> GetAll(DataTablesRequest request);
        Task<List<Custom.ResumenCtaCteProPendiente>> GetCtasPagar(int idProveedor, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<Custom.ResumenCtaCteProDetalle>> GetCtaCteDetalle(int idProveedor, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<ResumenCtaCteProPendienteGeneral>> DetalleGeneralCtasPagar(DateTime? fechaDesde, DateTime? fechaHasta);
    }
}