using LizardCode.Framework.Application.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UtilitiesExtensions = LizardCode.Framework.Helpers.Utilities.Extensions;

namespace LizardCode.SalmaSalud.Application.Common.Extensions
{
    public static class StartupExtensions
    {
        public static void UseSalmaSaludUtilities(this IApplicationBuilder app)
        {
            var httpContextAccessor = new HttpContextAccessor();
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var memoryCache = app.ApplicationServices.GetRequiredService<IMemoryCache>();

            HttpContextHelper.Configure(httpContextAccessor);
            UtilitiesExtensions.Configure(configuration, httpContextAccessor, memoryCache);
        }
    }
}
