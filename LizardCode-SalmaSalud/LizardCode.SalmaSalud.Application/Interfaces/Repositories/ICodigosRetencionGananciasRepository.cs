using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICodigosRetencionGananciasRepository
    {
        Task<IList<TCodigosRetencionGanancias>> GetAll<TCodigosRetencionGanancias>(IDbTransaction transaction = null);

        Task<TCodigosRetencionGanancias> GetById<TCodigosRetencionGanancias>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TCodigosRetencionGanancias>(TCodigosRetencionGanancias entity, IDbTransaction transaction = null);

        Task<bool> Update<TCodigosRetencionGanancias>(TCodigosRetencionGanancias entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdCodigoRetencion(int idCodigoRetencion, IDbTransaction transaction = null);
    }
}