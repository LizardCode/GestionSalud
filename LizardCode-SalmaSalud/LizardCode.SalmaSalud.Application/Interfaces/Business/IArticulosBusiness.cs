using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Articulos;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IArticulosBusiness
    {
        Task<ArticulosViewModel> Get(int idArticulo);
        Task<DataTablesResponse<Articulo>> GetAll(DataTablesRequest request);
        Task New(ArticulosViewModel model);
        Task Remove(int idArticulo);
        Task Update(ArticulosViewModel model);
    }
}