using System;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class PadronException : Exception
    {
        public int Code => 1;

        public PadronException(string message) : base(message ?? "Error en WebService de PadronV4")
        {
            //
        }
    }
}
