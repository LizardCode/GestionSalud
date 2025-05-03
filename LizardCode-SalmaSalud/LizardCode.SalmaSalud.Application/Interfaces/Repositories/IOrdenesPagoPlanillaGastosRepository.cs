using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IOrdenesPagoPlanillaGastosRepository
    {
        Task<IList<TOrdenPagoPlanilla>> GetAll<TOrdenPagoPlanilla>(IDbTransaction transaction = null);

        Task<TOrdenPagoPlanilla> GetById<TOrdenPagoPlanilla>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TOrdenPagoPlanilla>(TOrdenPagoPlanilla entity, IDbTransaction transaction = null);

        Task<bool> Update<TOrdenPagoPlanilla>(TOrdenPagoPlanilla entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null);

        Task<IList<Domain.EntitiesCustom.OrdenPagoPlanillaGasto>> GetPlanillasGastosByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null);
        
        Task<List<Domain.EntitiesCustom.OrdenPagoPlanillaGasto>> GetPlanillasImputar(string idMoneda, int idEmpresa, IDbTransaction transaction = null);
    }
}