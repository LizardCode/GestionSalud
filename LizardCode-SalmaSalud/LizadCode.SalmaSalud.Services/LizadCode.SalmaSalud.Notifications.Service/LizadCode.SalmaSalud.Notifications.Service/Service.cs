using Dawa.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using LizadCode.SalmaSalud.Notifications.Application.Interfaces.Business;

namespace LizadCode.SalmaSalud.Notifications
{
    public class Service : IService
    {
        private readonly ILogger<Service> _logger;
        private readonly ITurnosBusiness _turnosBusiness;


        public Service(ILogger<Service> logger, 
                        ITurnosBusiness turnosBusiness)
        {
            _logger = logger;
            _turnosBusiness = turnosBusiness;
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
                    var turnos = await _turnosBusiness.GetTurnosAConfirmar();

                    if (turnos != null && turnos.Count > 0)
                    {
                        _logger.LogInformation($"Se encontraron {turnos.Count} turnos para enviar recordatorio de confirmación.");

                        foreach (var turno in turnos)
                        {
                            _logger.LogInformation($"Procesando turno {turno.IdTurno}.");
                            await _turnosBusiness.EnviarRecordatorio(turno);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"No se encontraron turnos para procesar.");
                    }

                    _logger.LogInformation($"Notifications...");
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
