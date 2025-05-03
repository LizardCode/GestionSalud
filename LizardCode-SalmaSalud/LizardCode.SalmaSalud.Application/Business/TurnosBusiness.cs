using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Common.Enums;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.FullCalendar;
using LizardCode.SalmaSalud.Application.Models.Reportes;
using LizardCode.SalmaSalud.Application.Models.Turnos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    internal class TurnosBusiness : BaseBusiness, ITurnosBusiness
    {
        private readonly ILogger<TurnosBusiness> _logger;
        private readonly ITurnosRepository _turnosRepository;
        private readonly ITurnosHistorialRepository _turnosHistorialRepository;
        private readonly IFeriadosRepository _feriadosRepository;
        private readonly IProfesionalesTurnosRepository _profesionalesTurnosRepository;
        private readonly IPacientesRepository _pacientesRepository;
        private readonly IConsultoriosRepository _consultoriosRepository;
        private readonly IClientesRepository _clientesRepository;
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly IFinanciadoresPadronRepository _financiadoresPadronRepository;

        private readonly IChatApiBusiness _chatApiBusiness;
        private readonly IMailBusiness _mailBusiness;

        public TurnosBusiness(
            ITurnosRepository turnosRepository,
            ITurnosHistorialRepository turnosHistorialRepository,
            IFeriadosRepository feriadosRepository,
            IProfesionalesTurnosRepository profesionalesTurnosRepository,
            ILogger<TurnosBusiness> logger,
            IPacientesRepository pacientesRepository,
            IChatApiBusiness chatApiBusiness,
            IConsultoriosRepository consultoriosRepository,
            IClientesRepository clientesRepository,
            IUsuariosRepository usuariosRepository,
            IFinanciadoresPadronRepository financiadoresPadronRepository,
            IMailBusiness mailBusiness)
        {
            _turnosRepository = turnosRepository;
            _turnosHistorialRepository = turnosHistorialRepository;
            _feriadosRepository = feriadosRepository;
            _profesionalesTurnosRepository = profesionalesTurnosRepository;
            _pacientesRepository = pacientesRepository;
            _consultoriosRepository = consultoriosRepository;
            _clientesRepository = clientesRepository;
            _usuariosRepository = usuariosRepository;
            _financiadoresPadronRepository = financiadoresPadronRepository;
            _logger = logger;
            _chatApiBusiness = chatApiBusiness;
            _mailBusiness = mailBusiness;
        }


        public async Task<DataTablesResponse<Custom.TurnoProfesional>> GetAll(DataTablesRequest request)
        {
            //var customQuery = _turnosRepository.GetProfesionalesDisponiblesByEspecialidad();
            var customQuery = _turnosRepository.GetProfesionalesDisponiblesByEspecialidad(DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(31).ToString("yyyy-MM-dd"), _permissionsBusiness.Value.User.IdEmpresa, 1, 0);
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            return await _dataTablesService.Resolve<Custom.TurnoProfesional>(request, customQuery.Sql, customQuery.Parameters, false, staticWhere: builder.Sql);
        }

        public async Task<List<Custom.Turno>> GetPrimerosTurnosDisponibles(int idEspecialidad, int idProfesional)
        {
            var turnosDisponibles = await _turnosRepository.GetPrimerosTurnosDisponibles(_permissionsBusiness.Value.User.IdEmpresa, DateTime.Now, idEspecialidad, idProfesional);

            return turnosDisponibles.ToList();
        }

        public async Task<List<TurnoCalendarioEvent>> GetTurnosDisponiblesPorDia(DateTime desde, DateTime hasta, int idEspecialidad, int idProfesional)
        {
            List<TurnoCalendarioEvent> events = new List<TurnoCalendarioEvent>();

            //Turnos... va devolver por día la cantidad de turnos...
            if (desde.DayOfYear == DateTime.Now.DayOfYear)
            {
                desde = desde.AddHours(DateTime.Now.Hour);
                desde = desde.AddMinutes(DateTime.Now.Minute);
            }
            var turnosDisponibles = await _turnosRepository.GetTurnosDisponiblesPorDia(_permissionsBusiness.Value.User.IdEmpresa, desde, hasta, idEspecialidad, idProfesional);
            if (turnosDisponibles != null && turnosDisponibles.Count > 0)
            {
                foreach (var turno in turnosDisponibles)
                {
                    events.Add(new TurnoCalendarioEvent
                    {
                        //IdProfesionalTurno = turno.IdProfesionalTurno,
                        ClassNames = "fc-event-custom fc-event-odonto",
                        Title = turno.Cantidad.ToString() + " TURNOS DISPONIBLES",
                        Start = turno.Fecha,
                        //End = turno.Fecha.ToString("yyyy-MM-ddTHH:mm:ss"),
                        AllDay = true,
                        Disponibles = turno.Cantidad
                    });
                }
            }

            //Feriados / Eventos
            //TODO: Hacer con rango de fechas... no "ALL"...
            var feriadosEventos = await _feriadosRepository.GetAllByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa);
            if (feriadosEventos != null && feriadosEventos.Count > 0)
            {
                foreach (var fe in feriadosEventos)
                {
                    events.Add(new TurnoCalendarioEvent
                    {
                        ClassNames = "fc-event-custom fc-event-warning",
                        Title = fe.Nombre.ToUpperInvariant(),
                        Start = fe.Fecha.ToString("yyyy-MM-ddTHH:mm:ss"),
                        Display = "background",
                        AllDay = true
                    });
                }

            }

            while (desde.Date <= hasta.Date)
            {
                var sDesde = desde.ToString("yyyy-MM-dd");
                if (events.FirstOrDefault(f => f.Start == sDesde && f.Disponibles > 0) == null)
                {
                    events.Add(new TurnoCalendarioEvent
                    {
                        ClassNames = "fc-event-custom-no-click fc-event-gris-oscuro",
                        Title = "SIN TURNOS DISPONIBLES",
                        Start = sDesde,
                        AllDay = true,
                        Disponibles = 0
                    });
                    events.Add(new TurnoCalendarioEvent
                    {
                        ClassNames = "fc-event-custom-no-click fc-event-gris-claro",
                        Title = "",
                        Start = sDesde,
                        Display = "background",
                        AllDay = true,
                        Disponibles = 0
                    });
                }

                desde = desde.AddDays(1);
            }

            return events;
        }

        public async Task<List<Custom.Turno>> GetTurnosDisponibles(DateTime fecha, int idEspecialidad, int idProfesional)
        {
            if (fecha.DayOfYear == DateTime.Now.DayOfYear)
            {
                fecha = fecha.AddHours(DateTime.Now.Hour);
                fecha = fecha.AddMinutes(DateTime.Now.Minute);
            }
            var turnosDisponibles = await _turnosRepository.GetTurnosDisponibles(_permissionsBusiness.Value.User.IdEmpresa, fecha, idEspecialidad, idProfesional);

            return turnosDisponibles.ToList();
        }

        public async Task<DataTablesResponse<Custom.Turno>> GetTurnosHoy(DataTablesRequest request)
        {

            var customQuery = _turnosRepository.GetTurnos(null);
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            builder.Append($"AND FechaInicio >= {DateTime.ParseExact(DateTime.Now.Date.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null)}");
            builder.Append($"AND FechaInicio <= {DateTime.ParseExact(DateTime.Now.Date.AddDays(1).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null)}");

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Turno>(request, customQuery.Sql, customQuery.Parameters, false, staticWhere: builder.Sql);
        }

        public async Task<DataTablesResponse<Custom.Turno>> GetTurnosProximos(DataTablesRequest request)
        {

            var customQuery = _turnosRepository.GetTurnos(null);
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            builder.Append($"AND FechaInicio >= {DateTime.ParseExact(DateTime.Now.Date.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null)}");
            builder.Append($"AND FechaInicio <= {DateTime.ParseExact(DateTime.Now.Date.AddDays(90).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null)}");
            builder.Append($"AND IdEstadoTurno IN ({(int)EstadoTurno.Agendado}, {(int)EstadoTurno.ReAgendado}, {(int)EstadoTurno.Confirmado})");
            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Turno>(request, customQuery.Sql, customQuery.Parameters, false, staticWhere: builder.Sql);
        }

        public async Task<DataTablesResponse<Custom.Turno>> GetTurnos(DataTablesRequest request)
        {

            var customQuery = _turnosRepository.GetTurnos(null);
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("IdTipoTurno"))
                builder.Append($"AND IdTipoTurno = {filters["IdTipoTurno"]}");

            if (filters.ContainsKey("IdEspecialidad"))
                builder.Append($"AND IdEspecialidad = {filters["IdEspecialidad"]}");

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND FechaInicio >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____") {
                var date = DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null);
                builder.Append($"AND FechaInicio <= {date.AddDays(1)}");
            }

            if (filters.ContainsKey("IdProfesional"))
                builder.Append($"AND IdProfesional = {filters["IdProfesional"]}");

            if (_permissionsBusiness.Value.User.IdPaciente > 0)
                builder.Append($"AND IdPaciente = {_permissionsBusiness.Value.User.IdPaciente}");

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Turno>(request, customQuery.Sql, customQuery.Parameters, false, staticWhere: builder.Sql);
        }

        public async Task<DataTablesResponse<Custom.Turno>> GetSalaEspera(DataTablesRequest request)
        {

            var customQuery = _turnosRepository.GetSalaEspera(null);
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("IdEspecialidad"))
                builder.Append($"AND IdEspecialidad = {filters["IdEspecialidad"]}");

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND FechaInicio >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
            {
                var date = DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null);
                builder.Append($"AND FechaInicio <= {date.AddDays(1)}");
            }

            if (_permissionsBusiness.Value.User.IdProfesional > 0)
            {
                builder.Append($"AND IdProfesional = {_permissionsBusiness.Value.User.IdProfesional}");
            }
            else
            { 
                if (filters.ContainsKey("IdProfesional"))
                    builder.Append($"AND IdProfesional = {filters["IdProfesional"]}");
            }

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            builder.Append($"AND IdEstadoTurno = {(int)EstadoTurno.Recepcionado}");

            builder.Append($"AND IdEspecialidad IS NOT NULL ");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Turno>(request, customQuery.Sql, customQuery.Parameters, false, staticWhere: builder.Sql);
        }

        public async Task<DataTablesResponse<Custom.Turno>> GetTurnosReAgendar(int idEspecialidad, int idProfesional, DataTablesRequest request)
        {
            //var filters = request.ParseFilters();

            var customQuery = _turnosRepository.GetTurnosReagendar(_permissionsBusiness.Value.User.IdEmpresa, idEspecialidad, idProfesional, null);
            var builder = _dbContext.Connection.QueryBuilder();

            return await _dataTablesService.Resolve<Custom.Turno>(request, customQuery.Sql, customQuery.Parameters, false, staticWhere: builder.Sql);
        }


        public async Task Agendar(int idProfesionalTurno, int idPaciente)
        {
            var pTurno = await _profesionalesTurnosRepository.GetById<ProfesionalTurno>(idProfesionalTurno);
            if (pTurno == null)
            {
                throw new BusinessException("Turno inexistente");
            }

            if (pTurno.IdEstadoProfesionalTurno != (int)EstadoProfesionalTurno.Disponible)
            {
                throw new BusinessException("El turno ya no se encuentra disponible.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                pTurno.IdEstadoProfesionalTurno = (int)EstadoProfesionalTurno.Agendado;
                await _profesionalesTurnosRepository.Update(pTurno, tran);

                var idTurno = await _turnosRepository.Insert(new Turno
                {
                    
                    IdEspecialidad = pTurno.IdEspecialidad,
                    IdEmpresa = pTurno.IdEmpresa,
                    IdProfesional = pTurno.IdProfesional,
                    FechaInicio = pTurno.FechaInicio,
                    FechaFin = pTurno.FechaFin,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    IdProfesionalTurno = idProfesionalTurno,
                    IdEstadoTurno = (int)EstadoTurno.Agendado,
                    IdPaciente = idPaciente        
                }, tran);

                await _turnosHistorialRepository.Insert(new TurnoHistorial()
                {
                    IdTurno = (int)idTurno,
                    Fecha = DateTime.Now,
                    IdEstadoTurno = (int)EstadoTurno.Agendado,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    Observaciones = string.Empty
                }, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }
        public async Task Asignar(AsignarViewModel model, bool noActualizarPaciente = false)
        {
            //throw new BusinessException("Probandolo.");

            var pTurno = await _profesionalesTurnosRepository.GetById<ProfesionalTurno>(model.IdProfesionalTurno);
            if (pTurno == null)
            {
                throw new BusinessException("Turno inexistente");
            }

            if (pTurno.FechaInicio < DateTime.Now)
            {
                throw new BusinessException("La fecha del turno es anterior a la fecha actual. El Turno ya no puede ser asigando.");
            }

            if (pTurno.IdEstadoProfesionalTurno != (int)EstadoProfesionalTurno.Disponible)
            {
                throw new BusinessException("El turno ya no se encuentra disponible.");
            }

            var especialidad = (await _lookupsBusiness.Value.GetAllEspecialidades()).FirstOrDefault(f => f.IdEspecialidad == pTurno.IdEspecialidad);
            var profesional = (await _lookupsBusiness.Value.GetAllProfesionales(_permissionsBusiness.Value.User.IdEmpresa)).FirstOrDefault(f => f.IdProfesional == pTurno.IdProfesional);

            Paciente paciente = null;
            if (model.IdPaciente > 0)
            {
                paciente = await _pacientesRepository.GetById<Paciente>(model.IdPaciente);
                if (paciente == null)
                { 
                    throw new BusinessException("No se encontró el paciente.");
                }

                var turnos = await _turnosRepository.GetTurnosByIdPaciente(model.IdPaciente);
                if (turnos.Count(t => t.IdEstadoTurno == (int)EstadoTurno.Agendado) > 1)
                {
                    throw new BusinessException("El paciente tiene 2 turnos agendados. No se pueden agendar mas turnos.");
                }
            }

            //TODO: Validaciones de cliente...
            var tran = _uow.BeginTransaction();

            try
            {
                long idPaciente = 0;
                if (model.IdPaciente == 0)
                {
                    //Creo un paciente nuevo
                    paciente = new Paciente
                    {
                        Nombre = model.Nombre.ToUpperInvariant(),
                        Documento = model.Documento,
                        Email = model.Email,
                        IdTipoTelefono = (int)TipoTelefono.Movil, //model.IdTipoTelefono,
                        Telefono = model.Telefono,
                        IdFinanciador = model.SinCobertura ? null : model.IdFinanciador,
                        IdFinanciadorPlan = model.SinCobertura ? null : model.IdFinanciadorPlan,
                        FinanciadorNro = model.SinCobertura ? null : model.FinanciadorNro,
                        FechaNacimiento = model.FechaNacimiento,
                        Nacionalidad = model.Nacionalidad,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo

                    };

                    idPaciente = await NewPacienteCliente(paciente, tran);
                }
                else
                {                    
                    idPaciente = model.IdPaciente;

                    if (!noActualizarPaciente) 
                    { 
                        paciente.Email = model.Email;
                        paciente.IdTipoTelefono = (int)TipoTelefono.Movil;
                        paciente.Telefono = model.Telefono;
                        paciente.IdFinanciador = model.SinCobertura ? null : model.IdFinanciador;
                        paciente.IdFinanciadorPlan = model.SinCobertura ? null : model.IdFinanciadorPlan;
                        paciente.FinanciadorNro = model.SinCobertura ? null : model.FinanciadorNro;
                        paciente.FechaNacimiento = model.FechaNacimiento;
                        paciente.Nacionalidad = model.Nacionalidad;

                        await UpdatePacienteCliente(paciente, tran);
                    }
                }

                pTurno.IdEstadoProfesionalTurno = (int)EstadoProfesionalTurno.Agendado;
                await _profesionalesTurnosRepository.Update(pTurno, tran);

                var idTurno = await _turnosRepository.Insert(new Turno
                {
                    IdTipoTurno = (int)TipoTurno.Turno,
                    IdEspecialidad = pTurno.IdEspecialidad,
                    IdEmpresa = pTurno.IdEmpresa,
                    IdProfesional = pTurno.IdProfesional,
                    FechaInicio = pTurno.FechaInicio,
                    FechaFin = pTurno.FechaFin,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    IdProfesionalTurno = model.IdProfesionalTurno,
                    IdEstadoTurno = (int)EstadoTurno.Agendado,
                    IdPaciente = (int)idPaciente,
                    Observaciones = model.Observaciones
                }, tran);

                await _turnosHistorialRepository.Insert(new TurnoHistorial()
                {
                    IdTurno = (int)idTurno,
                    Fecha = DateTime.Now,
                    IdEstadoTurno = (int)EstadoTurno.Agendado,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    Observaciones = string.Empty
                }, tran);

                tran.Commit();

                await _chatApiBusiness.SendMessageTurnoAsignado(paciente.Telefono, 
                                                                pTurno.FechaInicio, 
                                                                paciente.Nombre,
                                                                profesional.Nombre, 
                                                                especialidad.Descripcion, 
                                                                paciente.IdPaciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Cancelar(CancelarViewModel model)
        {
            var turno = await _turnosRepository.GetByIdCustom(model.IdTurno);
            if (turno == null)
            {
                throw new BusinessException("Turno inexistente");
            }

            if (_permissionsBusiness.Value.User.IdPaciente > 0 && _permissionsBusiness.Value.User.IdPaciente != turno.IdPaciente)
            {
                throw new UnauthorizedAccessException("El turno no pertence al paciente.");
            }

            if (turno.IdEstadoTurno == (int)EstadoTurno.AusenteConAviso
                || turno.IdEstadoTurno == (int)EstadoTurno.AusenteSinAviso
                || turno.IdEstadoTurno == (int)EstadoTurno.Cancelado
                || turno.IdEstadoTurno == (int)EstadoTurno.EnConsultorio
                || turno.IdEstadoTurno == (int)EstadoTurno.Atendido)
            {
                throw new BusinessException("El turno no se encuentra en un estado válido.");
            }

            //TODO: Validaciones de cliente...
            var dbturno = await _turnosRepository.GetById<Turno>(model.IdTurno);
            var tran = _uow.BeginTransaction();

            try
            {
                //Libero el turno original
                if (dbturno.IdProfesionalTurno > 0)
                { 
                    var dbProfesionalTurno = await _profesionalesTurnosRepository.GetById<ProfesionalTurno>(dbturno.IdProfesionalTurno.Value, tran);
                    dbProfesionalTurno.IdEstadoProfesionalTurno = (int)EstadoProfesionalTurno.Disponible;
                    await _profesionalesTurnosRepository.Update(dbProfesionalTurno, tran);
                }
                //Cancelo el turno
                dbturno.IdProfesionalTurno = null;
                dbturno.IdEstadoTurno = (int)EstadoTurno.AusenteConAviso;
                await _turnosRepository.Update(dbturno, tran);

                await _turnosHistorialRepository.Insert(new TurnoHistorial()
                {
                    IdTurno = (int)turno.IdTurno,
                    Fecha = DateTime.Now,
                    IdEstadoTurno = (int)EstadoTurno.AusenteConAviso,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    Observaciones = model.Observaciones
                }, tran);

                tran.Commit();

                if (turno.IdTipoTurno == (int)TipoTurno.Guardia)
                {
                    await _chatApiBusiness.SendMessageTurnoGuardiaCancelado(turno.Telefono,
                                                        turno.Paciente,
                                                        model.Observaciones,
                                                        turno.IdPaciente);
                }
                else
                { 
                    await _chatApiBusiness.SendMessageTurnoCancelado(turno.Telefono,
                                                        turno.FechaInicio,
                                                        turno.Paciente,
                                                        turno.Profesional,
                                                        turno.Especialidad,
                                                        turno.IdPaciente);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Confirmar(int idTurno)
        {
            var turno = await _turnosRepository.GetByIdCustom(idTurno);
            if (turno == null)
            {
                throw new BusinessException("Turno inexistente");
            }

            if (_permissionsBusiness.Value.User.IdPaciente > 0 && _permissionsBusiness.Value.User.IdPaciente != turno.IdPaciente)
            {
                throw new UnauthorizedAccessException("El turno no pertence al paciente.");
            }

            if (turno.FechaInicio < DateTime.Now)
            {
                throw new BusinessException("La fecha del turno es anterior a la fecha actual. El Turno ya no puede ser confirmado.");
            }

            if (turno.IdEstadoTurno != (int)EstadoTurno.Agendado)
            {
                throw new BusinessException("El turno no se encuentra en un estado válido.");
            }

            //TODO: Validaciones de cliente...
            var dbturno = await _turnosRepository.GetById<Turno>(idTurno);
            var tran = _uow.BeginTransaction();

            try
            {
                dbturno.FechaConfirmacion = DateTime.Now;
                dbturno.IdEstadoTurno = (int)EstadoTurno.Confirmado;
                await _turnosRepository.Update(dbturno, tran);

                await _turnosHistorialRepository.Insert(new TurnoHistorial()
                {
                    IdTurno = (int)turno.IdTurno,
                    Fecha = DateTime.Now,
                    IdEstadoTurno = (int)EstadoTurno.Confirmado,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    Observaciones = string.Empty
                }, tran);

                tran.Commit();

                await _chatApiBusiness.SendMessageTurnoConfirmado(turno.Telefono,
                                                    turno.FechaInicio,
                                                    turno.Paciente,
                                                    turno.Profesional,
                                                    turno.Especialidad,
                                                    turno.IdPaciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Recepcionar(RecepcionarViewModel model)
        {
            var turno = await _turnosRepository.GetByIdCustom(model.IdTurno);
            if (turno == null)
            {
                throw new BusinessException("Turno inexistente");
            }

            //if (turno.FechaInicio.Date < DateTime.Now.Date)
            //{
            //    throw new BusinessException("La fecha del turno es anterior a la fecha actual. El Turno no puede ser recepcionado.");
            //}

            //if (turno.FechaInicio.Date > DateTime.Now.Date)
            //{
            //    throw new BusinessException("La fecha del turno es posterir a la fecha actual. El Turno no puede ser recepcionado.");
            //}

            if (turno.IdEstadoTurno != (int)EstadoTurno.Confirmado
                && turno.IdEstadoTurno != (int)EstadoTurno.Agendado
                && turno.IdEstadoTurno != (int)EstadoTurno.ReAgendado)
            {
                throw new BusinessException("El turno no se encuentra en un estado válido.");
            }

            var consultorio = await _consultoriosRepository.GetById<Consultorio>(model.IdConsultorio);
            if (consultorio == null)
            {
                throw new BusinessException("Consultorio inexistente");
            }

            //TODO: Validaciones de cliente...
            var dbturno = await _turnosRepository.GetById<Turno>(model.IdTurno);
            var tran = _uow.BeginTransaction();

            try
            {
                if (!model.ForzarParticular && !string.IsNullOrEmpty(model.FinanciadorNro))
                {
                    var dbPaciente = await _pacientesRepository.GetById<Paciente>(model.IdPaciente, tran);
                    dbPaciente.FinanciadorNro = model.FinanciadorNro;

                    await _pacientesRepository.Update(dbPaciente, tran);
                }

                if (model.ForzarPadron)
                {
                    var dbPaciente = await _pacientesRepository.GetById<Paciente>(model.IdPaciente, tran);
                    if (dbPaciente.IdFinanciador > 0)
                    {
                        await _financiadoresPadronRepository.Insert(new FinanciadorPadron
                        {
                            IdFinanciador = dbPaciente.IdFinanciador.Value,
                            Documento = dbPaciente.Documento,
                            FinanciadorNro = dbPaciente.FinanciadorNro,
                            Fecha = DateTime.Now,
                            Nombre = dbPaciente.Nombre
                        }, tran);
                    }
                }

                dbturno.FechaRecepcion = DateTime.Now;
                dbturno.Consultorio = consultorio.Descripcion;
                dbturno.IdConsultorio = model.IdConsultorio;
                dbturno.IdEstadoTurno = (int)EstadoTurno.Recepcionado;
                dbturno.ForzarParticular = model.ForzarParticular;
                await _turnosRepository.Update(dbturno, tran);

                await _turnosHistorialRepository.Insert(new TurnoHistorial()
                {
                    IdTurno = (int)turno.IdTurno,
                    Fecha = DateTime.Now,
                    IdEstadoTurno = (int)EstadoTurno.Recepcionado,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    Observaciones = string.Empty
                }, tran);

                tran.Commit();

                await _chatApiBusiness.SendMessageTurnoRecepcionado(turno.Telefono, 
                                                                    dbturno.Consultorio, 
                                                                    turno.Paciente, 
                                                                    turno.Profesional, 
                                                                    turno.Profesional, 
                                                                    turno.IdPaciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task ReAgendar(int idTurno, int idProfesionalTurno)
        {
            var turno = await _turnosRepository.GetByIdCustom(idTurno);
            if (turno == null)
            {
                throw new BusinessException("Turno inexistente");
            }

            if (turno.IdEstadoTurno == (int)EstadoTurno.Cancelado
                || turno.IdEstadoTurno == (int)EstadoTurno.AusenteConAviso
                || turno.IdEstadoTurno == (int)EstadoTurno.AusenteSinAviso
                || turno.IdEstadoTurno == (int)EstadoTurno.EnConsultorio
                || turno.IdEstadoTurno == (int)EstadoTurno.Atendido)
            {
                throw new BusinessException("El turno no se encuentra en un estado válido.");
            }

            //TODO: Validaciones de cliente...
            var dbturno = await _turnosRepository.GetById<Turno>(idTurno);
            var tran = _uow.BeginTransaction();

            try
            {
                //Libero el turno original
                var dbProfesionalTurno = await _profesionalesTurnosRepository.GetById<ProfesionalTurno>(dbturno.IdProfesionalTurno.Value, tran);
                dbProfesionalTurno.IdEstadoProfesionalTurno = (int)EstadoProfesionalTurno.Disponible;
                await _profesionalesTurnosRepository.Update(dbProfesionalTurno, tran);

                //Tomo el nuevo turno
                var dbProfesionalTurnoNew = await _profesionalesTurnosRepository.GetById<ProfesionalTurno>(idProfesionalTurno, tran);
                dbProfesionalTurnoNew.IdEstadoProfesionalTurno = (int)EstadoProfesionalTurno.Agendado;
                await _profesionalesTurnosRepository.Update(dbProfesionalTurnoNew, tran);

                //Piso lo correspondiente en el turno
                dbturno.IdProfesional = dbProfesionalTurnoNew.IdProfesional;
                dbturno.FechaInicio = dbProfesionalTurnoNew.FechaInicio;
                dbturno.FechaFin = dbProfesionalTurnoNew.FechaFin;
                dbturno.IdProfesionalTurno = dbProfesionalTurnoNew.IdProfesionalTurno;
                dbturno.IdUsuario = _permissionsBusiness.Value.User.Id;
                dbturno.IdEstadoTurno = (int)EstadoTurno.ReAgendado;
                dbturno.FechaConfirmacion = null;
                dbturno.FechaRecepcion = null;
                dbturno.IdConsultorio = null;
                dbturno.Consultorio = null;
                await _turnosRepository.Update(dbturno, tran);

                await _turnosHistorialRepository.Insert(new TurnoHistorial()
                {
                    IdTurno = (int)turno.IdTurno,
                    Fecha = DateTime.Now,
                    IdEstadoTurno = (int)EstadoTurno.ReAgendado,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    Observaciones = string.Format("Turno RE-AGENDADO del {0} al {1}", dbProfesionalTurno.FechaInicio, dbturno.FechaInicio)
                }, tran);

                tran.Commit();

                await _chatApiBusiness.SendMessageTurnoReAgendado(turno.Telefono, 
                                                                    dbturno.FechaInicio, 
                                                                    turno.Paciente, 
                                                                    turno.Profesional, 
                                                                    turno.Especialidad, 
                                                                    turno.IdPaciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Llamar(LlamarViewModel model)
        {
            var turno = await _turnosRepository.GetByIdCustom(model.IdTurno);
            if (turno == null)
            {
                throw new BusinessException("Turno inexistente");
            }

            if (turno.IdEstadoTurno != (int)EstadoTurno.Recepcionado)
            {
                throw new BusinessException("El turno no se encuentra en un estado válido.");
            }

            var consultorio = await _consultoriosRepository.GetById<Consultorio>(model.IdConsultorio);
            if (consultorio == null)
            {
                throw new BusinessException("Consultorio inexistente.");
            }

            var profesional = (await _lookupsBusiness.Value.GetAllProfesionales(_permissionsBusiness.Value.User.IdEmpresa)).FirstOrDefault(f => f.IdProfesional == _permissionsBusiness.Value.User.IdProfesional);
            if (profesional == null)
            {
                throw new BusinessException("Profesional inexistente.");
            }

            await _chatApiBusiness.SendMessageLlamar(turno.Telefono,
                                                        consultorio.Descripcion,
                                                        turno.Paciente,
                                                        profesional.Nombre,
                                                        turno.IdPaciente);
        }

        public async Task<Custom.Turno> GetCustomById(int idTurno)
        {
            return await _turnosRepository.GetByIdCustom(idTurno);
        }

        public async Task<Custom.TurnosTotales> ObtenerTotalesByEstado()
            => await _turnosRepository.GetTotalesByEstado(_permissionsBusiness.Value.User.IdEmpresa, _permissionsBusiness.Value.User.IdProfesional, _permissionsBusiness.Value.User.IdPaciente);


        public async Task<List<ProfesionalTurnoEvent>> GetAgendaSobreTurnos(DateTime desde, DateTime hasta, int idProfesional)
        {
            List<ProfesionalTurnoEvent> events = new List<ProfesionalTurnoEvent>();

            var profesional = (await _lookupsBusiness.Value.GetAllProfesionales(_permissionsBusiness.Value.User.IdEmpresa)).FirstOrDefault(f => f.IdProfesional == idProfesional);
            var especialidad = (await _lookupsBusiness.Value.GetAllEspecialidades()).FirstOrDefault(f => f.IdEspecialidad == profesional.IdEspecialidad);

            var empresa = (await _lookupsBusiness.Value.GetAllEmpresasLookup()).FirstOrDefault(f => f.IdEmpresa == _permissionsBusiness.Value.User.IdEmpresa);
            var hDesde = empresa.TurnosHoraInicio.Split(":");
            var hHasta = empresa.TurnosHoraFin.Split(":");

            var iIntervaloTrunos = profesional.TurnosIntervalo > 0 ? profesional.TurnosIntervalo : (especialidad.TurnosIntervalo == 0 ? empresa.TurnosIntervalo : especialidad.TurnosIntervalo);
            var sIntervaloTrunos = iIntervaloTrunos.ToString();

            var horaDesde = int.Parse(hDesde[0]);
            var horaHasta = int.Parse(hHasta[0]);

            desde = desde.AddHours(int.Parse(hDesde[0])).AddMinutes(int.Parse(hDesde[1]));
            hasta = hasta.AddHours(int.Parse(hHasta[0])).AddMinutes(int.Parse(hHasta[1]));

            //Sobre-Turnos DADOS
            var sobreTurnos = await _turnosRepository.GetSobreTurnos(desde, hasta, _permissionsBusiness.Value.User.IdEmpresa, idProfesional);
            if (sobreTurnos != null && sobreTurnos.Count > 0)
            {
                foreach (var sobreTurno in sobreTurnos)
                {
                    var paciente = await _pacientesRepository.GetById<Paciente>(sobreTurno.IdPaciente);
                    events.Add(new ProfesionalTurnoEvent
                    {
                        IdProfesionalTurno = 0,
                        Fecha = sobreTurno.FechaInicio.ToString("yyyy-MM-ddTHH:mm:ss"), //agrego un segundo para que aparezca.. .segundo?
                        ClassNames = "fc-event-custom fc-event-naranja",
                        Title = "S-TURNO",
                        Start = sobreTurno.FechaInicio.AddSeconds(1).ToString("yyyy-MM-ddTHH:mm:ss"),
                        End = sobreTurno.FechaFin.ToString("yyyy-MM-ddTHH:mm:ss"),
                        AllDay = false,

                        Paciente = paciente.Nombre.ToUpperInvariant(),
                        Descripcion = sobreTurno.Observaciones ?? ""
                    });
                }
            }

            //Turnos AGENDABLES / AGENDADOS
            var turnos = await _profesionalesTurnosRepository.GetAllByIdProfesionalAndIdEmpresa(desde, hasta, idProfesional, _permissionsBusiness.Value.User.IdEmpresa);
            if (turnos != null && turnos.Count > 0)
            {
                foreach (var turno in turnos)
                {
                    var tCustom = await _turnosRepository.GetByIdProfesionalTurnoCustom(turno.IdProfesionalTurno);
                    events.Add(new ProfesionalTurnoEvent
                    {
                        //Si ya hay unn sobreturno apra ese día, no se puede dar uno nuevo...
                        IdProfesionalTurno = sobreTurnos.Any(st => st.FechaInicio == turno.FechaInicio) ? 0 : turno.IdProfesionalTurno,
                        Fecha = turno.FechaInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                        ClassNames = turno.IdEstadoProfesionalTurno == (int)EstadoProfesionalTurno.Disponible ? "fc-event-custom fc-event-celeste" : "fc-event-custom fc-event-verde-claro",
                        Title = turno.IdEstadoProfesionalTurno == (int)EstadoProfesionalTurno.Disponible ? "DISPONIBLE" : "TURNO",
                        Start = turno.FechaInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                        End = turno.FechaFin.ToString("yyyy-MM-ddTHH:mm:ss"),
                        AllDay = false,

                        Paciente = tCustom?.Paciente ?? "",
                        Descripcion = tCustom?.Observaciones ?? ""
                    });
                }
            }

            //Feriados/Eventos
            //TODO: Hacer con rango de fechas... no "ALL"...
            var feriadosEventos = await _feriadosRepository.GetAllByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa);
            if (feriadosEventos != null && feriadosEventos.Count > 0)
            {
                foreach (var fe in feriadosEventos)
                {
                    events.Add(new ProfesionalTurnoEvent
                    {
                        Fecha = fe.Fecha.ToString("yyyy-MM-ddTHH:mm:ss"),
                        Clickable = false,
                        IdProfesionalTurno = -1,
                        ClassNames = "fc-event-custom fc-event-warning",
                        Title = fe.Nombre.ToUpperInvariant(),
                        Start = fe.Fecha.ToString("yyyy-MM-ddTHH:mm:ss"),
                        //End = fe.Fecha.ToString("yyyy-MM-ddTHH:mm:ss"),
                        Display = "background",
                        AllDay = true
                    });
                }

            }

            return events;
        }
        public async Task AsignarSobreTurno(AsignarSobreTurnoViewModel model, bool noActualizarPaciente = false)
        {
            //throw new BusinessException("Probandolo.");

            var pTurno = await _profesionalesTurnosRepository.GetById<ProfesionalTurno>(model.IdProfesionalTurno);
            if (pTurno == null)
            {
                throw new BusinessException("Turno inexistente");
            }

            var especialidad = (await _lookupsBusiness.Value.GetAllEspecialidades()).FirstOrDefault(f => f.IdEspecialidad == pTurno.IdEspecialidad);
            var profesional = (await _lookupsBusiness.Value.GetAllProfesionales(_permissionsBusiness.Value.User.IdEmpresa)).FirstOrDefault(f => f.IdProfesional == pTurno.IdProfesional);

            //Validar cantidad de sobreturnos...
            var sobreTurnos = await _turnosRepository.GetSobreTurnos(pTurno.FechaInicio.Date, pTurno.FechaInicio.Date.AddHours(23).AddMinutes(59), _permissionsBusiness.Value.User.IdEmpresa, pTurno.IdProfesional);
            if (sobreTurnos != null && sobreTurnos.Count >= profesional.CantidadSobreturnos)
            {
                throw new BusinessException(string.Format("Ya se ha alcanzado la cantidad máxima de Sobre-Turnos para el profesional (cant. max: {0}). ", profesional.CantidadSobreturnos));
            }

            Paciente paciente = null;
            if (model.IdPaciente > 0)
            {
                paciente = await _pacientesRepository.GetById<Paciente>(model.IdPaciente);
                if (paciente == null)
                {
                    throw new BusinessException("No se encontró el paciente.");
                }

                var turnos = await _turnosRepository.GetTurnosByIdPaciente(model.IdPaciente);
                if (turnos.Count(t => t.IdEstadoTurno == (int)EstadoTurno.Agendado) > 1)
                {
                    throw new BusinessException("El paciente tiene 2 turnos agendados. No se pueden agendar mas turnos.");
                }
            }

            //TODO: Validaciones de cliente...
            var tran = _uow.BeginTransaction();

            try
            {
                long idPaciente = 0;
                if (model.IdPaciente == 0)
                {
                    //Creo un paciente nuevo
                    paciente = new Paciente
                    {
                        Nombre = model.Nombre.ToUpperInvariant(),
                        Documento = model.Documento,
                        Email = model.Email,
                        IdTipoTelefono = (int)TipoTelefono.Movil, //model.IdTipoTelefono,
                        Telefono = model.Telefono,
                        IdFinanciador = model.SinCobertura ? null : model.IdFinanciador,
                        IdFinanciadorPlan = model.SinCobertura ? null : model.IdFinanciadorPlan,
                        FinanciadorNro = model.SinCobertura ? null : model.FinanciadorNro,
                        FechaNacimiento = model.FechaNacimiento,
                        Nacionalidad = model.Nacionalidad,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo
                    };

                    idPaciente = await NewPacienteCliente(paciente, tran);
                }
                else
                {
                    idPaciente = model.IdPaciente;

                    if (!noActualizarPaciente)
                    {
                        paciente.Email = model.Email;
                        paciente.IdTipoTelefono = (int)TipoTelefono.Movil;
                        paciente.Telefono = model.Telefono;
                        paciente.IdFinanciador = model.SinCobertura ? null : model.IdFinanciador;
                        paciente.IdFinanciadorPlan = model.SinCobertura ? null : model.IdFinanciadorPlan;
                        paciente.FinanciadorNro = model.SinCobertura ? null : model.FinanciadorNro;
                        paciente.FechaNacimiento = model.FechaNacimiento;
                        paciente.Nacionalidad = model.Nacionalidad;

                        await UpdatePacienteCliente(paciente, tran);
                    }
                }

                var idTurno = await _turnosRepository.Insert(new Turno
                {
                    IdTipoTurno = (int)TipoTurno.SobreTurno,
                    IdEspecialidad = pTurno.IdEspecialidad,
                    IdEmpresa = pTurno.IdEmpresa,
                    IdProfesional = pTurno.IdProfesional,
                    FechaInicio = pTurno.FechaInicio,
                    FechaFin = pTurno.FechaFin,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    IdProfesionalTurno = null, //model.IdProfesionalTurno,
                    IdEstadoTurno = (int)EstadoTurno.Agendado,
                    IdPaciente = (int)idPaciente,
                    Observaciones = model.Observaciones
                }, tran);

                await _turnosHistorialRepository.Insert(new TurnoHistorial()
                {
                    IdTurno = (int)idTurno,
                    Fecha = DateTime.Now,
                    IdEstadoTurno = (int)EstadoTurno.Agendado,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    Observaciones = string.Empty
                }, tran);

                tran.Commit();

                await _chatApiBusiness.SendMessageSobreTurnoAsignado(paciente.Telefono, 
                                                                    pTurno.FechaInicio, 
                                                                    paciente.Nombre, 
                                                                    profesional.Nombre, 
                                                                    especialidad.Descripcion, 
                                                                    paciente.IdPaciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task CancelarAgenda(CancelarAgendaViewModel model)
        {
            if (model.FechaCancelacion.Date < DateTime.Now.Date) 
            {
                throw new BusinessException("La fecha de cancelación no puede ser anterior a la fecha actual.");
            }

            if (model.IdProfesional == 0)
            {
                throw new BusinessException("No se ingresó el profesional para cancelar agenda.");
            }

            //TODO: Validaciones de cliente...
            var tran = _uow.BeginTransaction();

            try
            {
                var turnos = await _turnosRepository.GetTurnosByFecha(_permissionsBusiness.Value.User.IdEmpresa, model.FechaCancelacion.Date, model.IdProfesional, tran);
                if (turnos != null && turnos.Count > 0)
                {

                    foreach (var turno in turnos)
                    {
                        var dbturno = await _turnosRepository.GetById<Turno>(turno.IdTurno, tran);

                        //Libero el turno original
                        //if (dbturno.IdProfesionalTurno > 0)
                        //{
                        //    var dbProfesionalTurno = await _profesionalesTurnosRepository.GetById<ProfesionalTurno>(dbturno.IdProfesionalTurno.Value, tran);
                        //    dbProfesionalTurno.IdEstadoProfesionalTurno = (int)EstadoProfesionalTurno.Disponible;
                        //    await _profesionalesTurnosRepository.Update(dbProfesionalTurno, tran);
                        //}

                        //Cancelo el turno
                        dbturno.IdProfesionalTurno = null;
                        dbturno.IdEstadoTurno = (int)EstadoTurno.Cancelado;
                        await _turnosRepository.Update(dbturno, tran);

                        await _turnosHistorialRepository.Insert(new TurnoHistorial()
                        {
                            IdTurno = (int)turno.IdTurno,
                            Fecha = DateTime.Now,
                            IdEstadoTurno = (int)EstadoTurno.Cancelado,
                            IdUsuario = _permissionsBusiness.Value.User.Id,
                            Observaciones = model.Motivo
                        }, tran);

                        await _chatApiBusiness.SendMessageTurnoCancelado(turno.Telefono, 
                                                                        turno.FechaInicio, 
                                                                        turno.Paciente, 
                                                                        turno.Profesional, 
                                                                        turno.Especialidad, 
                                                                        model.Motivo, 
                                                                        turno.IdPaciente
                                                , tran);
                    }
                }
                else
                {
                    throw new BusinessException("No existen turnos para cancelar para la fecha y profesional seleccionados.");
                }

                tran.Commit();
            }
            catch (BusinessException bEx)
            {
                _logger.LogError(bEx, null);
                tran.Rollback();
                throw bEx;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task DemandaEspontanea(DemandaEspontaneaViewModel model, bool noActualizarPaciente = false)
        {
            var especialidad = (await _lookupsBusiness.Value.GetAllEspecialidades()).FirstOrDefault(f => f.IdEspecialidad == model.IdEspecialidad);
            if (especialidad == null)
            {
                throw new BusinessException("No se encontró la especialidad.");
            }

            var profesional = (await _lookupsBusiness.Value.GetAllProfesionales(_permissionsBusiness.Value.User.IdEmpresa)).FirstOrDefault(f => f.IdProfesional == model.IdProfesional);
            if (profesional == null)
            {
                throw new BusinessException("No se encontró el profesional.");
            }

            if (especialidad.IdEspecialidad != profesional.IdEspecialidad)
            {
                throw new BusinessException("La especialidad seleccionada no es la misma del profesional.");
            }

            Paciente paciente = null;
            if (model.IdPaciente > 0)
            {
                paciente = await _pacientesRepository.GetById<Paciente>(model.IdPaciente);
                if (paciente == null)
                {
                    throw new BusinessException("No se encontró el paciente.");
                }
            }

            var consultorio = await _consultoriosRepository.GetById<Consultorio>(model.IdConsultorio);
            if (consultorio == null)
            {
                throw new BusinessException("Consultorio inexistente");
            }

            //TODO: Validaciones de cliente...
            var tran = _uow.BeginTransaction();

            try
            {
                long idPaciente = 0;
                if (model.IdPaciente == 0)
                {
                    //Creo un paciente nuevo
                    paciente = new Paciente
                    {
                        Nombre = model.Nombre.ToUpperInvariant(),
                        Documento = model.Documento,
                        Email = model.Email,
                        IdTipoTelefono = (int)TipoTelefono.Movil, //model.IdTipoTelefono,
                        Telefono = model.Telefono,
                        IdFinanciador = model.SinCobertura ? null : model.IdFinanciador,
                        IdFinanciadorPlan = model.SinCobertura ? null : model.IdFinanciadorPlan,
                        FinanciadorNro = model.SinCobertura ? null : model.FinanciadorNro,
                        FechaNacimiento = model.FechaNacimiento,
                        Nacionalidad = model.Nacionalidad,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo

                    };

                    idPaciente = await NewPacienteCliente(paciente, tran);
                }
                else
                {
                    idPaciente = model.IdPaciente;

                    if (!noActualizarPaciente)
                    {
                        paciente.Email = model.Email;
                        paciente.IdTipoTelefono = (int)TipoTelefono.Movil;
                        paciente.Telefono = model.Telefono;
                        paciente.IdFinanciador = model.SinCobertura ? null : model.IdFinanciador;
                        paciente.IdFinanciadorPlan = model.SinCobertura ? null : model.IdFinanciadorPlan;
                        paciente.FinanciadorNro = model.SinCobertura ? null : model.FinanciadorNro;
                        paciente.FechaNacimiento = model.FechaNacimiento;
                        paciente.Nacionalidad = model.Nacionalidad;

                        await UpdatePacienteCliente(paciente, tran);
                    }
                }

                if (model.ForzarPadron)
                {
                    if (paciente.IdFinanciador > 0)
                    {
                        await _financiadoresPadronRepository.Insert(new FinanciadorPadron
                        {
                            IdFinanciador = paciente.IdFinanciador.Value,
                            Documento = paciente.Documento,
                            FinanciadorNro = paciente.FinanciadorNro,
                            Fecha = DateTime.Now,
                            Nombre = paciente.Nombre
                        }, tran);
                    }
                }

                var now = DateTime.Now;
                var fechaInicio = now.AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
                var fechaFin = fechaInicio.AddMinutes(profesional.TurnosIntervalo > 0 ? profesional.TurnosIntervalo : especialidad.TurnosIntervalo);

                var idTurno = await _turnosRepository.Insert(new Turno
                {
                    IdTipoTurno = (int)TipoTurno.DemandaEspontanea,
                    IdEspecialidad = model.IdEspecialidad,
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    IdProfesional = model.IdProfesional,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    IdProfesionalTurno = null,
                    IdEstadoTurno = (int)EstadoTurno.Recepcionado,
                    IdPaciente = (int)idPaciente,
                    FechaConfirmacion = fechaInicio,
                    FechaRecepcion = fechaInicio,
                    IdConsultorio = model.IdConsultorio,
                    Consultorio = consultorio.Descripcion,
                    Observaciones = model.Observaciones,

                    ForzarParticular = model.ForzarParticular
                }, tran);

                await _turnosHistorialRepository.Insert(new TurnoHistorial()
                {
                    IdTurno = (int)idTurno,
                    Fecha = DateTime.Now,
                    IdEstadoTurno = (int)EstadoTurno.Recepcionado,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    Observaciones = string.Empty
                }, tran);

                tran.Commit();

                await _chatApiBusiness.SendMessageDemandaEspontanea(paciente.Telefono, 
                                                                    fechaInicio, 
                                                                    paciente.Nombre, 
                                                                    profesional.Nombre, 
                                                                    especialidad.Descripcion, 
                                                                    paciente.IdPaciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Guardia(GuardiaViewModel model, bool noActualizarPaciente = false)
        {
            Paciente paciente = null;
            if (model.IdPaciente > 0)
            {
                paciente = await _pacientesRepository.GetById<Paciente>(model.IdPaciente);
                if (paciente == null)
                {
                    throw new BusinessException("No se encontró el paciente.");
                }
            }

            //TODO: Validaciones de cliente...
            var tran = _uow.BeginTransaction();

            try
            {
                long idPaciente = 0;
                if (model.IdPaciente == 0)
                {
                    //Creo un paciente nuevo
                    paciente = new Paciente
                    {
                        Nombre = model.Nombre.ToUpperInvariant(),
                        Documento = model.Documento,
                        Email = model.Email,
                        IdTipoTelefono = (int)TipoTelefono.Movil, //model.IdTipoTelefono,
                        Telefono = model.Telefono,
                        IdFinanciador = model.SinCobertura ? null : model.IdFinanciador,
                        IdFinanciadorPlan = model.SinCobertura ? null : model.IdFinanciadorPlan,
                        FinanciadorNro = model.SinCobertura ? null : model.FinanciadorNro,
                        FechaNacimiento = model.FechaNacimiento,
                        Nacionalidad = model.Nacionalidad,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo

                    };

                    idPaciente = await NewPacienteCliente(paciente, tran);
                }
                else
                {
                    idPaciente = model.IdPaciente;

                    if (!noActualizarPaciente)
                    {
                        paciente.Email = model.Email;
                        paciente.IdTipoTelefono = (int)TipoTelefono.Movil;
                        paciente.Telefono = model.Telefono;
                        paciente.IdFinanciador = model.SinCobertura ? null : model.IdFinanciador;
                        paciente.IdFinanciadorPlan = model.SinCobertura ? null : model.IdFinanciadorPlan;
                        paciente.FinanciadorNro = model.SinCobertura ? null : model.FinanciadorNro;
                        paciente.FechaNacimiento = model.FechaNacimiento;
                        paciente.Nacionalidad = model.Nacionalidad;

                        await UpdatePacienteCliente(paciente, tran);
                    }
                }

                if (model.ForzarPadron)
                {
                    if (paciente.IdFinanciador > 0)
                    {
                        await _financiadoresPadronRepository.Insert(new FinanciadorPadron
                        {
                            IdFinanciador = paciente.IdFinanciador.Value,
                            Documento = paciente.Documento,
                            FinanciadorNro = paciente.FinanciadorNro,
                            Fecha = DateTime.Now,
                            Nombre = paciente.Nombre
                        }, tran);
                    }
                }

                var now = DateTime.Now;
                var fechaInicio = now.AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
                //var fechaFin = fechaInicio.AddMinutes(profesional.TurnosIntervalo > 0 ? profesional.TurnosIntervalo : especialidad.TurnosIntervalo);
                var fechaFin = fechaInicio.AddMinutes(5);

                var idTurno = await _turnosRepository.Insert(new Turno
                {
                    IdTipoTurno = (int)TipoTurno.Guardia,
                    IdEspecialidad = null,
                    IdProfesional = null,
                    IdConsultorio = null,
                    Consultorio = string.Empty,
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    IdProfesionalTurno = null,
                    IdEstadoTurno = (int)EstadoTurno.Recepcionado,
                    IdPaciente = (int)idPaciente,
                    FechaConfirmacion = fechaInicio,
                    FechaRecepcion = fechaInicio,
                    Observaciones = model.Observaciones,

                    ForzarParticular = model.ForzarParticular
                }, tran);

                await _turnosHistorialRepository.Insert(new TurnoHistorial()
                {
                    IdTurno = (int)idTurno,
                    Fecha = DateTime.Now,
                    IdEstadoTurno = (int)EstadoTurno.Recepcionado,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    Observaciones = string.Empty
                }, tran);

                tran.Commit();

                await _chatApiBusiness.SendMessageGuardia(paciente.Telefono,
                                                                    fechaInicio,
                                                                    paciente.Nombre,
                                                                    paciente.IdPaciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task<DataTablesResponse<Custom.TurnoHistorial>> GetHistorial(int idTurno, DataTablesRequest request)
        {
            var customQuery = _turnosHistorialRepository.GetHistorial(idTurno);
            var builder = _dbContext.Connection.QueryBuilder();

            return await _dataTablesService.Resolve<Custom.TurnoHistorial>(request, customQuery.Sql, customQuery.Parameters, staticWhere: builder.Sql);
        }

        private async Task<int> NewPacienteCliente(Paciente paciente, IDbTransaction transaction = null)
        {
            var idPaciente = await _pacientesRepository.Insert(paciente, transaction);

            var cliente = new Cliente
            {
                IdPaciente = (int)idPaciente,
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

            paciente.IdPaciente = (int)idPaciente;
            await NewUsuario(paciente, transaction);

            //_chatApiBusiness.SendMessageBienvenida(paciente.Telefono, paciente.Nombre);
            _mailBusiness.EnviarMailBienvenidaPaciente(paciente.Email, paciente.Nombre);

            return (int)idPaciente;
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

        private async Task UpdatePacienteCliente(Paciente paciente, IDbTransaction transaction = null)
        {
            await _pacientesRepository.Update(paciente, transaction);

            var cliente = await _clientesRepository.GetClienteByIdPaciente(paciente.IdPaciente, transaction);

            cliente.NombreFantasia = paciente.Nombre;
            cliente.Email = paciente.Email;
            cliente.IdTipoTelefono = (int)TipoTelefono.Movil;
            cliente.Telefono = paciente.Telefono;
            cliente.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            await _clientesRepository.Update(cliente, transaction);

        }
        public async Task<List<Custom.Turno>> GetTurnosPaciente()
            => await _turnosRepository.GetTurnosByIdPaciente(_permissionsBusiness.Value.User.IdPaciente);

        public Task<List<Custom.Select2Custom>> GetMedicamentos(string q)
            => _turnosRepository.GetMedicamentos(q);

        public async Task<Custom.TurnosTotales> ObtenerTotalesDashboard()
            => await _turnosRepository.GetTotalesDashboard(_permissionsBusiness.Value.User.IdEmpresa, _permissionsBusiness.Value.User.IdProfesional);


        public async Task<DataTablesResponse<Custom.Turno>> GetGuardia(DataTablesRequest request)
        {
            var customQuery = _turnosRepository.GetGuardia(null);
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("FechaDesde") && filters["FechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND FechaInicio >= {DateTime.ParseExact(filters["FechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FechaHasta") && filters["FechaHasta"].ToString() != "__/__/____")
            {
                var date = DateTime.ParseExact(filters["FechaHasta"].ToString(), "dd/MM/yyyy", null);
                builder.Append($"AND FechaInicio <= {date.AddDays(1)}");
            }

            builder.Append($"AND IdEspecialidad IS NULL");

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            builder.Append($"AND IdEstadoTurno = {(int)EstadoTurno.Recepcionado}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Turno>(request, customQuery.Sql, customQuery.Parameters, false, staticWhere: builder.Sql);
        }

        public async Task<List<Custom.Turno>> GetReporteTurnos(DataTablesRequest request)
        {
            var filters = request.ParseFilters();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            return await _turnosRepository.GetReporteTurnos(filters);
        }

        public async Task<List<Custom.TurnosEstadisticasEstados>> GetEstadisticasEstados(TurnosEstadisticasViewModel filters)
        {
            return await _turnosRepository.GetEstadisticasEstados(_permissionsBusiness.Value.User.IdEmpresa,
                                                                    DateTime.ParseExact(filters.FechaDesde, "dd/MM/yyyy", null),
                                                                    DateTime.ParseExact(filters.FechaHasta, "dd/MM/yyyy", null),
                                                                    filters.IdProfesional, filters.IdEspecialidad);
        }

        public async Task<Custom.TurnosEstadisticasAusentes> GetEstadisticasAusentes(TurnosEstadisticasViewModel filters)
        {
            return await _turnosRepository.GetEstadisticasAusentes(_permissionsBusiness.Value.User.IdEmpresa,
                                                                    DateTime.ParseExact(filters.FechaDesde, "dd/MM/yyyy", null),
                                                                    DateTime.ParseExact(filters.FechaHasta, "dd/MM/yyyy", null),
                                                                    filters.IdProfesional, filters.IdEspecialidad);
        }
    }
}
