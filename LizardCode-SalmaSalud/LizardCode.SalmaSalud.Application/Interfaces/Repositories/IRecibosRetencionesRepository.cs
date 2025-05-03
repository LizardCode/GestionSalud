using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IRecibosRetencionesRepository
    {
        Task<IList<TReciboRetencion>> GetAll<TReciboRetencion>(IDbTransaction transaction = null);

        Task<TReciboRetencion> GetById<TReciboRetencion>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TReciboRetencion>(TReciboRetencion entity, IDbTransaction transaction = null);

        Task<bool> Update<TReciboRetencion>(TReciboRetencion entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdRecibo(int idRecibo, IDbTransaction transaction = null);

        Task<List<ReciboRetencion>> GetAllByIdRecibo(int idRecibo);
    }
}