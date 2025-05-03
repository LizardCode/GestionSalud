using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Application.Interfaces.Context;
using Microsoft.Extensions.Logging;
using LizardCode.SalmaSalud.Appointments.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Appointments.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Appointments.Domain.Entities;
using LizardCode.SalmaSalud.Appointments.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Appointments.Application.Business
{
    public class PresupuestosBusiness : IPresupuestosBusiness
    {
        private readonly ILogger<PresupuestosBusiness> _logger;
        private readonly IPresupuestosRepository _presupuestosRepository;
        private readonly IUnitOfWork _uow;

        public PresupuestosBusiness(
            IPresupuestosRepository presupuestosRepository,
            IUnitOfWork uow,
            ILogger<PresupuestosBusiness> logger)
        {
            _presupuestosRepository = presupuestosRepository;
            _uow = uow;
            _logger = logger;
        }

        public async Task<List<Presupuesto>> GetPresupuestosAVencer()
        {
            var presupuestosAVencer = await _presupuestosRepository.GetPresupuestosAVencer();

            return presupuestosAVencer.ToList();
        }

        public async Task MarcarVencido(Presupuesto presupuesto)
        {
            var tran = _uow.BeginTransaction();

            try
            {
                presupuesto.IdEstadoPresupuesto = (int)EstadoPresupuesto.Vencido;
                await _presupuestosRepository.Update(presupuesto, tran);

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
