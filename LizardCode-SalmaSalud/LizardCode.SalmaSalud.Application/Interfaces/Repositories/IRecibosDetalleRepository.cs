using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IRecibosDetalleRepository
    {
        Task<IList<TReciboDetalle>> GetAll<TReciboDetalle>(IDbTransaction transaction = null);

        Task<TReciboDetalle> GetById<TReciboDetalle>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TReciboDetalle>(TReciboDetalle entity, IDbTransaction transaction = null);

        Task<bool> Update<TReciboDetalle>(TReciboDetalle entity, IDbTransaction transaction = null);

        Task<List<Custom.ReciboDetalle>> GetAllByIdRecibo(int idRecibo, IDbTransaction transaction = null);

        Task<bool> DeleteByIdRecibo(int idRecibo, IDbTransaction transaction = null);
    }
}