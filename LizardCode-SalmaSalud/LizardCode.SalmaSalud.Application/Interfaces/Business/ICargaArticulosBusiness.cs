using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.CargaArticulos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ICargaArticulosBusiness
    {
        Task<CargaArticulosViewModel> Get(int idComprobanteCompra);
        Task<DataTablesResponse<Custom.ComprobanteCompra>> GetAll(DataTablesRequest request);
        Task New(CargaArticulosViewModel model);
        Task Remove(int idComprobanteCompra);
        Task<Custom.ComprobanteCompra> GetCustom(int idComprobanteCompra);
        Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id);
        Task<Custom.DatosComprobanteCompraAFIP> ValidateComprobante(int idComprobanteCpa);
    }
}