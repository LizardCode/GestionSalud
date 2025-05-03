using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.CondicionVentasCompras;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ICondicionVentasComprasBusiness
    {
        Task<CondicionVentasComprasViewModel> Get(int idCondicion);
        Task<DataTablesResponse<Custom.CondicionVentaCompra>> GetAll(DataTablesRequest request);
        Task New(CondicionVentasComprasViewModel model);
        Task Remove(int idCondicion);
        Task Update(CondicionVentasComprasViewModel model);
    }
}