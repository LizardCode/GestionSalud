using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;

namespace LizardCode.SalmaSalud
{
    public class Program
    {
        private static NLog.Logger _log;


        public static void Main(string[] args)
        {
            _log = NLog.LogManager.GetCurrentClassLogger();

            var logger = NLog.LogManager.GetCurrentClassLogger();

            try
            {
                _log.Info("Iniciando LizardCode.SalmaSalud...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Sistema detenido por excepción");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logginBuilder =>
                {
                    logginBuilder.ClearProviders();
                })
                .UseNLog();
    }
}
