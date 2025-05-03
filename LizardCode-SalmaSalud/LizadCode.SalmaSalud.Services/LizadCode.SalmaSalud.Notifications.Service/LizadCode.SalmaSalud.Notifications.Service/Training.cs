using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LizadCode.SalmaSalud.Notifications
{
    public class Training : BackgroundService
    {
        private readonly ILogger<Training> _logger;
        private readonly IService _service;



        public Training(ILogger<Training> logger, IService service)
        {
            _logger = logger;
            _service = service;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Ejecutando...");
            await _service.DoWork(stoppingToken);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando...");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deteniendo...");
            return base.StopAsync(cancellationToken);
        }
    }
}