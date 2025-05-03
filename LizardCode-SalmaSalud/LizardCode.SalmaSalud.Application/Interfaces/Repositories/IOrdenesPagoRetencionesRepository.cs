using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IOrdenesPagoRetencionesRepository
    {
        Task<IList<TOrdenPagoRetencion>> GetAll<TOrdenPagoRetencion>(IDbTransaction transaction = null);

        Task<TOrdenPagoRetencion> GetById<TOrdenPagoRetencion>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TOrdenPagoRetencion>(TOrdenPagoRetencion entity, IDbTransaction transaction = null);

        Task<bool> Update<TOrdenPagoRetencion>(TOrdenPagoRetencion entity, IDbTransaction transaction = null);
        
        Task<List<OrdenPagoRetencion>> GetAllByIdOrdenPago(int idOrdenPago, IDbTransaction tran = null);

        Task<List<Custom.OrdenPagoRetencion>> GetCustomByIdOrdenPago(int idOrdenPago, IDbTransaction tran = null);

        Task<bool> DeleteByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null);
        
    }
}