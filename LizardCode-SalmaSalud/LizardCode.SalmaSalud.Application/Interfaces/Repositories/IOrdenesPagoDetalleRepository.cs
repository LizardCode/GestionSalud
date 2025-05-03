using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IOrdenesPagoDetalleRepository
    {
        Task<IList<TOrdenPagoDetalle>> GetAll<TOrdenPagoDetalle>(IDbTransaction transaction = null);

        Task<TOrdenPagoDetalle> GetById<TOrdenPagoDetalle>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TOrdenPagoDetalle>(TOrdenPagoDetalle entity, IDbTransaction transaction = null);

        Task<bool> Update<TOrdenPagoDetalle>(TOrdenPagoDetalle entity, IDbTransaction transaction = null);

        Task<IList<OrdenPagoDetalle>> GetAllByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null);

        Task<bool> DeleteByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null);
    }
}