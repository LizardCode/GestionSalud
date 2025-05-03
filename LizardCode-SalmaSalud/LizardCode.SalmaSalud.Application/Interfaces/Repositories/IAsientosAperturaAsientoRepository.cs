using LizardCode.SalmaSalud.Domain.Entities;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IAsientosAperturaAsientoRepository
    {
        Task<bool> DeleteByIdAsientoApertura(int idAsientoAperturaCierre, IDbTransaction transaction = null);
        
        Task<AsientoAperturaCierreAsiento> GetByIdAsientoApertura(int idAsientoAperturaCierre, IDbTransaction transaction = null);
        
        Task<bool> Insert(AsientoAperturaCierreAsiento entity, IDbTransaction transaction = null);

    }
}