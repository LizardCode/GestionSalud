using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LizadCode.SalmaSalud.Notifications.Application;
using LizadCode.SalmaSalud.Notifications.Infrastructure;

namespace LizadCode.SalmaSalud.Notifications
{
    public static class Injection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IService, Service>();

            services.AddApplication(configuration);
            services.AddInfrastructure(configuration);

            return services;
        }
    }
}
