using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.CuentasContables;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ICuentasContablesBusiness
    {
        Task<CuentasContablesViewModel> Get(int idCuentaContable);
        Task<DataTablesResponse<Custom.CuentaContable>> GetAll(DataTablesRequest request);
        Task New(CuentasContablesViewModel model);
        Task Remove(int idCuentaContable);
        Task Update(CuentasContablesViewModel model);
    }
}