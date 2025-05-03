using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ISubdiarioIVAComprasBusiness
    {
        Task<List<DetalleAlicuota>> DetalleAlicuotas(DateTime? fechaDesde, DateTime? fechaHasta, int? idProveedor);
        Task<List<ComprobanteCompraSubdiario>> GetAll(DataTablesRequest request);
        Task<string> GetCITICompras(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<string> GetCITIComprasAli(DateTime? fechaDesde, DateTime? fechaHasta);
    }
}