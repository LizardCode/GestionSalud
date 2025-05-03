using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IRecibosComprobantesRepository
    {
        Task<IList<TReciboComprobante>> GetAll<TReciboComprobante>(IDbTransaction transaction = null);

        Task<TReciboComprobante> GetById<TReciboComprobante>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TReciboComprobante>(TReciboComprobante entity, IDbTransaction transaction = null);

        Task<bool> Update<TReciboComprobante>(TReciboComprobante entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdRecibo(int idRecibo, IDbTransaction transaction = null);

        Task<IList<Custom.ReciboComprobante>> GetComprobantesImputar(int idCliente, string Moneda, int idEmpresa, IDbTransaction transaction = null);

        Task<IList<Custom.ReciboComprobante>> GetComprobantesByIdRecibo(int idRecibo, IDbTransaction transaction = null);

        Task<ReciboComprobante> GetByIdComprobanteVenta(int idComprobanteVta, IDbTransaction transaction = null);
    }
}