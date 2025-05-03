using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Template.Application.Models.Usuarios;
using Template.Domain.Entities;

namespace Template.Application.Interfaces.Business
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
    }
}