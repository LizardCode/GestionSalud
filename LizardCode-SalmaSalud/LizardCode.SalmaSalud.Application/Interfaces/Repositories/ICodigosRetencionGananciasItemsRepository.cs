using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICodigosRetencionGananciasItemsRepository
    {
        Task<IList<Domain.Entities.CodigosRetencionGananciasItems>> GetAllByIdCodigoRetencion(int idCodigoRetencion, IDbTransaction transaction = null);

        Task<bool> Insert(Domain.Entities.CodigosRetencionGananciasItems entity, IDbTransaction transaction = null);

        Task<bool> Update(Domain.Entities.CodigosRetencionGananciasItems entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdCodigoRetencion(int idCodigoRetencion, IDbTransaction transaction = null);

        Task<bool> DeleteByIdCodigoRetencionAndItem(int idCodigoRetencion, int item, IDbTransaction transaction = null);

        Task<Domain.Entities.CodigosRetencionGananciasItems> GetByIdCodigoRetencionAndItem(int idCodigoRetencion, int item, IDbTransaction transaction = null);

        Task<Domain.Entities.CodigosRetencionGananciasItems> GetByImporteDesdeHasta(int idCodigoRetencion, double baseRetencionGanancias, IDbTransaction tran = null);
    }
}