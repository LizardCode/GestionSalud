using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
using LizardCode.Framework.Aplication.Common.Exceptions;
using LizardCode.Framework.Aplication.Helpers;
using LizardCode.Framework.Aplication.Interfaces.Business;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.Framework.Infrastructure.Interfaces.Context;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Template.Application.Interfaces.Business;
using Template.Application.Interfaces.Repositories;
using Template.Application.Models.Usuarios;
using Template.Domain.Entities;
using Template.Domain.Enums;

namespace Template.Application.Business
{
    public class UsuariosBusiness : IUsuariosBusiness
    {
        private readonly IMapper _mapper;
        private readonly IDbContext _dbContext;
        private readonly IUnitOfWork _uow;
        private readonly IDataTablesService _dataTablesService;
        private readonly IUsuariosRepository _usersRepository;
        private readonly Lazy<IPermisosBusiness> _permissionsBusiness;


        public UsuariosBusiness(
            IMapper mapper,
            IDbContext dbContext,
            IUnitOfWork uow,
            IDataTablesService dataTablesService,
            IUsuariosRepository usersRepository,
            Lazy<IPermisosBusiness> permissionsBusiness)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _uow = uow;
            _dataTablesService = dataTablesService;
            _usersRepository = usersRepository;
            _permissionsBusiness = permissionsBusiness;
        }


        public async Task New(UsuarioViewModel model)
        {
            var user = _mapper.Map<Usuario>(model);

            Validate(user);

            var salt = Cryptography.GenerateSalt();

            user.Login = user.Login.ToLower().Trim();
            user.Nombre = user.Nombre.ToUpper().Trim();
            user.Email = user.Email.ToLower().Trim();
            user.Password = Cryptography.HashPassword("1234", salt);
            user.PasswordSalt = Convert.ToBase64String(salt);
            user.BlankToken = null;
            user.Vencimiento = DateTime.Now.AddDays(-1);
            user.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

            var tran = _uow.BeginTransaction();

            await _usersRepository.Insert(user, tran);

            tran.Commit();
        }

        public async Task<UsuarioViewModel> Get(int idUser)
        {
            var user = await _usersRepository.GetById<Usuario>(idUser);

            if (user == null)
                return null;

            var model = _mapper.Map<UsuarioViewModel>(user);

            return model;
        }

        public async Task<DataTablesResponse<Usuario>> GetAll(DataTablesRequest request)
        {
            var results = await _dataTablesService.Resolve<Usuario>(request);
            return results;
        }

        public async Task Update(UsuarioViewModel model)
        {
            var user = _mapper.Map<Usuario>(model);

            Validate(user);

            var dbUser = await _usersRepository.GetById<Usuario>(user.IdUsuario);

            if (dbUser == null)
            {
                throw new BusinessException("Usuario inexistente");
            }

            dbUser.IdTipoUsuario = user.IdTipoUsuario;
            dbUser.Nombre = user.Nombre.ToUpper().Trim();
            dbUser.Email = user.Email.ToLower().Trim();
            dbUser.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            using var tran = _uow.BeginTransaction();
            await _usersRepository.Update(dbUser, tran);

            tran.Commit();
        }

        public async Task Remove(int idUser)
        {
            var user = await _usersRepository.GetById<Usuario>(idUser);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            user.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _usersRepository.Update(user);

            if (idUser == _permissionsBusiness.Value.User.Id)
                await HttpContextHelper.Current.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task Blank(int idUser)
        {
            var user = await _usersRepository.GetById<Usuario>(idUser);

            if (user == null)
                throw new UserNotFoundException();

            var salt = Cryptography.GenerateSalt();

            user.Password = Cryptography.HashPassword("1234", salt);
            user.PasswordSalt = Convert.ToBase64String(salt);
            user.BlankToken = null;
            user.Vencimiento = DateTime.Now.AddDays(-1);
            user.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            await _usersRepository.Update(user);
        }

        public async Task ResetPassword(HttpContext context, string login, string pass, string newPass, string repeatPass)
        {
            if (login.IsNull() || pass.IsNull() || newPass.IsNull() || repeatPass.IsNull())
                throw new BusinessException(nameof(login));

            login = login.Trim().ToLower();
            pass = pass.Trim();
            newPass = newPass.Trim();
            repeatPass = repeatPass.Trim();

            if (!Regex.IsMatch(newPass, @"^(?=.*?\d.*\d)[a-zA-Z0-9]{8,}$"))
                throw new WeekPasswordException();

            if (!newPass.Equals(repeatPass))
                throw new PasswordNotMatchException();

            var users = await _usersRepository.GetAll<Usuario>();
            users = users
                .Where(w =>
                    w.Login == login &&
                    w.IdEstadoRegistro != (int)EstadoRegistro.Eliminado
                )
                .ToList();

            if (users == null)
                throw new UserNotFoundException();

            if (users.Count > 1)
                throw new TooManyRecordsException();

            var dbUser = users.First();

            if (!dbUser.Password.Equals(Cryptography.HashPassword(pass, Convert.FromBase64String(dbUser.PasswordSalt))))
                throw new WrongPasswordException();

            var saltBuffer = Cryptography.GenerateSalt();
            dbUser.Password = Cryptography.HashPassword(newPass, saltBuffer);
            dbUser.PasswordSalt = Convert.ToBase64String(saltBuffer);
            dbUser.BlankToken = null;
            dbUser.Vencimiento = DateTime.Now.AddMonths("Users:PasswordsExpiration".FromAppSettings(6));

            await _usersRepository.Update(dbUser);
            await _permissionsBusiness.Value.SetCookie(context, dbUser.IdUsuario, dbUser.Login, "ADMIN");
        }

        private void Validate(Usuario user)
        {
            if (user.Nombre.IsNull())
            {
                throw new BusinessException(nameof(user.Nombre));
            }

            if (user.Email.IsNull())
            {
                throw new BusinessException(nameof(user.Email));
            }
        }

        public async Task<bool> CheckLogin(string login)
        {
            login = login.ToLower().Trim();

            var users = await _usersRepository.GetAll<Usuario>();
            var user = users
                .Where(w =>
                    w.Login == login &&
                    w.IdEstadoRegistro != (int)EstadoRegistro.Eliminado
                )
                .FirstOrDefault();

            return (user == null);
        }
    }
}
