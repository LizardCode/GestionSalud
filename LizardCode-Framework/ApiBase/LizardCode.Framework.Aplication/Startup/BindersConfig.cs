using LizardCode.Framework.Application.Binders;
using Microsoft.AspNetCore.Mvc;

namespace LizardCode.Framework.Application.Startup
{
    public static class BindersConfig
    {
        public static void RegisterBinders(this MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new DateTimeBinderProvider());
            options.ModelBinderProviders.Insert(0, new DecimalBinderProvider());
            options.ModelBinderProviders.Insert(0, new DoubleBinderProvider());
            options.ModelBinderProviders.Insert(0, new NullableDateTimeBinderProvider());
        }
    }
}
