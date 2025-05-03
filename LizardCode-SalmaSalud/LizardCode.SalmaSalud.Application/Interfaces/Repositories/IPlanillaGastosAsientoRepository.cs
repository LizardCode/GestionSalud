using LizardCode.SalmaSalud.Domain.Entities;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPlanillaGastosAsientoRepository
    {
        Task<bool> DeleteByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null);

        Task<PlanillaGastoAsiento> GetByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null);

        Task<bool> Insert(PlanillaGastoAsiento entity, IDbTransaction transaction = null);

    }
}