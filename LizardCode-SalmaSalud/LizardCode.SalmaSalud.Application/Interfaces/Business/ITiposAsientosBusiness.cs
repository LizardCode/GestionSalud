using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.TiposAsientos;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ITiposAsientosBusiness
    {
        Task<TiposAsientosViewModel> Get(int idTipoAsiento);
        Task<DataTablesResponse<TipoAsiento>> GetAll(DataTablesRequest request);
        Task<TiposAsientosViewModel> GetCustom(int idTipoAsiento);
        Task New(TiposAsientosViewModel model);
        Task Remove(int idTipoAsiento);
        Task Update(TiposAsientosViewModel model);
    }
}