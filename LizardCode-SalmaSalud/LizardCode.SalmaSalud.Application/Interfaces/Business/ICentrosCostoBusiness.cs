using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.CentrosCosto;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ICentrosCostoBusiness
    {
        Task<CentrosCostoViewModel> Get(int idCentroCosto);
        Task<DataTablesResponse<CentroCosto>> GetAll(DataTablesRequest request);
        Task New(CentrosCostoViewModel model);
        Task Remove(int idCentroCosto);
        Task Update(CentrosCostoViewModel model);
    }
}