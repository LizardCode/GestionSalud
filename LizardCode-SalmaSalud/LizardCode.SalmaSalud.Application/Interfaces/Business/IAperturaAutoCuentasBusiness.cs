using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.AperturaAutoCuentas;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IAperturaAutoCuentasBusiness
    {
        Task<AperturaAutoCuentasViewModel> Get(int idAsientoApertura);
        Task<Custom.AsientoAperturaCierre> GetCustom(int idAsientoApertura);
        Task<DataTablesResponse<Custom.AsientoAperturaCierre>> GetAll(DataTablesRequest request);
        Task New(AperturaAutoCuentasViewModel model);
        Task Remove(int idAsientoApertura);
        Task Update(AperturaAutoCuentasViewModel model);
    }
}