using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICodigosRetencionSussRepository
    {
        Task<IList<TCodigosRetencionSuss>> GetAll<TCodigosRetencionSuss>(IDbTransaction transaction = null);

        Task<TCodigosRetencionSuss> GetById<TCodigosRetencionSuss>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TCodigosRetencionSuss>(TCodigosRetencionSuss entity, IDbTransaction transaction = null);

        Task<bool> Update<TCodigosRetencionSuss>(TCodigosRetencionSuss entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdCodigoRetencion(int idCodigoRetencion, IDbTransaction transaction = null);
    }
}