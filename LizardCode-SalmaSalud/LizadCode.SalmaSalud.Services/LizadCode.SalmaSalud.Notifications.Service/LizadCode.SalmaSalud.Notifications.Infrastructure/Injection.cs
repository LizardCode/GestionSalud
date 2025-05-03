using Dawa.Framework.Application.Interfaces.Context;
using Dawa.Framework.Infrastructure.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LizadCode.SalmaSalud.Notifications.Application.Interfaces.Repositories;
using LizadCode.SalmaSalud.Notifications.Infrastructure.Repositories;

namespace LizadCode.SalmaSalud.Notifications.Infrastructure
{
    public static class Injection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbContext, DbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();

            services.AddTransient<ITurnosRepository, TurnosRepository>();
            services.AddTransient<ITurnosHistorialRepository, TurnosHistorialRepository>();
            services.AddTransient<IAuditoriasChatApiRepository, AuditoriasChatApiRepository>();


            return services;
        }
    }
}
