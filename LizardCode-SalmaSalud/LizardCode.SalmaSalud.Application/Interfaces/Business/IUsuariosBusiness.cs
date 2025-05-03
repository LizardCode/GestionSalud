using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Http;
using LizardCode.SalmaSalud.Application.Models.Usuarios;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IUsuariosBusiness
    {
        Task Update(UsuarioViewModel model);
        Task Blank(int idUser);
        Task Remove(int idUser);
        Task New(UsuarioViewModel model);
        Task<UsuarioViewModel> Get(int idUser);
        Task<DataTablesResponse<Usuario>> GetAll(DataTablesRequest request);
        Task ResetPassword(HttpContext context, string login, string pass, string newPass, string repeatPass);
        Task<bool> CheckLogin(string login);
        Task<DataTablesResponse<Domain.EntitiesCustom.AuditoriaLogin>> GetAllAuditoriaLogin(DataTablesRequest request);
        Task<string> RequestAccessCode(string documento);
    }
}