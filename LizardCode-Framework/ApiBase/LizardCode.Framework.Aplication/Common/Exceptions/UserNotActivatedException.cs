using LizardCode.Framework.Application.Interfaces.Exceptions;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class UserNotActivatedException : Exception, IBaseException
    {
        public int Code => 10;

        public UserNotActivatedException() : base("Usuario inactivo. Por favor revise su correo electrónico para la activación.")
        {
            //
        }
    }
}
