using Dapper.DataTables.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ISubdiarioVentasBusiness
    {
        Task<byte[]> GetExcel(int idComprobanteVenta, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<Custom.SubdiarioVentas>> GetAll(DataTablesRequest request);
        Task<List<Custom.SubdiarioVentasDetalle>> GetDetalle(int idComprobanteVenta, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<Custom.SubdiarioImputaciones>> DetalleImputaciones(DateTime? fechaDesde, DateTime? fechaHasta, int? idCliente);
    }
}
