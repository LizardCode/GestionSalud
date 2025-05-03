using LizardCode.Framework.Application.Interfaces.Exceptions;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class RequiredCaptchaException : BusinessBaseException, IBaseException
    {
        private const int _code = 12;

        public RequiredCaptchaException() : base(_code, "Ingrese el Captcha.")
        {
            //
        }

        public RequiredCaptchaException(string message) : base(_code, message)
        {
        }
    }
}
