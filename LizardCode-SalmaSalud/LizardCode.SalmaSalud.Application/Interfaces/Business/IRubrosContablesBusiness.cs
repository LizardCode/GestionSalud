using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.RubrosContables;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IRubrosContablesBusiness
    {
        Task<RubrosContablesViewModel> Get(int idRubroContable);
        Task<DataTablesResponse<Custom.RubroContable>> GetAll(DataTablesRequest request);
        Task<List<Custom.Select2Custom>> GetRubrosContables(string q);
        Task New(RubrosContablesViewModel model);
        Task Remove(int idRubroContable);
        Task Update(RubrosContablesViewModel model);
    }
}