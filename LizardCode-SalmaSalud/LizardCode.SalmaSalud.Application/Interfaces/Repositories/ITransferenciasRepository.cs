using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ITransferenciasRepository
    {
        Task<IList<TTransferencia>> GetAll<TTransferencia>(IDbTransaction transaction = null);

        Task<TTransferencia> GetById<TTransferencia>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TTransferencia>(TTransferencia entity, IDbTransaction transaction = null);

        Task<bool> Update<TTransferencia>(TTransferencia entity, IDbTransaction transaction = null);

        Task<bool> DeleteById(int idTransferencia, int idEmpresa, IDbTransaction transaction = null);
    }
}