using LizardCode.SalmaSalud.Domain.Entities;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICargosBancoAsientoRepository
    {
        Task<bool> DeleteByIdCargoBanco(int idCargoBanco, IDbTransaction transaction = null);
        
        Task<CargoBancoAsiento> GetByIdCargoBanco(int idCargoBanco, IDbTransaction transaction = null);
        
        Task<bool> Insert(CargoBancoAsiento entity, IDbTransaction transaction = null);

    }
}