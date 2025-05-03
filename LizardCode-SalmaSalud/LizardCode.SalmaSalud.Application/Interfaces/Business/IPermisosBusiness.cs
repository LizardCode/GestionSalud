using Microsoft.AspNetCore.Http;
using LizardCode.SalmaSalud.Application.Models.Login;
using LizardCode.SalmaSalud.Application.Models.Permissions;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IPermisosBusiness
    {
        Usuario User { get; }
        Task SetCookie(HttpContext context, int idUser, string login, string userType);
        Task<Usuario> SignIn(HttpContext context, LoginViewModel model);
        bool CheckGeneralAccess();
        Usuario UserSession(string user);
        Task<Usuario> SignInPacientes(HttpContext context, LoginViewModel model);
    }
}