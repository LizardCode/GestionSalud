using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.FacturacionAutomatica;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IFacturacionAutomaticaBusiness
    {
        Task<FacturacionAutomaticaViewModel> Get(int idComprobanteVenta);
        Task<DataTablesResponse<Custom.ComprobanteVenta>> GetAll(DataTablesRequest request);
        Task New(FacturacionAutomaticaViewModel model);
        Task Remove(int idComprobanteVenta);
        Task<Custom.ComprobanteVenta> GetCustom(int idComprobanteVenta);
        Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id);
        Task<Custom.DatosComprobanteVentaAFIP> ValidateComprobante(int idComprobanteVta);
        Task<Custom.DatosComprobanteVentaAFIP> ObtenerCAEComprobantesById(int idComprobanteVenta);
        Task<Custom.Percepcion> GetPercepcionesByCliente(int idCliente, int idComprobante, DateTime fecha, int idEmpresa);
        Task<List<ComprobanteVentaItem>> GetItemsFacturaByCliente(DateTime fecha, int idCliente, string idMoneda1, string idMoneda2, bool porCuentaOrden);
    }
}