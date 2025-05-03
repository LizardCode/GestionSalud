using Microsoft.AspNetCore.Http;

namespace LizardCode.Framework.Application.Interfaces.Business
{
    public interface IPermisosBusiness
    {
        IPermisoUsuario User { get; }

        Task SetCookie(HttpContext context, int idUser, string login, string userType);
        Task<IPermisoUsuario> SignIn(HttpContext context, string login, string pass);
        bool CheckGeneralAccess();
    }
}