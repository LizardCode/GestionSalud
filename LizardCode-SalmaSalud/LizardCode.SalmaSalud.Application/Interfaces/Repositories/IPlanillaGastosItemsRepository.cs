using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPlanillaGastosItemsRepository
    {
        Task<bool> Insert(PlanillaGastoItem entity, IDbTransaction transaction = null);

        Task<bool> Update(PlanillaGastoItem entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null);

        Task<bool> DeleteByIdPlanillaGastosAndItem(int idPlanillaGastos, int item, IDbTransaction transaction = null);

        Task<PlanillaGastoItem> GetByIdPlanillaGastosAndItem(int idPlanillaGastos, int item, IDbTransaction transaction = null);

        Task<List<Domain.EntitiesCustom.PlanillaGastoItem>> GetByIdPlanillaGastos(int idPlanillaGastos, IDbTransaction transaction = null);

        //Task<double> GetImportePlanillaByItem(int item, IDbTransaction transaction = null);
    }
}