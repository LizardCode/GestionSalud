﻿using System;

namespace LizardCode.Framework.RestClient.RestException
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
        {
        }

        public UnauthorizedException(string message)
          : base(message)
        {
        }

        public UnauthorizedException(string message, Exception inner)
          : base(message, inner)
        {
        }
    }

}
