using Dapper.DataTables.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ISubdiarioCobrosBusiness
    {
        Task<byte[]> GetExcel(int idRecibo, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<Custom.SubdiarioCobros>> GetAll(DataTablesRequest request);
        Task<List<Custom.SubdiarioCobrosDetalle>> GetDetalle(int idRecibo, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<List<Custom.SubdiarioImputaciones>> DetalleImputaciones(DateTime? fechaDesde, DateTime? fechaHasta);
    }
}
