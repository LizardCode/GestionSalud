using System;

namespace LizardCode.Framework.RestClient.RestException
{
    public class CorruptedFileException : Exception
    {
        public CorruptedFileException(string message)
          : base(message)
        {
        }

        public CorruptedFileException(string message, Exception inner)
          : base(message, inner)
        {
        }
    }
}
