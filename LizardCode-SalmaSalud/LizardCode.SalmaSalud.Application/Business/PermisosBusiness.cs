using LizardCode.Framework.Application.Helpers;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using LizardCode.Framework.Application.Common.Enums;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Login;
using LizardCode.SalmaSalud.Application.Models.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class PermisosBusiness : IPermisosBusiness
    {
        private readonly IUsuariosRepository _usersRepository;
        private readonly IEmpresasRepository _empresasRepository;
        private readonly IAuditoriaLoginRepository _auditoriaLoginRepository;
        private readonly IProfesionalesRepository _profesionalesRepository;

        public Usuario User => DetectUser();

        public PermisosBusiness(IUsuariosRepository usersRepository, IEmpresasRepository empresasRepository, IAuditoriaLoginRepository auditoriaLoginRepository, IProfesionalesRepository profesionalesRepository)
        {
            _usersRepository = usersRepository;
            _empresasRepository = empresasRepository;
            _auditoriaLoginRepository = auditoriaLoginRepository;
            _profesionalesRepository = profesionalesRepository;

        }

        public async Task<Usuario> SignIn(HttpContext context, LoginViewModel model)
        {
            var user = await ValidateLogin(model);

            var empresas = await _empresasRepository.GetAllByIdUser(user.IdUsuario);

            string profesional = string.Empty;
            if (user.IdProfesional > 0) {
                profesional = (await _profesionalesRepository.GetById<Domain.Entities.Profesional>(user.IdProfesional.Value)).Nombre;
            }

            var sessionUser = new Usuario(
                id: user.IdUsuario,
                nombre: user.Nombre,
                login: user.Login,
                idTipoUsuario: user.IdTipoUsuario,
                admin: user.Admin,
                idEmpresa: empresas.FirstOrDefault()?.IdEmpresa ?? 0,
                empresa: String.Empty,
                cuit: String.Empty,
                idProfesional: user.IdProfesional.HasValue ? user.IdProfesional.Value : 0,
                idPaciente: user.IdPaciente.HasValue ? user.IdPaciente.Value : 0,
                profesional: profesional
            );

            var userCookie = $"{user.Login}";
            context.Session.SetString($"LizardCode.SalmaSalud.{userCookie}", JsonConvert.SerializeObject(sessionUser));

            if (empresas.Count == 1)
            {
                sessionUser.IdEmpresa = empresas.FirstOrDefault()?.IdEmpresa ?? 0;
                sessionUser.Empresa = empresas.FirstOrDefault()?.RazonSocial ?? String.Empty;
                sessionUser.CUIT = empresas.FirstOrDefault()?.CUIT ?? String.Empty;

                context.Session.SetString($"LizardCode.SalmaSalud.{userCookie}", JsonConvert.SerializeObject(sessionUser));
            }
            else
            {
                if(model.IdEmpresa == 0)
                {
                    throw new EmpresaNotDefinedException();
                }

                var empresa = await _empresasRepository.GetById<Domain.Entities.Empresa>(model.IdEmpresa);
                if (empresa == null)
                {
                    throw new EmpresaNotFoundException();
                }

                sessionUser.IdEmpresa = empresa.IdEmpresa;
                sessionUser.Empresa = empresa.RazonSocial;
                sessionUser.CUIT = empresa.CUIT;

                context.Session.SetString($"LizardCode.SalmaSalud.{userCookie}", JsonConvert.SerializeObject(sessionUser));

            }

            await SetAuditoriaLogin(model, user.IdUsuario, sessionUser.IdEmpresa, context.Request.GetIP());

            var tipoUsuario = ((TipoUsuario)user.IdTipoUsuario).Description();
            await SetCookie(context, user.IdUsuario, user.Login, tipoUsuario);

            return sessionUser;
        }

        public async Task<Usuario> SignInPacientes(HttpContext context, LoginViewModel model)
        {
            if (model.User.IsNull() || model.Pass.IsNull())
            {
                throw new ArgumentNullException(nameof(model.User));
            }

            var login = model.User.Trim().ToLower();

            var user = await _usersRepository.GetByDocumento(model.User);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if (user.IdTipoUsuario != (int)TipoUsuario.Paciente)
            {
                throw new UserNotFoundException();
            }

            //if (!user.BlankToken.IsNull())
            //{
            //    throw new UserNotActivatedException();
            //}

            if (user.Vencimiento < DateTime.Now)
            {
                throw new PasswordExpiredException();
            }

            if (!user.Password.Equals(Cryptography.HashPassword(model.Pass, Convert.FromBase64String(user.PasswordSalt))))
            {
                throw new LoginFailedException();
            }

            //if (!user.Admin && !CheckGeneralAccess())
            //{
            //    throw new PermissionException(PermisoError.SinPermiso);
            //}

            var empresas = await _empresasRepository.GetAllByIdUser(user.IdUsuario);

            var sessionUser = new Usuario(
                id: user.IdUsuario,
                nombre: user.Nombre,
                login: user.Login,
                idTipoUsuario: (int)TipoUsuario.Paciente,
                admin: false,
                idEmpresa: 0,
                empresa: string.Empty,
                cuit: string.Empty,
                idProfesional: 0,
                idPaciente: user.IdPaciente.HasValue ? user.IdPaciente.Value : 0,
                profesional: string.Empty
            );

            var userCookie = $"{user.Login}";
            context.Session.SetString($"LizardCode.SalmaSalud.{userCookie}", JsonConvert.SerializeObject(sessionUser));

            sessionUser.IdEmpresa = 0;
            sessionUser.Empresa = string.Empty;
            sessionUser.CUIT = string.Empty;

            context.Session.SetString($"LizardCode.SalmaSalud.{userCookie}", JsonConvert.SerializeObject(sessionUser));
            
            await SetAuditoriaLogin(model, user.IdUsuario, sessionUser.IdEmpresa, context.Request.GetIP());

            var tipoUsuario = ((TipoUsuario)user.IdTipoUsuario).Description();
            await SetCookie(context, user.IdUsuario, user.Login, tipoUsuario);

            return sessionUser;
        }

        public async Task SetCookie(HttpContext context, int idUser, string login, string idUserType)
        {
            var userCookie = $"{login}";

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

            return $"LizardCode.SalmaSalud.{user}".FromSession<Usuario>();
        }

        public Usuario UserSession(string user)
        {
            return $"LizardCode.SalmaSalud.{user}".FromSession<Usuario>();
        }

        private async Task SetAuditoriaLogin(LoginViewModel model, int idUsuario, int idEmpresa, string IP)
        {
            await _auditoriaLoginRepository.Insert(new Domain.Entities.AuditoriaLogin
            {
                IdUsuario = idUsuario,
                IdEmpresa = idEmpresa,
                Fecha = DateTime.Now,
                Platform = model.Platform,
                Os = model.OS,
                Browser = model.Browser,
                Version = model.Version,
                Resolucion = model.Resolucion,
                IP = IP
            });
        }

        public async Task<Domain.Entities.Usuario> ValidateLogin(LoginViewModel model)
        {
            if (model.User.IsNull() || model.Pass.IsNull())
            {
                throw new ArgumentNullException(nameof(model.User));
            }

            var login = model.User.Trim().ToLower();

            var users = await _usersRepository.GetAll<Domain.Entities.Usuario>();
            var user = users.FirstOrDefault(user => user.Login == login);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if (!user.BlankToken.IsNull())
            {
                throw new UserNotActivatedException();
            }

            if (!user.Password.Equals(Cryptography.HashPassword(model.Pass, Convert.FromBase64String(user.PasswordSalt))))
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

            return user;
        }
    }
}
