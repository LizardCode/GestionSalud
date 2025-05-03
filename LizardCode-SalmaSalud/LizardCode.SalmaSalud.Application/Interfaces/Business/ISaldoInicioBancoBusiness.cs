using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.SaldoInicioBanco;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ISaldoInicioBancoBusiness
    {
        Task<SaldoInicioBancoViewModel> Get(int idSaldoInicioBanco);
        Task<DataTablesResponse<Custom.SaldoInicioBanco>> GetAll(DataTablesRequest request);
        Task New(SaldoInicioBancoViewModel model);
        Task Remove(int idSaldoInicioBanco);
        Task<Custom.SaldoInicioBanco> ObtenerDetalleSaldoInicio(int id);
    }
}
