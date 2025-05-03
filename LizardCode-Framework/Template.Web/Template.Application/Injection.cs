using LizardCode.Framework.Aplication.Common.Extensions;
using LizardCode.Framework.Aplication.Interfaces.Business;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Template.Application.Business;
using Template.Application.Interfaces.Business;

namespace Template.Application
{
    public static class Injection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLazyResolution();
            services.AddSingleton(GetConfiguredMappingConfig());
            services.AddScoped<IMapper, ServiceMapper>();
            services.AddScoped<IPermisosBusiness, PermisosBusiness>();
            services.AddScoped<IUsuariosBusiness, UsuariosBusiness>();
            services.AddScoped<ILookupsBusiness, LookupsBusiness>();

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
