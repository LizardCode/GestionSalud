using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Application.Interfaces.Context;
using Microsoft.Extensions.Logging;
using LizardCode.SalmaSalud.Appointments.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Appointments.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Appointments.Domain.Entities;
using LizardCode.SalmaSalud.Appointments.Domain.Enums;

namespace LizardCode.SalmaSalud.Appointments.Application.Business
{
    internal class TurnosBusiness : ITurnosBusiness   
    {
        private readonly ILogger<TurnosBusiness> _logger;
        private readonly ITurnosRepository _turnosRepository;
        private readonly ITurnosHistorialRepository _turnosHistorialRepository;
        private readonly IUnitOfWork _uow;

        //private readonly IChatApiHelper _chatApiHelper;

        public TurnosBusiness(
            ITurnosRepository turnosRepository,
            ITurnosHistorialRepository turnosHistorialRepository,
            IUnitOfWork uow,
            ILogger<TurnosBusiness> logger)
            //IChatApiHelper chatApiHelper)
        {
            _turnosRepository = turnosRepository;
            _turnosHistorialRepository = turnosHistorialRepository;
            _uow = uow;
            _logger = logger;

            //_chatApiHelper = chatApiHelper;
        }

        public async Task<List<Turno>> GetTurnosAusentes()
        {
            var turnosAusentes = await _turnosRepository.GetTurnosAusentes();

            return turnosAusentes.ToList();
        }

        public async Task MarcarAusenteSinAviso(Turno turno)
        {
            var tran = _uow.BeginTransaction();

            try
            {
                turno.IdEstadoTurno = (int)EstadoTurno.AusenteSinAviso;
                await _turnosRepository.Update(turno, tran);

                await _turnosHistorialRepository.Insert(new TurnoHistorial()
                {
                    IdTurno = (int)turno.IdTurno,
                    Fecha = DateTime.Now,
                    IdEstadoTurno = (int)EstadoTurno.AusenteConAviso,
                    IdUsuario = 1, //admin - sistema
                    Observaciones = "MARCADO COMO AUSENTE POR PROCESO"
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
    }
}
