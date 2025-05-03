using LizardCode.Framework.Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LizardCode.Framework.Application.Common.Extensions
{
    public static class ControllerExtensions
    {
        public static T GetService<T>(this Controller controller)
        {
            return HttpContextHelper.Current.RequestServices.GetService<T>();
        }
    }
}
