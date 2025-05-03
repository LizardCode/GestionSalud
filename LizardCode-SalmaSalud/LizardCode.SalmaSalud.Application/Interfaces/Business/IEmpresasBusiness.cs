using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Empresas;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IEmpresasBusiness
    {
        Task<EmpresaViewModel> Get(int idEmpresa);
        Task<DataTablesResponse<Custom.Empresa>> GetAll(DataTablesRequest request);
        Task New(EmpresaViewModel model);
        Task Remove(int idEmpresa);
        Task Update(EmpresaViewModel model);
        Task<List<Empresa>> GetAllByIdUsuario(int idUsuario);
        Task<Empresa> GetEmpresaById(int id);
        Task UploadCertificate(int idEmpresa, string crt);
        Task CopiarPlanCtas(int idEmpresaDestino, int idEmpresaOrigen);
    }
}