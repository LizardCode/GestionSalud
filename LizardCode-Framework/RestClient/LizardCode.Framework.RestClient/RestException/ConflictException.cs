using System;
namespace LizardCode.Framework.RestClient.RestException
{
    public class ConflictException : Exception
    {
        public new string Message { get; set; }

        public ConflictException()
        {
            this.Message = "No se pudo crear el objeto enviado.";
        }

        public ConflictException(string message)
          : base(message)
        {
            this.Message = message;
        }

        public ConflictException(string message, Exception inner)
          : base(message, inner)
        {
            this.Message = message;
        }
    }

}
