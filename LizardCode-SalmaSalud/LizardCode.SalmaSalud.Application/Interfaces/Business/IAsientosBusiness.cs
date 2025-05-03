using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Asientos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IAsientosBusiness
    {
        Task<AsientosViewModel> Get(int idAsiento);
        Task<DataTablesResponse<Custom.Asiento>> GetAll(DataTablesRequest request);
        Task New(AsientosViewModel model);
        Task Remove(int idAsiento);
        Task Update(AsientosViewModel model);
        Task<Custom.Asiento> GetCustom(int idAsiento);
        Task<List<AsientosDetalle>> GetCuentasByTipoAsiento(int idTipoAsiento);
    }
}