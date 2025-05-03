using Dapper.DataTables.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ISubdiarioPagosBusiness
    {
        Task<byte[]> GetExcel(int idOrdenPago, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<Custom.SubdiarioPagos>> GetAll(DataTablesRequest request);
        Task<List<Custom.SubdiarioPagosDetalle>> GetDetalle(int idOrdenPago, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<Custom.SubdiarioImputaciones>> DetalleImputaciones(DateTime? fechaDesde, DateTime? fechaHasta);
    }
}
