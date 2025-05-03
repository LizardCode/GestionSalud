using LizardCode.Framework.Application.Interfaces.Exceptions;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class PasswordExpiredException : Exception, IBaseException
    {
        public int Code => 2;

        public PasswordExpiredException() : base("Contraseña vencida")
        {
            //
        }
    }
}
