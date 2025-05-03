using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Bancos;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IBancosBusiness
    {
        Task<BancosViewModel> Get(int idBanco);
        Task<DataTablesResponse<Banco>> GetAll(DataTablesRequest request);
        Task New(BancosViewModel model);
        Task Remove(int idBanco);
        Task Update(BancosViewModel model);
    }
}