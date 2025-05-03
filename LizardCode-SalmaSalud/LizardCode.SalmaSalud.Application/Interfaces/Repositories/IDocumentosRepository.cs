using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IDocumentosRepository
    {
        Task<IList<TDocumento>> GetAll<TDocumento>(IDbTransaction transaction = null);

        Task<TDocumento> GetById<TDocumento>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TDocumento>(TDocumento entity, IDbTransaction transaction = null);

        Task<bool> Update<TDocumento>(TDocumento entity, IDbTransaction transaction = null);

        Task<bool> DeleteById(int idDocumento, int idEmpresa, IDbTransaction transaction = null);
    }
}