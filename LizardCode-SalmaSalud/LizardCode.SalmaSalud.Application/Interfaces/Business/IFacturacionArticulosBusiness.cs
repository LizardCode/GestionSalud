using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.FacturacionArticulos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IFacturacionArticulosBusiness
    {
        Task<FacturacionArticulosViewModel> Get(int idComprobanteVenta);
        Task<DataTablesResponse<Custom.ComprobanteVenta>> GetAll(DataTablesRequest request);
        Task New(FacturacionArticulosViewModel model);
        Task Remove(int idComprobanteVenta);
        Task<Custom.ComprobanteVenta> GetCustom(int idComprobanteVenta);
        Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id);
        Task<Custom.DatosComprobanteVentaAFIP> ValidateComprobante(int idComprobanteVenta);
        Task<Custom.DatosComprobanteVentaAFIP> ObtenerCAEComprobantesById(int idComprobanteVenta);
        Task<Custom.Percepcion> GetPercepcionesByCliente(int idCliente, int idComprobante, DateTime fecha, int idEmpresa);
        Task<double> GetPrecio(int idArticulo, int idEmpresa);
    }
}