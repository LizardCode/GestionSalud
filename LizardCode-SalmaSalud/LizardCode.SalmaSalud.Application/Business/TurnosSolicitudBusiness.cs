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

namespace LizardCode.SalmaSalud.Application.Business
{
    public class TurnosSolicitudBusiness : BaseBusiness, ITurnosSolicitudBusiness
    {
        private readonly ILogger<TurnosSolicitudBusiness> _logger;
        private readonly ITurnosSolicitudRepository _turnosSolicitudRepository;
        private readonly IPacientesRepository _pacientesRepository;
        private readonly IMailBusiness _mailBusiness;

        public TurnosSolicitudBusiness(
            ILogger<TurnosSolicitudBusiness> logger,
            IPacientesRepository pacientesRepository,
            ITurnosSolicitudRepository turnosSolicitudRepository,
            IMailBusiness mailBusiness)
        {
            _logger = logger;
            _turnosSolicitudRepository = turnosSolicitudRepository;
            _pacientesRepository = pacientesRepository;
            _mailBusiness = mailBusiness;
        }

        public async Task New(TurnoSolicitudViewModel model)
        {
            var solicitud = _mapper.Map<TurnoSolicitud>(model);

            Validate(solicitud);

            var tran = _uow.BeginTransaction();

            try
            {
                solicitud.FechaSolicitud = DateTime.Now;
                solicitud.IdEstadoTurnoSolicitud = (int)EstadoTurnoSolicitud.Solicitado;
                solicitud.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                var id = await _turnosSolicitudRepository.Insert(solicitud, tran);

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

        public async Task Solicitar(NuevaSolicitudViewModel model)
        {
            var newModel = new TurnoSolicitudViewModel
            {
                IdPaciente = model.IdPaciente,
                IdEspecialidad = model.IdEspecialidad,
                Dia = model.Dia,
                IdRangoHorario = model.IdRangoHorario,
                Observaciones = model.Observaciones
            };

            await New(newModel);
        }

        public async Task<List<Custom.TurnoSolicitud>> GetTurnosPaciente()
            => await _turnosSolicitudRepository.GetTurnosByIdPaciente(_permissionsBusiness.Value.User.IdPaciente);

        public async Task Cancelar(CancelarViewModel model)
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

                _mailBusiness.EnviarMailSolicitudTurnoCanceladaPaciente(turno.Email,
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
                //Cancelo el turno
                dbturno.IdEstadoTurnoSolicitud = (int)EstadoTurnoSolicitud.Asignado;
                dbturno.FechaAsignacion = model.Fecha;
                dbturno.ObservacionesAsignacion = model.Observaciones;
                dbturno.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _turnosSolicitudRepository.Update(dbturno, tran);

                tran.Commit();

                _mailBusiness.EnviarMailTurnoAsignadoPaciente(turno.Email,
                                                            turno.Paciente,
                                                            model.Fecha?.ToString("dd/MM/yyyy HH:mm"),
                                                            turno.Especialidad,
                                                            model.Observaciones);
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


            if (solicitud.Dia == 0 || solicitud.IdRangoHorario == 0)
            {
                throw new BusinessException("Ingrese un día y horario válido.");
            }
        }
    }
}
