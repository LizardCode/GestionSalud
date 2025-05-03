using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Monedas;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.EntitiesCustom.Dashboard;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IMonedasBusiness
    {
        Task<MonedasViewModel> Get(int idMoneda);
        Task<DataTablesResponse<Moneda>> GetAll(DataTablesRequest request);
        Task<DataTablesResponse<TipoDeCambio>> GetTiposDeCambio(DataTablesRequest request);
        Task New(MonedasViewModel model);
        Task Remove(int idMoneda);
        Task Update(MonedasViewModel model);
    }
}