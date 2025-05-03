using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IOrdenesPagoComprobantesRepository
    {
        Task<IList<TOrdenPagoComprobante>> GetAll<TOrdenPagoComprobante>(IDbTransaction transaction = null);

        Task<TOrdenPagoComprobante> GetById<TOrdenPagoComprobante>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TOrdenPagoComprobante>(TOrdenPagoComprobante entity, IDbTransaction transaction = null);

        Task<bool> Update<TOrdenPagoComprobante>(TOrdenPagoComprobante entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null);

        Task<OrdenPagoComprobante> GetByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null);

        Task<IList<Domain.EntitiesCustom.OrdenPagoComprobante>> GetComprobantesByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null);
        
        Task<List<Domain.EntitiesCustom.OrdenPagoComprobante>> GetComprobantesImputar(int idProveedor, string idMoneda, int idEmpresa, IDbTransaction transaction = null);

        Task<List<Domain.EntitiesCustom.OrdenPagoPlanillaGasto>> GetPlanillasImputar(string idMoneda, int idEmpresa, IDbTransaction transaction = null);

        Task<List<Domain.EntitiesCustom.OrdenPagoGrilla>> GetOrdenPagoByIdComprobanteCompra(int idComprobanteCpa);
    }
}