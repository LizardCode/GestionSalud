using System;

namespace LizardCode.Framework.Helpers.Utilities.Mail
{
    public class Base64FormatException : Exception
    {
        public Base64FormatException()
        {
            //
        }

        public Base64FormatException(string message)
            : base(message)
        {
            //
        }
    }

    public class EmailFormatException : Exception
    {
        public EmailFormatException()
        {
            //
        }

        public EmailFormatException(string message)
            : base(message)
        {
            //
        }
    }
}
