using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ISdoCtaCteCliComprobantesVentasRepository
    {
        Task<IList<SdoCtaCteCliComprobantesVentas>> GetAll<SdoCtaCteCliComprobantesVentas>(IDbTransaction transaction = null);

        Task<SdoCtaCteCliComprobantesVentas> GetById<SdoCtaCteCliComprobantesVentas>(int id, IDbTransaction transaction = null);

        Task<long> Insert<SdoCtaCteCliComprobantesVentas>(SdoCtaCteCliComprobantesVentas entity, IDbTransaction transaction = null);

        Task<bool> Update<SdoCtaCteCliComprobantesVentas>(SdoCtaCteCliComprobantesVentas entity, IDbTransaction transaction = null);

        Task<bool> RemoveAllByIdSdoCtaCteCli(int idSdoCtaCteCli, IDbTransaction transaction = null);
    }
}
