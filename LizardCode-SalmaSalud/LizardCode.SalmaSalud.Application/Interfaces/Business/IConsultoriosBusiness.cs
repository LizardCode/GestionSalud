using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Consultorios;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IConsultoriosBusiness
    {
        Task<ConsultorioViewModel> Get(int idConsultorio);
        Task<DataTablesResponse<Consultorio>> GetAll(DataTablesRequest request);
        Task New(ConsultorioViewModel model);
        Task Remove(int idConsultorio);
        Task Update(ConsultorioViewModel model);
    }
}
