using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IComprobantesComprasItemRepository
    {
        Task<IList<Custom.ComprobanteCompraManualItem>> GetAllByIdComprobanteCompraManual(int idComprobanteCompra, IDbTransaction transaction = null);

        Task<IList<Custom.ComprobanteCompraItem>> GetAllByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null);

        Task<bool> Insert(ComprobanteCompraItem entity, IDbTransaction transaction = null);

        Task<bool> Update(ComprobanteCompraItem entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null);

        Task<bool> DeleteByIdComprobanteCompraAndItem(int idComprobanteCompra, int item, IDbTransaction transaction = null);

        Task<ComprobanteCompraItem> GetByIdComprobanteCompraAndItem(int idComprobanteCompra, int item, IDbTransaction transaction = null);
        
        Task<List<Custom.ComprobanteCompraItem>> GetItemsNCAnulaByFactura(int idComprobante, string numeroComprobante, int idProveedor, int idEmpresa, IDbTransaction transaction = null);
    }
}