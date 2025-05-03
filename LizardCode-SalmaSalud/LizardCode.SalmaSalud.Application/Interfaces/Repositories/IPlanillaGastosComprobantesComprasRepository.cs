using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPlanillaGastosComprobantesComprasRepository
    {
        Task<bool> DeleteByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null);

        Task<List<PlanillaGastoComprobanteCompra>> GetByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null);
        
        Task<bool> Insert(PlanillaGastoComprobanteCompra entity, IDbTransaction transaction = null);

    }
}