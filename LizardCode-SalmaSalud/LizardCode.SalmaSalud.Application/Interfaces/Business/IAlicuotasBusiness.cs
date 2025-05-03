using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Alicuotas;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IAlicuotasBusiness
    {
        Task<AlicuotasViewModel> Get(int idAlicuota);
        Task<DataTablesResponse<Custom.Alicuota>> GetAll(DataTablesRequest request);
        Task New(AlicuotasViewModel model);
        Task Remove(int idAlicuota);
        Task Update(AlicuotasViewModel model);
    }
}