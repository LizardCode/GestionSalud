using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Pacientes;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Collections.Generic;
using System.Data;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class PacientesBusiness : BaseBusiness, IPacientesBusiness
    {
        private readonly ILogger<PacientesBusiness> _logger;
        private readonly IPacientesRepository _pacientesRepository;
        private readonly IClientesRepository _clientesRepository;
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly IMailBusiness _mailBusiness;

        public PacientesBusiness(
            IPacientesRepository PacientesRepository,
            ILogger<PacientesBusiness> logger,
            IClientesRepository clientesRepository,
            IUsuariosRepository usuariosRepository,
            IMailBusiness mailBusiness)
        {
            _pacientesRepository = PacientesRepository;
            _logger = logger;
            _clientesRepository = clientesRepository;
            _usuariosRepository = usuariosRepository;
            _mailBusiness = mailBusiness;
        }

        public async Task New(PacienteViewModel model)
        {
            var paciente = _mapper.Map<Paciente>(model);

            Validate(paciente);

            var tran = _uow.BeginTransaction();

            try
            {
                //Busco si ya no Existe el Paciente en otra Empresa
                Paciente dbPaciente = null;
                if (!string.IsNullOrEmpty(paciente.Documento))
                    dbPaciente = await _pacientesRepository.GetPacienteByDocumento(paciente.Documento.ToUpper().Trim(), tran);

                if (dbPaciente == default)
                {
                    //if (model.SinCobertura)
                    //{
                    //    paciente.IdFinanciador = null;
                    //    paciente.IdFinanciadorPlan = null;
                    //    paciente.FinanciadorNro = string.Empty;
                    //}
                    //else
                    //{
                    //    paciente.IdFinanciador = model.IdFinanciador;
                    //    paciente.IdFinanciadorPlan = model.IdFinanciadorPlan;
                    //    paciente.FinanciadorNro = model.FinanciadorNro;
                    //}
                    paciente.FinanciadorNro = model.FinanciadorNro;

                    paciente.IdTipoTelefono = (int)TipoTelefono.Movil;
                    paciente.Nombre = paciente.Nombre.ToUpper().Trim();
                    paciente.Nacionalidad = paciente.Nacionalidad?.ToUpper().Trim();
                    paciente.Email = paciente.Email.ToLower().Trim();
                    paciente.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                    var id = await _pacientesRepository.Insert(paciente, tran);

                    paciente.IdPaciente = (int)id;
                    await NewCliente(paciente, tran);

                    _mailBusiness.EnviarMailBienvenidaPaciente(paciente.Email, paciente.Nombre);
                }
                else
                {
                    //if (model.SinCobertura)
                    //{
                    //    dbPaciente.IdFinanciador = null;
                    //    dbPaciente.IdFinanciadorPlan = null;
                    //    dbPaciente.FinanciadorNro = string.Empty;
                    //}
                    //else
                    //{
                    //    dbPaciente.IdFinanciador = model.IdFinanciador;
                    //    dbPaciente.IdFinanciadorPlan = model.IdFinanciadorPlan;
                    //    dbPaciente.FinanciadorNro = model.FinanciadorNro;
                    //}
                    dbPaciente.FinanciadorNro = model.FinanciadorNro;

                    dbPaciente.IdTipoTelefono = (int)TipoTelefono.Movil;
                    dbPaciente.Telefono = paciente.Telefono;
                    dbPaciente.Nombre = paciente.Nombre.ToUpper().Trim();
                    dbPaciente.Nacionalidad = paciente.Nacionalidad?.ToUpper().Trim();
                    dbPaciente.Email = paciente.Email.ToLower().Trim();
                    dbPaciente.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                    await _pacientesRepository.Update(dbPaciente, tran);

                    await UpdateCliente(dbPaciente, tran);
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task<PacienteViewModel> Get(int idPaciente)
        {
            var client = await _pacientesRepository.GetById<Paciente>(idPaciente);

            if (client == null)
                return null;

            var model = _mapper.Map<PacienteViewModel>(client);

            return model;
        }

        public async Task<Custom.Paciente> GetCustomById(int idPaciente)
            => await _pacientesRepository.GetCustomById(idPaciente);

        public async Task<DataTablesResponse<Custom.Paciente>> GetAll(DataTablesRequest request)
        {
            var customQuery = _pacientesRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("FiltroNombre"))
                builder.Append($"AND Nombre LIKE {string.Concat("%", filters["FiltroNombre"], "%")}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Paciente>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(PacienteViewModel model)
        {
            var paciente = _mapper.Map<Paciente>(model);

            Validate(paciente);

            var dbPaciente = await _pacientesRepository.GetById<Paciente>(paciente.IdPaciente);

            if (dbPaciente == null)
            {
                throw new ArgumentException("Paciente inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbPaciente.IdTipoTelefono = (int)TipoTelefono.Movil;
                dbPaciente.Telefono = paciente.Telefono;
                dbPaciente.FechaNacimiento = paciente.FechaNacimiento;
                dbPaciente.Nombre = paciente.Nombre.ToUpper().Trim();
                dbPaciente.Nacionalidad = paciente.Nacionalidad?.ToUpper().Trim();
                dbPaciente.Email = paciente.Email.ToLower().Trim();
                dbPaciente.Telefono = paciente.Telefono.ToLower().Trim();

                if (model.SinCobertura)
                {
                    dbPaciente.IdFinanciador = null;
                    dbPaciente.IdFinanciadorPlan = null;
                    dbPaciente.FinanciadorNro = string.Empty;
                }
                else
                {
                    dbPaciente.IdFinanciador = model.IdFinanciador;
                    dbPaciente.IdFinanciadorPlan = model.IdFinanciadorPlan;
                    dbPaciente.FinanciadorNro = model.FinanciadorNro;
                }

                dbPaciente.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _pacientesRepository.Update(dbPaciente, tran);

                await UpdateCliente(dbPaciente, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Remove(int idPaciente)
        {
            var paciente = await _pacientesRepository.GetById<Paciente>(idPaciente);

            if (paciente == null)
            {
                throw new ArgumentException("Paciente inexistente");
            }

            paciente.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _pacientesRepository.Update(paciente);
        }


        private void Validate(Paciente paciente)
        {
            if (paciente.Nombre.IsNull())
            {
                throw new BusinessException("Ingrese el Nombre de Fantasia para Paciente");
            }

            if (paciente.Telefono.IsNull())
            {
                throw new BusinessException("Ingrese un Teléfono para el Paciente");
            }

            if (paciente.Email.IsNull())
            {
                throw new BusinessException("Ingrese un E-Mail para el Paciente");
            }
        }

        public async Task<string> ValidarNroCUIT(string cuit, int? idPaciente)
        {
            if (!cuit.ValidarCUIT())
            {
                return new string("C.U.I.T. Invalido");
            }

            var result = await _pacientesRepository.ValidarCUITExistente(cuit, idPaciente);
            if (result)
                return new string("Existe un Paciente con el nro de C.U.I.T. Ingresado. Verifique");

            return null;
        }

        public async Task<string> ValidarNroDocumento(string documento, int? idCliente)
        {
            var result = await _pacientesRepository.ValidarDocumentoExistente(documento, idCliente);
            if (result)
                return new string("Existe un Paciente con el nro de Documento Ingresado. Verifique"); 

            return null;
        }

        public async Task<string> ValidarNroFinanciador(string financiadorNro, int? idCliente)
        {
            if (string.IsNullOrEmpty(financiadorNro))
                return null;

            var result = await _pacientesRepository.ValidarNroFinanciadorExistente(financiadorNro, idCliente);
            if (result)
                return new string("Ya existe un Paciente con el nro. de Afiliado ingresado. Verifique");

            return null;
        }

        public async Task<Custom.Paciente> GetLikeDocument(string documento) =>
            await _pacientesRepository.GetLikeDocumentoCustom(documento.Replace(".", "").Replace(" ", ""));

        public async Task<Custom.Paciente> GetLikePhone(string phone) =>
            await _pacientesRepository.GetLikePhoneCustom(phone.Replace(".", "").Replace(" ", ""));

        private async Task NewCliente(Paciente paciente, IDbTransaction transaction = null)
        {
            var cliente = new Cliente
            {
                IdPaciente = paciente.IdPaciente,
                RazonSocial = paciente.Nombre,
                NombreFantasia = paciente.Nombre,
                IdTipoDocumento = (int)TipoDocumento.DNI,
                Documento = paciente.Documento,
                IdTipoIVA = (int)TipoIVA.ConsumidorFinal,
                Email = paciente.Email,
                IdTipoTelefono = (int)TipoTelefono.Movil,
                Telefono = paciente.Telefono,
                IdEstadoRegistro = (int)EstadoRegistro.Nuevo
            };

            var idCliente = await _clientesRepository.Insert(cliente, transaction);

            //paciente.IdPaciente = (int)idPaciente;
            await NewUsuario(paciente, transaction);
        }

        private async Task NewUsuario(Paciente paciente, IDbTransaction transaction = null)
        {
            var salt = Cryptography.GenerateSalt();
            var password = Cryptography.GetTempPassword();
            var usuario = new Usuario
            {
                Login = paciente.Documento,
                Nombre = paciente.Nombre,
                Email = paciente.Email,
                Password = Cryptography.HashPassword(password, salt),
                PasswordSalt = Convert.ToBase64String(salt),
                BlankToken = null,
                Vencimiento = DateTime.Now.AddDays(-1),
                IdTipoUsuario = (int)TipoUsuario.Paciente,
                IdPaciente = paciente.IdPaciente,
                IdEstadoRegistro = (int)EstadoRegistro.Nuevo
            };

            await _usuariosRepository.Insert(usuario, transaction);
        }

        private async Task UpdateCliente(Paciente paciente, IDbTransaction transaction = null)
        {
            var cliente = await _clientesRepository.GetClienteByIdPaciente(paciente.IdPaciente, transaction);

            cliente.NombreFantasia = paciente.Nombre;
            cliente.Email = paciente.Email;
            cliente.IdTipoTelefono = (int)TipoTelefono.Movil;
            cliente.Telefono = paciente.Telefono;
            cliente.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            await _clientesRepository.Update(cliente, transaction);
        }

        public async Task UpdateFromPortal(PacienteViewModel model)
        {
            var paciente = _mapper.Map<Paciente>(model);

            if (!paciente.FechaNacimiento.HasValue)
            {
                throw new BusinessException("Ingrese una Fecha de Nacimiento para el Paciente");
            }

            if (paciente.Telefono.IsNull())
            {
                throw new BusinessException("Ingrese un Teléfono para el Paciente");
            }

            if (paciente.Email.IsNull())
            {
                throw new BusinessException("Ingrese un E-Mail para el Paciente");
            }

            var dbPaciente = await _pacientesRepository.GetById<Paciente>(_permissionsBusiness.Value.User.IdPaciente);
            if (dbPaciente == null)
            {
                throw new ArgumentException("Paciente inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbPaciente.IdTipoTelefono = (int)TipoTelefono.Movil;
                dbPaciente.FechaNacimiento = paciente.FechaNacimiento;
                dbPaciente.Telefono = paciente.Telefono;
                dbPaciente.Email = paciente.Email.ToLower().Trim();

                dbPaciente.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _pacientesRepository.Update(dbPaciente, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task ADMIN_CreateClientes()
        {
            var pacientes = await _pacientesRepository.GetAll<Paciente>();
            var tran = _uow.BeginTransaction();
            try
            {
                foreach (var paciente in pacientes)
                {
                    await NewCliente(paciente, tran);
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }
        public async Task ADMIN_CreateUsuarios()
        {
            var pacientes = await _pacientesRepository.GetAll<Paciente>();
            var tran = _uow.BeginTransaction();
            try
            {
                foreach (var paciente in pacientes)
                {
                    await NewUsuario(paciente, tran);
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }
    }
}
