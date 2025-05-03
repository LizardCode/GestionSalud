using Dapper.DataTables.Extensions;
using LizardCode.Framework.Infrastructure.Context;
using LizardCode.Framework.Infrastructure.Interfaces.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Application.Interfaces.Repositories;
using Template.Application.Interfaces.Services;
using Template.Infrastructure.Repositories;
using Template.Infrastructure.Services;

namespace Template.Infrastructure
{
    public static class Injection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbContext, DbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();

            services.AddDapperDataTables<IDbContext>();
            services.AddTransient<IUsuariosRepository, UsuariosRepository>();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IFileService, FileService>();

            return services;
        }
    }
}
