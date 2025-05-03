using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Helpers;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using LizardCode.Framework.Application.Common.Enums;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Usuarios;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class UsuariosBusiness: BaseBusiness, IUsuariosBusiness
    {
        private readonly ILogger<UsuariosBusiness> _logger;
        private readonly IUsuariosRepository _usersRepository;
        private readonly IUsuariosEmpresasRepository _usuariosEmpresasRepository;
        private readonly IAuditoriaLoginRepository _auditoriaLoginRepository;
        private readonly IProfesionalesRepository _profesionalesRepository;
        private readonly IPacientesRepository _pacientesRepository;

        private readonly IChatApiBusiness _chatApiBusiness;
        private readonly IMailBusiness _mailBusiness;
        public UsuariosBusiness(
            IUsuariosRepository usersRepository,
            ILogger<UsuariosBusiness> logger,
            IUsuariosEmpresasRepository usuariosEmpresasRepository,
            IAuditoriaLoginRepository auditoriaLoginRepository,
            IProfesionalesRepository profesionalesRepository,
            IPacientesRepository pacientesRepository,
            IChatApiBusiness chatApiBusiness,
            IMailBusiness mailBusiness)
        {
            _usersRepository = usersRepository;
            _logger = logger;
            _usuariosEmpresasRepository = usuariosEmpresasRepository;
            _auditoriaLoginRepository = auditoriaLoginRepository;
            _profesionalesRepository = profesionalesRepository;
            _pacientesRepository = pacientesRepository;
            _chatApiBusiness = chatApiBusiness;
            _mailBusiness = mailBusiness;
        }

        public async Task New(UsuarioViewModel model)
        {
            var user = _mapper.Map<Usuario>(model);
            var empresas = model.Empresas;

            Validate(user);

            var salt = Cryptography.GenerateSalt();

            var tran = _uow.BeginTransaction();

            try
            {
                user.Login = user.Login.ToLower().Trim();
                user.Nombre = user.Nombre.ToUpper().Trim();
                user.Email = user.Email.ToLower().Trim();
                user.Password = Cryptography.HashPassword("1234", salt);
                user.PasswordSalt = Convert.ToBase64String(salt);
                user.BlankToken = null;
                user.Vencimiento = DateTime.Now.AddDays(-1);
                user.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                user.IdPaciente = null; //Siempre NULL... no hay alta de user pacientes vía sistema

                if (user.IdTipoUsuario != (int)TipoUsuario.Profesional
                    && user.IdTipoUsuario != (int)TipoUsuario.ProfesionalExterno
                    && user.IdTipoUsuario != (int)TipoUsuario.Administrador)
                { 
                    user.IdProfesional = null;
                }
                else
                {
                    if (user.IdProfesional.Value == 0)
                        user.IdProfesional = null;
                    else
                    { 
                        var profesional = await _profesionalesRepository.GetById<Profesional>(user.IdProfesional.Value, tran);

                        if (profesional == null)
                            throw new BusinessException("No se encontró el Profesional.");

                        user.IdProfesional = profesional.IdProfesional;
                    }
                }

                var id = await _usersRepository.Insert(user, tran);

                if (empresas != null && empresas.Count > 0)
                {
                    foreach (var empresa in empresas)
                    {
                        await _usuariosEmpresasRepository.Insert(new UsuarioEmpresa() { IdUsuario = (int)id, IdEmpresa = empresa }, tran);
                    }
                }

                tran.Commit();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();

                throw new Exception(ex.Message);
            }
        }

        public async Task<UsuarioViewModel> Get(int idUser)
        {
            var user = await _usersRepository.GetById<Usuario>(idUser);
            var empresas = await _usuariosEmpresasRepository.GetAllByIdUsuario(idUser);

            if (user == null)
                return null;

            var model = _mapper.Map<UsuarioViewModel>(user);
            model.Empresas = empresas.Select(e => e.IdEmpresa).ToList();

            return model;
        }

        public async Task<DataTablesResponse<Usuario>> GetAll(DataTablesRequest request)
        {
            var customQuery = _usersRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("IdUsuario"))
                builder.Append($"AND IdUsuario = {filters["IdUsuario"]}");

            if (filters.ContainsKey("IdTipoUsuario"))
                builder.Append($"AND IdTipoUsuario = {filters["IdTipoUsuario"]}");

            if (filters.ContainsKey("Login"))
                builder.Append($"AND Login LIKE {string.Concat("%", filters["Login"], "%")}");

            if (filters.ContainsKey("Nombre"))
                builder.Append($"AND Nombre LIKE {string.Concat("%", filters["Nombre"], "%")}");

            if (filters.ContainsKey("Email"))
                builder.Append($"AND Email LIKE {string.Concat("%", filters["Email"], "%")}");

            builder.Append($"AND idTipoUSuario != {(int)TipoUsuario.Paciente}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Usuario>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(UsuarioViewModel model)
        {
            var user = _mapper.Map<Usuario>(model);
            var empresas = model.Empresas;

            Validate(user);

            var dbUser = await _usersRepository.GetById<Usuario>(user.IdUsuario);

            if (dbUser == null)
                throw new BusinessException("Usuario Inexistente");

            using var tran = _uow.BeginTransaction();

            try
            {
                dbUser.IdTipoUsuario = user.IdTipoUsuario;
                dbUser.Nombre = user.Nombre.ToUpper().Trim();
                dbUser.Email = user.Email.ToLower().Trim();
                dbUser.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                if (user.IdTipoUsuario != (int)TipoUsuario.Profesional && user.IdTipoUsuario != (int)TipoUsuario.Administrador)
                    dbUser.IdProfesional = null;
                else
                {
                    if (user.IdProfesional.Value == 0)
                        dbUser.IdProfesional = null;
                    else
                    { 
                        var profesional = await _profesionalesRepository.GetById<Profesional>(user.IdProfesional.Value, tran);

                        if (profesional == null)
                            throw new BusinessException("No se encontró el Profesional.");

                        dbUser.IdProfesional = profesional.IdProfesional;
                    }
                }

                await _usersRepository.Update(dbUser, tran);

                await _usuariosEmpresasRepository.RemoveByIdUsuario(dbUser.IdUsuario, tran);
                if (empresas != null && empresas.Count > 0)
                {
                    foreach (var empresa in empresas)
                    {
                        await _usuariosEmpresasRepository.Insert(new UsuarioEmpresa() { IdUsuario = dbUser.IdUsuario, IdEmpresa = empresa }, tran);
                    }
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();

                throw new Exception(ex.Message);
            }
        }

        public async Task Remove(int idUser)
        {
            var user = await _usersRepository.GetById<Usuario>(idUser);

            if (user == null)
                throw new BusinessException("Usuario Inexistente");


            if (user.IdTipoUsuario == (int)TipoUsuario.Paciente)
            {
                throw new BusinessException("No se puede eliminar un Usuario del tipo Paciente.");
            }


            await _usuariosEmpresasRepository.RemoveByIdUsuario(user.IdUsuario);
            user.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _usersRepository.Update(user);

            if (idUser == _permissionsBusiness.Value.User.Id)
                await HttpContextHelper.Current.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task Blank(int idUser)
        {
            var user = await _usersRepository.GetById<Usuario>(idUser);

            if (user == null)
                throw new BusinessException("Usuario Inexistente");

            if (user.IdTipoUsuario == (int)TipoUsuario.Paciente)
                throw new BusinessException("No se puede blanquear el password de un usuario paciente");

            var salt = Cryptography.GenerateSalt();

            user.Password = Cryptography.HashPassword("1234", salt); //GetTempPassword()
            user.PasswordSalt = Convert.ToBase64String(salt);
            user.BlankToken = null;
            user.Vencimiento = DateTime.Now.AddDays(-1);
            user.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            await _usersRepository.Update(user);
        }

        public async Task ResetPassword(HttpContext context, string login, string pass, string newPass, string repeatPass)
        {
            if (login.IsNull() || pass.IsNull() || newPass.IsNull() || repeatPass.IsNull())
                throw new BusinessException("Login Incorrecto");

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

            var tipoUsuario = ((TipoUsuario)dbUser.IdTipoUsuario).Description();

            await _usersRepository.Update(dbUser);
            await _permissionsBusiness.Value.SetCookie(context, dbUser.IdUsuario, dbUser.Login, tipoUsuario);
        }

        private void Validate(Usuario user)
        {
            if (user.Nombre.IsNull())
            {
                throw new BusinessException("Ingrese el Nombre de Usuario.");
            }

            if (!Regex.Match(user.Login, "^[a-zA-Z][a-zA-Z0-9]{4,49}$").Success)
            {
                throw new BusinessException("El Nombre de Usuario debe empezar con una letra y solo puede contener letras y números. Además debe tener al menos 5 caracteres.");
            }

            if (user.Email.IsNull())
            {
                throw new BusinessException("Ingrese el EMail del Usuario.");
            }

            if ((user.IdTipoUsuario == (int)TipoUsuario.Profesional || user.IdTipoUsuario == (int)TipoUsuario.ProfesionalExterno) 
                && user.IdProfesional == 0)
            {
                throw new BusinessException("Ingrese un Profesional válido.");
            }

            if (user.IdTipoUsuario == (int)TipoUsuario.Paciente)
            {
                throw new BusinessException("No se puede dar de alta o modificar un Usuario del tipo Paciente.");
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

        public async Task<DataTablesResponse<Custom.AuditoriaLogin>> GetAllAuditoriaLogin(DataTablesRequest request)
        {
            var customQuery = _auditoriaLoginRepository.GetAllCustomQuery(_permissionsBusiness.Value.User.IdEmpresa);
            var builder = _dbContext.Connection.QueryBuilder();

            return await _dataTablesService.Resolve<Custom.AuditoriaLogin>(request, customQuery.Sql, customQuery.Parameters, false, staticWhere: builder.Sql);
        }

        public async Task<string> RequestAccessCode(string documento)
        {
            var user = await _usersRepository.GetByDocumento(documento);

            if (user == null)
                throw new BusinessException("Usuario inexistente");

            if (user.IdTipoUsuario != (int)TipoUsuario.Paciente)
                throw new BusinessException("El usuario no es un paciente");

            var paciente = await _pacientesRepository.GetById<Paciente>(user.IdPaciente.Value);
            if (paciente.IdEstadoRegistro == (int)EstadoRegistro.Eliminado)
                throw new BusinessException($"El usuario no se encuentra activo.");

            if (user.Vencimiento > DateTime.Now)
            {
                TimeSpan ts = user.Vencimiento - DateTime.Now;
                throw new BusinessException($"Ya se ha enviado un código. Intente solicitar nuevamente en {(int)ts.TotalMinutes} minutos.");
            }

            var salt = Cryptography.GenerateSalt();
            var codigoAcceso = GetTempPassword();

            user.Password = Cryptography.HashPassword(codigoAcceso, salt);
            user.PasswordSalt = Convert.ToBase64String(salt);
            user.BlankToken = null;
            user.Vencimiento = DateTime.Now.AddMinutes(30);
            user.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            await _usersRepository.Update(user);

            //TODO GS: VER DE ACTIVAR ESTOS MÉTODOS            
            await _chatApiBusiness.SendMessageCodigoAcceso(paciente.Telefono, paciente.Nombre, codigoAcceso, user.Vencimiento, paciente.IdPaciente);            
            await _mailBusiness.EnviarMailCodigoAccesoPaciente(codigoAcceso, paciente.Email, paciente.Nombre);

            return codigoAcceso;
        }

        private static string GetTempPassword() {
            var passwordCode = Guid.NewGuid().ToString();
            passwordCode = passwordCode.Substring(passwordCode.Length - 6, 6);

            return passwordCode;
        }
    }
}
