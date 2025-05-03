using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Filters;

namespace LizardCode.SalmaSalud.Application.Startup
{
    public static class FilterConfig
    {
        public static void RegisterFilters(this MvcOptions options)
        {
            options.Filters.Add<ExceptionsLogging>();
        }
    }
}