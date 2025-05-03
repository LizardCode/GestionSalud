namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class BusinessBaseException : Exception
    {
        public int Code { get; }

        public BusinessBaseException(int code, string message) : base(message)
        {
            Code = code;
        }
    }
}
