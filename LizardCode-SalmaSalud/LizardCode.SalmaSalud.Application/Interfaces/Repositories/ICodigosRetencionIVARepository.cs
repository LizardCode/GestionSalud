using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICodigosRetencionIVARepository
    {
        Task<IList<TCodigosRetencionIVA>> GetAll<TCodigosRetencionIVA>(IDbTransaction transaction = null);

        Task<TCodigosRetencionIVA> GetById<TCodigosRetencionIVA>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TCodigosRetencionIVA>(TCodigosRetencionIVA entity, IDbTransaction transaction = null);

        Task<bool> Update<TCodigosRetencionIVA>(TCodigosRetencionIVA entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdCodigoRetencion(int idCodigoRetencion, IDbTransaction transaction = null);
    }
}