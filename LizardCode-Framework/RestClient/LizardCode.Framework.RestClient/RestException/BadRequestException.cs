using System;

namespace LizardCode.Framework.RestClient.RestException
{
    public class BadRequestException : Exception
    {
        public int? Code { get; set; }

        public BadRequestException(int code, string message)
            : base(message)
        {
            Code = code;
        }

        public BadRequestException(string message)
          : base(message)
        {
        }

        public BadRequestException(string message, Exception inner)
          : base(message, inner)
        {
        }
    }

}
