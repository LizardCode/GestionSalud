using LizardCode.Framework.Application.Interfaces.Exceptions;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class BusinessException : BusinessBaseException, IBaseException
    {
        private const int _code = 400;


        public BusinessException(string message) : base(_code, message)
        {
        }
    }
}
