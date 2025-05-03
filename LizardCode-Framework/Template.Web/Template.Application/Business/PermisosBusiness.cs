using LizardCode.Framework.Aplication.Common.Enums;
using LizardCode.Framework.Aplication.Common.Exceptions;
using LizardCode.Framework.Aplication.Helpers;
using LizardCode.Framework.Aplication.Interfaces.Business;
using LizardCode.Framework.Aplication.Models.Permissions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Template.Application.Interfaces.Repositories;

namespace Template.Application.Business
{
    public class PermisosBusiness : IPermisosBusiness
    {
        private readonly IUsuariosRepository _usersRepository;

        public Usuario User => DetectUser();


        public PermisosBusiness(IUsuariosRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }


        public async Task<Usuario> SignIn(HttpContext context, string login, string pass)
        {
            if (login.IsNull() || pass.IsNull())
            {
                throw new BusinessException(nameof(login));
            }

            login = login.Trim().ToLower();

            var users = await _usersRepository.GetAll<Domain.Entities.Usuario>();
            var user = users
                .FirstOrDefault(user => user.Login == login);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if (!user.BlankToken.IsNull())
            {
                throw new UserNotActivatedException();
            }

            if (!user.Password.Equals(Cryptography.HashPassword(pass, Convert.FromBase64String(user.PasswordSalt))))
            {
                throw new LoginFailedException();
            }

            if (!user.Admin && !CheckGeneralAccess())
            {
                throw new PermissionException(PermisoError.SinPermiso);
            }

            if (user.Vencimiento < DateTime.Now)
            {
                throw new PasswordExpiredException();
            }

            var sessionUser = new Usuario(
                id: user.IdUsuario,
                nombre: user.Nombre,
                login: user.Login,
                idTipoUsuario: user.IdTipoUsuario,
                admin: user.Admin
            );

            var userCookie = $"{user.IdUsuario}|{user.Login}";

            context.Session.SetString($"TemplateWeb.{userCookie}", JsonConvert.SerializeObject(sessionUser));

            await SetCookie(context, user.IdUsuario, user.Login, "ADMIN");

            return sessionUser;
        }

        public async Task SetCookie(HttpContext context, int idUser, string login, string idUserType)
        {
            var userCookie = $"{idUser}|{login}";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, login),
                new Claim(ClaimTypes.Role, idUserType),
                new Claim(ClaimTypes.UserData, userCookie)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var properties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(8)
            };

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties);
        }


        public bool CheckGeneralAccess()
        {
            //TODO: Agregar lógica para permisos
            return true;
        }


        private Usuario DetectUser()
        {
            var user = HttpContextHelper.Current.User?.Claims
                .FirstOrDefault(f => f.Type.Equals(ClaimTypes.UserData))
                ?.Value;

            if (user.IsNull())
            {
                return null;
            }

            return $"TemplateWeb.{user}".FromSession<Usuario>();
        }
    }
}
