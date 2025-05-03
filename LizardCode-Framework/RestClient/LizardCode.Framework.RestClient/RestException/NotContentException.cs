using System;

namespace LizardCode.Framework.RestClient.RestException
{
    public class NotContentException : Exception
    {
        public new string Message { get; set; }

        public NotContentException()
        {
            this.Message = "El body recibido se encuentra vacío.";
        }

        public NotContentException(string message)
          : base(message)
        {
            this.Message = message;
        }

        public NotContentException(string message, Exception inner)
          : base(message, inner)
        {
            this.Message = message;
        }
    }

}
