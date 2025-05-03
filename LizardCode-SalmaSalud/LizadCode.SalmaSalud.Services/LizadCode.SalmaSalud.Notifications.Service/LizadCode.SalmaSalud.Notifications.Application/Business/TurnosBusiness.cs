using Dawa.Framework.Application.Common.Exceptions;
using Dawa.Framework.Application.Interfaces.Context;
using Microsoft.Extensions.Logging;
using LizadCode.SalmaSalud.Notifications.Application.Interfaces.Business;
using LizadCode.SalmaSalud.Notifications.Application.Interfaces.Repositories;
using LizadCode.SalmaSalud.Notifications.Domain.Entities;
using LizadCode.SalmaSalud.Notifications.Domain.Enums;

namespace LizadCode.SalmaSalud.Notifications.Application.Business
{
    internal class TurnosBusiness : ITurnosBusiness   
    {
        private readonly ILogger<TurnosBusiness> _logger;
        private readonly ITurnosRepository _turnosRepository;
        private readonly IUnitOfWork _uow;

        //private readonly IChatApiHelper _chatApiHelper;
        private readonly IChatApiBusiness _chatApiBusiness;

        public TurnosBusiness(
            ITurnosRepository turnosRepository,
            IUnitOfWork uow,
            ILogger<TurnosBusiness> logger,
            //IChatApiHelper chatApiHelper
            IChatApiBusiness chatApiBusiness)
        {
            _turnosRepository = turnosRepository;
            _uow = uow;
            _logger = logger;

            //_chatApiHelper = chatApiHelper;
            _chatApiBusiness = chatApiBusiness;
        }

        public async Task<List<Turno>> GetTurnosAConfirmar()
        {
            var turnos = await _turnosRepository.GetTurnosAConfirmar();

            return turnos.ToList();
        }

        public async Task EnviarRecordatorio(Turno turno)
        {
            //var tran = _uow.BeginTransaction();

            var turnoCustom = await _turnosRepository.GetByIdCustom(turno.IdTurno);
            try
            {
                //Implementar mecanismo de log de mensajes...
                var helperMessage = string.Format("Estimado *{0}*. Recuerde que tiene agendado un turno para el *{1}* a las *{2}* con el profesional *{3}* para la especialidad *{4}*. Observaciones: *{5}*",
                                                turnoCustom.Paciente, 
                                                turnoCustom.FechaInicio.ToString("dd/MM/yyyy"),
                                                turnoCustom.FechaInicio.ToString("HH:mm"),
                                                turnoCustom.Profesional,
                                                turnoCustom.Especialidad,
                                                turnoCustom.Observaciones);

                //_chatApiHelper.SendMessage("549" + turnoCustom.Telefono, helperMessage);
                await _chatApiBusiness.SendMessage(EventosChatApi.TurnoRecordatorio,
                                    "549" + turnoCustom.Telefono,
                                    helperMessage,
                                    turnoCustom.IdEmpresa,
                                    turnoCustom.IdPaciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                //tran.Rollback();
                throw new InternalException();
            }
        }
    }
}
