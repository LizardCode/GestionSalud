using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ISubdiarioIVAVentasBusiness
    {
        Task<List<DetalleAlicuota>> DetalleAlicuotas(DateTime? fechaDesde, DateTime? fechaHasta, int? idCliente);
        Task<List<ComprobanteVentaSubdiario>> GetAll(DataTablesRequest request);
        Task<string> GetCITIVentas(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<string> GetCITIVentasAli(DateTime? fechaDesde, DateTime? fechaHasta);

    }
}