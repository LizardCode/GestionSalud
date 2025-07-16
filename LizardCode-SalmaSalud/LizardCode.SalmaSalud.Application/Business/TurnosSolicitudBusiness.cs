using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Common.Exceptions;
using Microsoft.Extensions.Logging;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.PortalPacientes;
using LizardCode.SalmaSalud.Application.Models.TurnosSolicitud;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Linq;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class TurnosSolicitudBusiness : BaseBusiness, ITurnosSolicitudBusiness
    {
        private readonly ILogger<TurnosSolicitudBusiness> _logger;
        private readonly ITurnosSolicitudRepository _turnosSolicitudRepository;
        private readonly IPacientesRepository _pacientesRepository;
        private readonly IMailBusiness _mailBusiness;
        private readonly IChatApiBusiness _chatApiBusiness;
        private readonly ITurnosSolicitudDiasRepository _turnosSolicitudDiasRepository;
        private readonly ITurnosSolicitudRangosHorariosRepository _turnosSolicitudRangosHorariosRepository;

        public TurnosSolicitudBusiness(
            ILogger<TurnosSolicitudBusiness> logger,
            IPacientesRepository pacientesRepository,
            ITurnosSolicitudRepository turnosSolicitudRepository,
            IMailBusiness mailBusiness,
            IChatApiBusiness chatApiBusiness,
            ITurnosSolicitudDiasRepository turnosSolicitudDiasRepository,
            ITurnosSolicitudRangosHorariosRepository turnosSolicitudRangosHorariosRepository)
        {
            _logger = logger;
            _turnosSolicitudRepository = turnosSolicitudRepository;
            _pacientesRepository = pacientesRepository;
            _mailBusiness = mailBusiness;
            _chatApiBusiness = chatApiBusiness;
            _turnosSolicitudDiasRepository = turnosSolicitudDiasRepository;
            _turnosSolicitudRangosHorariosRepository = turnosSolicitudRangosHorariosRepository;
        }

        public async Task<Custom.TurnoSolicitudTotales> ObtenerTotalesDashboard()
            => await _turnosSolicitudRepository.GetTotalesDashboard();

        public async Task<int> New(TurnoSolicitudViewModel model)
        {
            var solicitud = _mapper.Map<TurnoSolicitud>(model);

            Validate(solicitud);

            if (model.Dias.Count == 0 || model.RangosHorarios.Count == 0)
            {
                throw new BusinessException("Ingrese un día y horario válido.");
            }

            var tran = _uow.BeginTransaction();
            try
            {
                solicitud.FechaSolicitud = DateTime.Now;
                solicitud.IdEstadoTurnoSolicitud = (int)EstadoTurnoSolicitud.Solicitado;
                solicitud.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                var id = await _turnosSolicitudRepository.Insert(solicitud, tran);

                foreach(var dia in model.Dias)
                {
                    await _turnosSolicitudDiasRepository.Insert(new TurnoSolicitudDia { 
                                                                                    IdTurnoSolicitud = (int)id, 
                                                                                    Dia = dia                                                                                    
                    }, tran);
                }

                foreach (var rango in model.RangosHorarios)
                {
                    await _turnosSolicitudRangosHorariosRepository.Insert(new TurnoSolicitudRangoHorario { 
                                                                                                        IdTurnoSolicitud = (int)id, 
                                                                                                        IdRangoHorario = rango 
                    }, tran);
                }

                //TODO GS: Enviar mail con la data que se solicitó
                //_mailBusiness.EnviarMailBienvenidaPaciente(paciente.Email, paciente.Nombre);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }

            return solicitud.IdTurnoSolicitud;
        }

        public async Task<TurnoSolicitudViewModel> Get(int idSolicitud)
        {
            var solicitud = await _turnosSolicitudRepository.GetById<TurnoSolicitud>(idSolicitud);

            if (solicitud == null)
                return null;

            var model = _mapper.Map<TurnoSolicitudViewModel>(solicitud);

            return model;
        }

        public async Task<Custom.TurnoSolicitud> GetCustomById(int idSolicitud)
            => await _turnosSolicitudRepository.GetCustomById(idSolicitud);

        public async Task<DataTablesResponse<Custom.TurnoSolicitud>> GetAll(DataTablesRequest request)
        {
            var customQuery = _turnosSolicitudRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND FechaSolicitud >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
            {
                var date = DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null);
                builder.Append($"AND FechaSolicitud <= {date.AddDays(1)}");
            }

            if (filters.ContainsKey("FechaAsigDesde") && filters["FechaAsigDesde"].ToString() != "__/__/____")
                builder.Append($"AND FechaAsignacion >= {DateTime.ParseExact(filters["FechaAsigDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaAsigHasta") && filters["FechaAsigHasta"].ToString() != "__/__/____")
            {
                var date = DateTime.ParseExact(filters["FechaAsigHasta"].ToString(), "dd/MM/yyyy", null);
                builder.Append($"AND FechaAsignacion <= {date.AddDays(1)}");
            }

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.TurnoSolicitud>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(TurnoSolicitudViewModel model)
        {
            var solicitud = _mapper.Map<TurnoSolicitud>(model);

            Validate(solicitud);

            var dbTurnoSolicitud = await _turnosSolicitudRepository.GetById<Paciente>(solicitud.IdTurnoSolicitud);

            if (dbTurnoSolicitud == null)
            {
                throw new ArgumentException("Solicitud inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                //dbTurnoSolicitud.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _turnosSolicitudRepository.Update(dbTurnoSolicitud, tran);

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

            throw new ForbiddenException();

            //var paciente = await _pacientesRepository.GetById<Paciente>(idPaciente);

            //if (paciente == null)
            //{
            //    throw new ArgumentException("Solicitud inexistente");
            //}

            //paciente.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            //await _pacientesRepository.Update(paciente);
        }

        public async Task<int> Solicitar(NuevaSolicitudViewModel model)
        {
            var newModel = new TurnoSolicitudViewModel
            {
                IdPaciente = model.IdPaciente,
                IdEspecialidad = model.IdEspecialidad,
                Dias = model.Dias,
                RangosHorarios = model.RangosHorarios,
                Observaciones = model.Observaciones
            };

            return await New(newModel);
        }

        public async Task<List<Custom.TurnoSolicitud>> GetTurnosPaciente()
            => await _turnosSolicitudRepository.GetTurnosByIdPaciente(_permissionsBusiness.Value.User.IdPaciente);

        public async Task Cancelar(CancelarViewModel model, Paciente paciente = null)
        {
            var turno = await _turnosSolicitudRepository.GetCustomById(model.IdTurnoSolicitud);
            if (turno == null)
            {
                throw new BusinessException("Solicitud inexistente");
            }

            if (paciente != null)
            {
                if (paciente.IdPaciente != turno.IdPaciente)
                {
                    throw new UnauthorizedAccessException("El turno no pertence al paciente.");
                }
            }
            else
            { 
                if (_permissionsBusiness.Value.User.IdPaciente > 0 && _permissionsBusiness.Value.User.IdPaciente != turno.IdPaciente)
                {
                    throw new UnauthorizedAccessException("El turno no pertence al paciente.");
                }
            }

            if (turno.IdEstadoTurnoSolicitud == (int)EstadoTurnoSolicitud.Cancelado)
            {
                throw new BusinessException("La solicitud no se encuentra en un estado válido.");
            }

            //TODO: Validaciones de cliente...
            var dbturno = await _turnosSolicitudRepository.GetById<TurnoSolicitud>(model.IdTurnoSolicitud);
            var tran = _uow.BeginTransaction();

            try
            {
                //Cancelo el turno
                dbturno.IdEstadoTurnoSolicitud = (int)EstadoTurnoSolicitud.Cancelado;
                dbturno.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _turnosSolicitudRepository.Update(dbturno, tran);

                tran.Commit();

                await _mailBusiness.EnviarMailSolicitudTurnoCanceladaPaciente(turno.Email,
                                                    turno.Paciente,
                                                    turno.Especialidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Asignar(AsignarViewModel model)
        {
            var turno = await _turnosSolicitudRepository.GetCustomById(model.IdTurnoSolicitud);
            if (turno == null)
            {
                throw new BusinessException("Solicitud inexistente");
            }

            if (_permissionsBusiness.Value.User.IdPaciente > 0 && _permissionsBusiness.Value.User.IdPaciente != turno.IdPaciente)
            {
                throw new UnauthorizedAccessException("El turno no pertence al paciente.");
            }

            if (turno.IdEstadoTurnoSolicitud == (int)EstadoTurno.Cancelado)
            {
                throw new BusinessException("La solicitud no se encuentra en un estado válido.");
            }

            //TODO: Validaciones de cliente...
            var dbturno = await _turnosSolicitudRepository.GetById<TurnoSolicitud>(model.IdTurnoSolicitud);
            var tran = _uow.BeginTransaction();

            try
            {
                //Asigno el turno
                dbturno.IdEstadoTurnoSolicitud = (int)EstadoTurnoSolicitud.Asignado;
                dbturno.FechaAsignacion = model.Fecha;
                dbturno.ObservacionesAsignacion = model.Observaciones;
                dbturno.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                dbturno.IdProfesional = model.IdProfesional > 0 ? model.IdProfesional : null;
                dbturno.IdUsuarioAsignacion = _permissionsBusiness.Value.User.Id;

                await _turnosSolicitudRepository.Update(dbturno, tran);

                tran.Commit();

                await _mailBusiness.EnviarMailTurnoAsignadoPaciente(turno.Email,
                                                            turno.Paciente,
                                                            model.Fecha?.ToString("dd/MM/yyyy HH:mm"),
                                                            turno.Especialidad,
                                                            model.Observaciones);

                await _chatApiBusiness.SendMessageSolicitudTurnoAsignado(turno.Telefono, model.Fecha.Value, turno.Paciente, turno.Especialidad, turno.IdPaciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Nuevo(NuevoViewModel model)
        {
            var paciente = await _turnosSolicitudRepository.GetById<Paciente>(model.IdPaciente);
            if (paciente == null )
            {
                throw new BusinessException("No se encontró el paciente.");
            }

            //TODO: Validaciones de cliente...
            var tran = _uow.BeginTransaction();

            try
            {
                var solicitud = new TurnoSolicitud
                {
                    IdPaciente = model.IdPaciente,
                    IdProfesional = model.IdProfesional > 0 ? model.IdProfesional : null,
                    FechaSolicitud = DateTime.Now,
                    Observaciones = "Solicitado de forma presencial",
                    FechaAsignacion = model.Fecha,
                    IdEspecialidad = model.IdEspecialidad,
                    IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                    IdEstadoTurnoSolicitud = (int)EstadoTurnoSolicitud.Asignado,
                    IdUsuarioAsignacion = _permissionsBusiness.Value.User.Id,
                    ObservacionesAsignacion = model.Observaciones
                };

                await _turnosSolicitudRepository.Insert(solicitud, tran);

                tran.Commit();

                var especialidad = (await _lookupsBusiness.Value.GetAllEspecialidades()).FirstOrDefault(f=> f.IdEspecialidad == model.IdEspecialidad).Descripcion;

                await _mailBusiness.EnviarMailTurnoAsignadoPaciente(paciente.Email,
                                                            paciente.Nombre,
                                                            model.Fecha?.ToString("dd/MM/yyyy HH:mm"),
                                                            especialidad,
                                                            model.Observaciones);

                await _chatApiBusiness.SendMessageSolicitudTurnoAsignado(paciente.Telefono, model.Fecha.Value, paciente.Nombre, especialidad, paciente.IdPaciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        private void Validate(TurnoSolicitud solicitud)
        {
            if (solicitud.IdPaciente == 0)
            {
                throw new BusinessException("Ingrese un Paciente válido.");
            }

            if (solicitud.IdEspecialidad == 0)
            {
                throw new BusinessException("Ingrese una Especialidad válida.");
            }
        }
    }
}
