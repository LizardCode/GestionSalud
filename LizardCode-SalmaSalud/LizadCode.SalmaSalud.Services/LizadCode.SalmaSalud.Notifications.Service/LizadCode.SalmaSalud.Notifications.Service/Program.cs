using Dawa.Framework.Helpers.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace LizadCode.SalmaSalud.Notifications
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var logger = NLog.LogManager.GetCurrentClassLogger();

            try
            {
                logger.Info("DTC-Notifications-Service...");
                await CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Servicio detenido por excepción");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(System.IO.Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .Build();


            Extensions.Configure(config, null, null);

            var host = Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices(services =>
                {
                    services.ConfigureServices(config);
                    services.AddHostedService<Training>();
                })
                .ConfigureLogging(logginBuilder =>
                {
                    logginBuilder.ClearProviders();
                    logginBuilder.AddNLog(config);
                });

            return host;
        }
    }
}
