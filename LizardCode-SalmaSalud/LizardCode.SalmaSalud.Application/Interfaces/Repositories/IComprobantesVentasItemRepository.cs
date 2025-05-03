using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IComprobantesVentasItemRepository
    {
        Task<IList<ComprobanteVentaItem>> GetAllByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null);

        Task<bool> Insert(ComprobanteVentaItem entity, IDbTransaction transaction = null);

        Task<bool> Update(ComprobanteVentaItem entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null);

        Task<bool> DeleteByIdComprobanteVentaAndItem(int idComprobanteVenta, int item, IDbTransaction transaction = null);

        Task<ComprobanteVentaItem> GetByIdComprobanteVentaAndItem(int idComprobanteVenta, int item, IDbTransaction transaction = null);

        Task<List<Domain.EntitiesCustom.ComprobanteVentaItem>> GetItemsNCAnulaByFactura(int idComprobante, string sucursal, string numero, int idEmpresa, IDbTransaction transaction = null);
        Task<List<Domain.EntitiesCustom.ComprobanteVentaItem>> GetItemsByClientePaciente(int idCliente, int idEmpresa, IDbTransaction transaction = null);
        Task<List<Domain.EntitiesCustom.ComprobanteVentaItem>> GetItemsByClienteFinanciador(int idCliente, int idEmpresa, IDbTransaction transaction = null);
        Task<double> GetSaldoPrestacion(int idEvolucionPrestacion, IDbTransaction transaction = null);
        Task<double> GetSaldoOtraPrestacion(int idEvolucionOtraPrestacion, IDbTransaction transaction = null);
        Task<List<Domain.EntitiesCustom.ComprobanteVentaItem>> GetItemsCoPagoByClientePaciente(int idCliente, int idEmpresa, IDbTransaction transaction = null);
    }
}