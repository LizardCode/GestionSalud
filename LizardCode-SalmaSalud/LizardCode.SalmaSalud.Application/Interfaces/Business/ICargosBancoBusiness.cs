using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.CargosBanco;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ICargosBancoBusiness
    {
        Task<CargosBancoViewModel> Get(int idCargoBanco);
        Task<DataTablesResponse<Custom.CargoBanco>> GetAll(DataTablesRequest request);

        Task New(CargosBancoViewModel model);

        Task Remove(int idAsiento);

        Task Update(CargosBancoViewModel model);

        Task<Custom.CargoBanco> GetCustom(int idCargoBanco);

        Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id);
    }
}