using LizardCode.Framework.Application.Interfaces.Exceptions;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class UserNotFoundException : Exception, IBaseException
    {
        public int Code => 3;

        public UserNotFoundException() : base("Usuario inexistente")
        {
            //
        }
    }
}
