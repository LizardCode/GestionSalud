using LizardCode.SalmaSalud.Application.Models.PlanCuentas;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IPlanCuentasBusiness
    {
        Task<PlanCuentasViewModel> Get();
        Task<PlanCuentasViewModel> GetCustom(int id);
    }
}