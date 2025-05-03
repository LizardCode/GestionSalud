using LizardCode.Framework.Helpers.DynamicCors.Accessors;
using LizardCode.Framework.Helpers.DynamicCors.Enums;
using LizardCode.Framework.Helpers.DynamicCors.Resolvers;
using LizardCode.Framework.Helpers.DynamicCors.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LizardCode.Framework.Helpers.DynamicCors.Middlewares
{

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class DynamicCorsPolicyMiddlewareExtensions
    {
        public static IApplicationBuilder UseDynamicCorsMiddleware(
            this IApplicationBuilder builder, 
            string policyName = default)
        {
            if (string.IsNullOrWhiteSpace(policyName))
                policyName = CorsPoliciesEnums.DynamicCorsPolicyName;

            return builder.UseMiddleware<DynamicCorsPolicyMiddleware>(policyName);
        }

        public static IServiceCollection AddDynamicCors<TDynamicCorsPolicyResolver>(
            this IServiceCollection services,
            Action<CorsOptions> setupAction)
            where TDynamicCorsPolicyResolver : class, IDynamicCorsPolicyResolver
        {
            services.AddCors(setupAction);

            services.TryAdd(ServiceDescriptor.Transient<IDynamicCorsPolicyService, DynamicCorsPolicyService>());
            services.TryAdd(ServiceDescriptor.Transient<ICorsPolicyAccessor, CorsPolicyAccessor>());
            services.TryAdd(ServiceDescriptor.Transient<IDynamicCorsPolicyResolver, TDynamicCorsPolicyResolver>());

            return services;
        }
    }
}