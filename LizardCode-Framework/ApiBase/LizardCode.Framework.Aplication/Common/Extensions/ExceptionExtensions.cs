using LizardCode.Framework.Application.Interfaces.Exceptions;

namespace LizardCode.Framework.Application.Common.Extensions
{
    public static class ExceptionExtensions
    {
        public static object ToJson(this IBaseException exception)
        {
            return new
            {
                code = exception.Code,
                message = exception.Message
            };
        }
    }
}
