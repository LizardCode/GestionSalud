namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class InternalException : BusinessBaseException
    {
        private const int _code = 500;
        private const string _message = "La operación no pudo realizarse debido a un error interno en la aplicación";


        public InternalException() : base(_code, _message)
        {
        }

        public InternalException(string message) : base(_code, message)
        {
        }
    }
}
