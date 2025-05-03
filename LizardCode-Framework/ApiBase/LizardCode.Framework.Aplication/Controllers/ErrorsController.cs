using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Application.Controllers.Base;
using LizardCode.Framework.Application.Models.Errors;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LizardCode.Framework.Application.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : BusinessController
    {

        [Route("Errors/{errorCode}")]
        [HttpGet]
        public ActionResult Index(int errorCode)
        {
            var model = TempData.Get<ErrorsViewModel>("HandledException");

            if (model == null)
                model = new ErrorsViewModel
                {
                    Code = errorCode,
                    Title = "Error",
                    Message = "No fue posible realizar correctamente la acción solicitada, por favor intente en unos minutos o contacte al administrador del sistema.",
                    UrlRedirect = "Index"
                };
            else
                TempData.Keep("HandledException");

            return View(model);
        }

        [Route("Errors/Handle/{errorCode}/{encodedException?}")]
        [HttpGet]
        public ActionResult Handle(int errorCode, string encodedException)
        {
            var request = HttpContext.Request;

            switch (errorCode)
            {
                case (int)HttpStatusCode.NotFound:
                    TempData.Add<ErrorsViewModel>("HandledException", new ErrorsViewModel
                    {
                        Code = (int)HttpStatusCode.NotFound,
                        Title = "Página no encontrada",
                        Message = "No se encontro la pagina requerida!",
                        UrlRedirect = "Index"
                    });
                    return Redirect($"~/Errors/{errorCode}");

                case (int)HttpStatusCode.Unauthorized:
                    if (request.Headers.ContainsKey("X-Requested-With") && request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Unauthorized();
                    break;

                case (int)HttpStatusCode.InternalServerError:
                    var exception = WebUtility.UrlDecode(encodedException?.Replace("Ý", "+") ?? "");
                    TempData.Add<ErrorsViewModel>("HandledException", new ErrorsViewModel
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Title = "Error",
                        Message = "No fue posible realizar correctamente la acción solicitada, por favor intente en unos minutos o contacte al administrador del sistema.",
                        Exception = exception,
                        UrlRedirect = "Index"
                    });
                    return Redirect($"~/Errors/{errorCode}");
            }

            return Redirect($"~/Errors/{errorCode}");
        }
    }
}

