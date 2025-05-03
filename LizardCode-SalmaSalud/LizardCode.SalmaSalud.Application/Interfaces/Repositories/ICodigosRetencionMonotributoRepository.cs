using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICodigosRetencionMonotributoRepository
    {
        Task<IList<TCodigosRetencionMonotributo>> GetAll<TCodigosRetencionMonotributo>(IDbTransaction transaction = null);

        Task<TCodigosRetencionMonotributo> GetById<TCodigosRetencionMonotributo>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TCodigosRetencionMonotributo>(TCodigosRetencionMonotributo entity, IDbTransaction transaction = null);

        Task<bool> Update<TCodigosRetencionMonotributo>(TCodigosRetencionMonotributo entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdCodigoRetencion(int idCodigoRetencion, IDbTransaction transaction = null);
    }
}