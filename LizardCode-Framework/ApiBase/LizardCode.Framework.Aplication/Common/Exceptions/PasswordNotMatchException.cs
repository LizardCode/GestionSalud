using LizardCode.Framework.Application.Interfaces.Exceptions;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class PasswordNotMatchException : Exception, IBaseException
    {
        public int Code => 5;

        public PasswordNotMatchException() : base("Las contraseñas no coinciden")
        {
        }
    }
}
