using LizardCode.Framework.Application.Interfaces.Exceptions;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class WrongPasswordException : Exception, IBaseException
    {
        public int Code => 8;

        public WrongPasswordException() : base("Contraseña actual incorrecta")
        {
            //
        }
    }
}
