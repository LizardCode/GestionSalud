using LizardCode.Framework.Application.Interfaces.Exceptions;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class LoginFailedException : Exception, IBaseException
    {
        public int Code => 1;

        public LoginFailedException() : base("Usuario y/o contraseña incorrectos")
        {
            //
        }
    }
}
