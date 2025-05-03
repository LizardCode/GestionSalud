using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.Framework.Infrastructure.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LizardCode.SalmaSalud.Appointments.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Appointments.Infrastructure.Repositories;

namespace LizardCode.SalmaSalud.Appointments.Infrastructure
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
            services.AddTransient<IPresupuestosRepository, PresupuestosRepository>();

            return services;
        }
    }
}
