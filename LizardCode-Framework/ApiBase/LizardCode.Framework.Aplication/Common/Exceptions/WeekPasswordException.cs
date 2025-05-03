using LizardCode.Framework.Application.Interfaces.Exceptions;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class WeekPasswordException : Exception, IBaseException
    {
        public int Code => 4;

        public WeekPasswordException() : base("La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula y dos números")
        {
            //
        }
    }
}
