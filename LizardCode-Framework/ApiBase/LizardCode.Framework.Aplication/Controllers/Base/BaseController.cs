using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Application.Helpers;
using LizardCode.Framework.Application.Interfaces.Exceptions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LizardCode.Framework.Application.Controllers.Base
{
    public partial class BaseController : Controller
    {
        protected JsonResult Json(Func<object> method)
        {
            return Json("OK", method);
        }

        protected JsonResult Json(IBaseException ex)
        {
            return Json("Error", () => ex.ToJson());
        }

        protected JsonResult Json(int code, string message)
        {
            return Json("Error", () => new { code, message });
        }

        protected JsonResult Json(string status, Func<object> method)
        {
            try
            {
                var results = method.Invoke();

                return Json(new { status, detail = results });
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex.ToString());

                var message = ex.InnerException?.Message;

                if (message.IsNull())
                {
                    message = ex.Message;
                }

                if (message.IsNull())
                {
                    message = "Error desconocido";
                }

                return Json(new { status = "Error", detail = message });
            }
        }

        public FileContentResult FileContent(byte[] fileContents, string contentType)
        {
            return File(fileContents, contentType);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var forceDomain = "ForceBrandDomain".FromAppSettings<bool>(notFoundException: false);
            var defaultDomain = "DefaultBrandDomain".FromAppSettings<string>(notFoundException: false) ?? "dawasoft";
            var domain = GetDomain();

            if (forceDomain)
                ViewBag.Domain = defaultDomain;
            else
                ViewBag.Domain = domain.Contains("localhost") ? defaultDomain : domain;
        }

        private string GetDomain()
        {
            var host = HttpContextHelper.Current.Request.Host.Host;

            return host.Replace("www.", string.Empty)
                        .Replace(".com", string.Empty)
                        .Replace("dtc.", string.Empty)
                        .Replace("dhc.", string.Empty)
                        .Replace("doc.", string.Empty);
        }
    }
}
