using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LizardCode.SalmaSalud.Appointments.Application.Business;
using LizardCode.SalmaSalud.Appointments.Application.Interfaces.Business;
using System.Reflection;

namespace LizardCode.SalmaSalud.Appointments.Application
{
    public static class Injection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(GetConfiguredMappingConfig());
            services.AddScoped<IMapper, ServiceMapper>();

            services.AddScoped<ITurnosBusiness, TurnosBusiness>();
            services.AddScoped<IPresupuestosBusiness, PresupuestosBusiness>();

            return services;
        }

        private static TypeAdapterConfig GetConfiguredMappingConfig()
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());

            //config.Apply(registers);

            return config;
        }
    }
}
