using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.AnulaComprobantesCompra;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IAnulaComprobantesCompraBusiness
    {
        Task<AnulaComprobantesCompraViewModel> Get(int idComprobanteCompra);
        Task<DataTablesResponse<Custom.ComprobanteCompra>> GetAll(DataTablesRequest request);
        Task New(AnulaComprobantesCompraViewModel model);
        Task Remove(int idComprobanteCompra);
        Task<Custom.ComprobanteCompra> GetCustom(int idComprobanteCompra);
        Task<List<Custom.ComprobanteCompraItem>> GetItemsNCAnulaByFactura(int IdComprobante, string numero, int idProveedor);
        Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id);
    }
}