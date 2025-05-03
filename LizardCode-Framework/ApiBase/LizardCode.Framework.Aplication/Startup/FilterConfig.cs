using LizardCode.Framework.Application.Common.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LizardCode.Framework.Application.Startup
{
    public static class FilterConfig
    {
        public static void RegisterFilters(this MvcOptions options)
        {
            options.Filters.Add<ExceptionsLogging>();
        }
    }
}