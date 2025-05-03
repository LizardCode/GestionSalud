using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LizardCode.SalmaSalud.Appointments.Application;
using LizardCode.SalmaSalud.Appointments.Infrastructure;

namespace LizardCode.SalmaSalud.Appointments
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
