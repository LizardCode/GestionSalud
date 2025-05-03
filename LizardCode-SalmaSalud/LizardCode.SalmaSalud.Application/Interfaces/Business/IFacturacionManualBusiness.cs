using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.FacturacionManual;
using LizardCode.SalmaSalud.Domain.EntitiesCustom.Dashboard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IFacturacionManualBusiness
    {
        Task<FacturacionManualViewModel> Get(int idComprobanteVenta);
        Task<DataTablesResponse<Custom.ComprobanteVenta>> GetAll(DataTablesRequest request);
        Task New(FacturacionManualViewModel model);
        Task Remove(int idComprobanteVenta);
        Task<Custom.ComprobanteVenta> GetCustom(int idComprobanteVenta);
        Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id);
        Task<Custom.DatosComprobanteVentaAFIP> ValidateComprobante(int idComprobanteVenta);
        Task<Custom.Percepcion> GetPercepcionesByCliente(int idCliente, int idComprobante, DateTime fecha, int idEmpresa);
        Task<decimal> GetIVADashboard();
        Task<int> GetContidadFacturasCompra();
        Task<int> GetContidadFacturasVenta();
        Task<Custom.DatosComprobanteVentaAFIP> ObtenerCAEComprobantesById(int idComprobanteVta);
        Task<int> GetCantidadFacturasComprasProveedor();
        Task<int> GetCantidadFacturasComprasPagasProveedor();
        Task<DataTablesResponse<FacturaCompra>> GetUltimasFacturasProveedor(DataTablesRequest request);
    }
}