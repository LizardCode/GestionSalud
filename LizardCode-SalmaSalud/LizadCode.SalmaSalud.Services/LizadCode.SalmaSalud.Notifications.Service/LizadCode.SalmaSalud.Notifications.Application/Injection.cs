using Dawa.Framework.Helpers.ChatApi;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LizadCode.SalmaSalud.Notifications.Application.Business;
using LizadCode.SalmaSalud.Notifications.Application.Interfaces.Business;
using System.Reflection;

namespace LizadCode.SalmaSalud.Notifications.Application
{
    public static class Injection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(GetConfiguredMappingConfig());
            services.AddScoped<IMapper, ServiceMapper>();

            services.AddScoped<IChatApiHelper, ChatApiHelper>();
            services.AddScoped<ITurnosBusiness, TurnosBusiness>();
            services.AddScoped<IChatApiBusiness, ChatApiBusiness>();

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
