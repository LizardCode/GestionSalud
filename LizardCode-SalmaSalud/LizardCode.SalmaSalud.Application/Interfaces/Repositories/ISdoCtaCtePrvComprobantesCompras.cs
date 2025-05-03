using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ISdoCtaCtePrvComprobantesComprasRepository
    {
        Task<IList<SdoCtaCtePrvComprobantesCompras>> GetAll<SdoCtaCtePrvComprobantesCompras>(IDbTransaction transaction = null);

        Task<SdoCtaCtePrvComprobantesCompras> GetById<SdoCtaCtePrvComprobantesCompras>(int id, IDbTransaction transaction = null);

        Task<long> Insert<SdoCtaCtePrvComprobantesCompras>(SdoCtaCtePrvComprobantesCompras entity, IDbTransaction transaction = null);

        Task<bool> Update<SdoCtaCtePrvComprobantesCompras>(SdoCtaCtePrvComprobantesCompras entity, IDbTransaction transaction = null);

        Task<bool> RemoveAllByIdSdoCtaCtePrv(int idSdoCtaCtePrv, IDbTransaction transaction = null);
    }
}
