using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.AnulaComprobantesVenta;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IAnulaComprobantesVentaBusiness
    {
        Task<AnulaComprobantesVentaViewModel> Get(int idComprobanteVenta);
        Task<DataTablesResponse<Custom.ComprobanteVenta>> GetAll(DataTablesRequest request);
        Task New(AnulaComprobantesVentaViewModel model);
        Task Remove(int idComprobanteVenta);
        Task<Custom.ComprobanteVenta> GetCustom(int idComprobanteVenta);
        Task<List<Custom.ComprobanteVentaItem>> GetItemsNCAnulaByFactura(int IdComprobante, string sucursal, string numero);
        Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id);
        Task<Custom.DatosComprobanteVentaAFIP> ValidateComprobante(int idComprobanteVta);
    }
}