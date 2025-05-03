using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Application.Models.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System.Net;

namespace LizardCode.Framework.Application.Common.Filters
{
    public class ExceptionsLogging : IExceptionFilter
    {
        private readonly ILogger<ExceptionsLogging> _logger;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;


        public ExceptionsLogging(ILogger<ExceptionsLogging> logger, ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _logger = logger;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
        }


        public void OnException(ExceptionContext filterContext)
        {
            _logger.LogError(filterContext.Exception, "Trapeo global de excepción");

            if (filterContext.Exception is BusinessException)
            {
                var businessException = (BusinessException)filterContext.Exception;
                var response = new
                {
                    Status = "Error",
                    Detail = businessException.Message
                };

                filterContext.Result = new JsonResult(response)
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            else if (filterContext.Exception != null)
            {
                if (filterContext.HttpContext.Request.Headers.ContainsKey("X-Requested-With") &&
                    filterContext.HttpContext.Request.Headers["X-Requested-With"].ToString().Equals("XMLHttpRequest", StringComparison.InvariantCultureIgnoreCase)
                )
                {
                    var response = new
                    {
                        Status = "Error",
                        Detail = filterContext.Exception.ToString()
                    };

                    filterContext.Result = new JsonResult(response)
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }
                else
                {
                    var exception = filterContext.Exception;
                    var exceptionMessage = $"{exception.Message}{(exception.InnerException != null ? exception.InnerException.Message : "")}";
                    var tempData = _tempDataDictionaryFactory.GetTempData(filterContext.HttpContext);

                    tempData.Add<ErrorsViewModel>("HandledException", new ErrorsViewModel
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Title = "Error",
                        Message = "No fue posible realizar correctamente la acción solicitada, por favor intente en unos minutos o contacte al administrador del sistema.",
                        Exception = exceptionMessage,
                        UrlRedirect = "Index"
                    });

                    filterContext.Result = new RedirectResult($"/Errors/500");
                }
            }
        }
    }
}