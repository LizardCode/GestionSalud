using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Feriados;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IFeriadosBusiness
    {
        Task<FeriadoViewModel> Get(int idFeriado);
        Task<DataTablesResponse<Custom.Feriado>> GetAll(DataTablesRequest request);
        Task New(FeriadoViewModel model);
        Task Remove(int idFeriado);
        Task Update(FeriadoViewModel model);
    }
}
