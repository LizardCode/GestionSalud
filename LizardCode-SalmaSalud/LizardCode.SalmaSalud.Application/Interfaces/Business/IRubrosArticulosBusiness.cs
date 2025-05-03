using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.RubrosArticulos;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IRubrosArticulosBusiness
    {
        Task<RubrosArticulosViewModel> Get(int idRubro);
        Task<DataTablesResponse<RubroArticulo>> GetAll(DataTablesRequest request);
        Task New(RubrosArticulosViewModel model);
        Task Remove(int idRubro);
        Task Update(RubrosArticulosViewModel model);
    }
}