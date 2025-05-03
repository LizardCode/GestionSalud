using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using LizardCode.SalmaSalud.Appointments.Application.Interfaces.Business;
//using LizardCode.SalmaSalud.Appointments.Application.Interfaces.Business;

namespace LizardCode.SalmaSalud.Appointments
{
    public class Service : IService
    {
        private readonly ILogger<Service> _logger;
        private readonly ITurnosBusiness _turnosBusiness;
        private readonly IPresupuestosBusiness _presupuestosBusiness;

        public Service(ILogger<Service> logger, 
                        ITurnosBusiness turnosBusiness,
                        IPresupuestosBusiness presupuestosBusiness)
        {
            _logger = logger;
            _turnosBusiness = turnosBusiness;
            _presupuestosBusiness = presupuestosBusiness;
        }


        public async Task DoWork(CancellationToken cancellationToken)
        {
            var scheduledTime = "ScheduledTime".FromAppSettings<TimeSpan>(notFoundException: true);
            var exceptionRetries = "ExceptionRetries".FromAppSettings<int>(notFoundException: true);
            var interval = "Interval".FromAppSettings<int>(notFoundException: true);
            var exceptionCount = 1;
            var cycle = 0;
            var nextCycle = DateTime.Today.Add(scheduledTime);

            _logger.LogInformation($"Service.DoWork()");
            _logger.LogInformation($"ScheduledTime: [{scheduledTime}]");
            _logger.LogInformation($"ExceptionRetries: [{exceptionRetries}]");
            _logger.LogInformation($"Interval: [{interval}]");
            _logger.LogInformation($"First cycle [{nextCycle:dd/MM/yyyy HH:mm}]");

            while (!cancellationToken.IsCancellationRequested)
            {
                Interlocked.Increment(ref cycle);
                _logger.LogInformation($"Cycle #{cycle}");

                await Task.Delay(interval * 1000, cancellationToken);

                if (nextCycle > DateTime.Now)
                    continue;

                _logger.LogInformation($"Running cycle [{nextCycle:dd/MM/yyyy HH:mm}]");
                _logger.LogInformation($"Next cycle [{nextCycle = nextCycle.AddDays(1):dd/MM/yyyy HH:mm}]");

                try
                {
                    // Inyectar business y ejecutar sus métodos acá
                    var turnos = await _turnosBusiness.GetTurnosAusentes();

                    if (turnos != null && turnos.Count > 0)
                    {
                        _logger.LogInformation($"Se encontraron {turnos.Count} turnos para procesar.");

                        foreach (var turno in turnos)
                        {
                            _logger.LogInformation($"Procesando turno {turno.IdTurno}.");
                            await _turnosBusiness.MarcarAusenteSinAviso(turno);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"No se encontraron turnos para procesar.");
                    }

                    var presupuestos = await _presupuestosBusiness.GetPresupuestosAVencer();

                    if (presupuestos != null && presupuestos.Count > 0)
                    {
                        _logger.LogInformation($"Se encontraron {presupuestos.Count} presupuestos para VENCER.");

                        foreach (var presupuesto in presupuestos)
                        {
                            _logger.LogInformation($"Procesando presupuesto {presupuesto.IdPresupuesto}.");
                            await _presupuestosBusiness.MarcarVencido(presupuesto);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"No se encontraron presupuestos para VENCER.");
                    }

                    _logger.LogInformation($"Appointments...");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[{exceptionCount}] Error");

                    if (exceptionCount++ >= exceptionRetries)
                        break;
                }
            }

            _logger.LogInformation($"Process terminated");
        }
    }
}
