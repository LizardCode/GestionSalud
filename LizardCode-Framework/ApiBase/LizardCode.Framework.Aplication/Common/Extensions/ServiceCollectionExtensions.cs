using LizardCode.Framework.Application.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace LizardCode.Framework.Application.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLazyResolution(this IServiceCollection services)
        {
            return services.AddTransient(typeof(Lazy<>), typeof(LazyResolver<>));
        }
    }
}
