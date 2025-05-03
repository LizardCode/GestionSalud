using Dapper.DataTables.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Linq;

namespace Dapper.DataTables.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDapperDataTables<TContext>(this IServiceCollection services)
        {
            services.AddTransient<IDataTablesService>(provider =>
            {
                var context = provider.GetRequiredService<TContext>();

                if (context == null)
                    throw new ArgumentNullException("DbContext inexistente");

                var pi = context.GetType().GetProperties()
                    .SingleOrDefault(property => property.PropertyType.IsAssignableFrom(typeof(IDbConnection)));

                if (pi == null)
                    throw new ArgumentNullException("IDbConnection inexistente");

                return new DataTablesService((IDbConnection)pi.GetValue(context));
            });
        }
    }
}
