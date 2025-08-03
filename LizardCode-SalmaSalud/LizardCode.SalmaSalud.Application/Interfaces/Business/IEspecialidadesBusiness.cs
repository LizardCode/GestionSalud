using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Especialidades;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IEspecialidadesBusiness
    {
        Task<EspecialidadViewModel> Get(int idEspecialidad);
        Task<DataTablesResponse<Especialidades>> GetAll(DataTablesRequest request);
        Task New(EspecialidadViewModel model);
        Task Remove(int idEspecialidad);
        Task Update(EspecialidadViewModel model);
    }
}