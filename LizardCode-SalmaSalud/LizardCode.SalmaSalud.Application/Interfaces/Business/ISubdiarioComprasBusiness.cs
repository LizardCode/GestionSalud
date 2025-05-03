using Dapper.DataTables.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ISubdiarioComprasBusiness
    {
        Task<byte[]> GetExcel(int idComprobanteCompra, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<Custom.SubdiarioCompras>> GetAll(DataTablesRequest request);
        Task<List<Custom.SubdiarioComprasDetalle>> GetDetalle(int idComprobanteCompra, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<Custom.SubdiarioImputaciones>> DetalleImputaciones(DateTime? fechaDesde, DateTime? fechaHasta, int? idProveedor);
    }
}
