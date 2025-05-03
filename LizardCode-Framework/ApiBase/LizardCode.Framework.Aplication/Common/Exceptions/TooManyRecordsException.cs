using LizardCode.Framework.Application.Interfaces.Exceptions;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class TooManyRecordsException : Exception, IBaseException
    {
        public int Code => 7;

        public TooManyRecordsException() : base("Demasiados registros")
        {
            //
        }
    }
}
