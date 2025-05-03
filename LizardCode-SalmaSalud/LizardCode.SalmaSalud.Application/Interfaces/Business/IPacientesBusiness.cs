using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Pacientes;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IPacientesBusiness
    {
        Task<PacienteViewModel> Get(int idPaciente);
        Task<DataTablesResponse<Custom.Paciente>> GetAll(DataTablesRequest request);
        Task New(PacienteViewModel model);
        Task Remove(int idPaciente);
        Task Update(PacienteViewModel model);
        Task<string> ValidarNroCUIT(string cuit, int? idPaciente);
        Task<string> ValidarNroDocumento(string documento, int? idPaciente);
        Task<Paciente> GetLikeDocument(string documento);
        Task<Paciente> GetLikePhone(string phone);
        Task<Paciente> GetCustomById(int idPaciente);
        Task UpdateFromPortal(PacienteViewModel model);
        Task ADMIN_CreateClientes();
        Task ADMIN_CreateUsuarios();
        Task<string> ValidarNroFinanciador(string financiadorNro, int? idCliente);
    }
}
