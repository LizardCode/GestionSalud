using System;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class WSFEException : Exception
    {
        public int Code => 1;

        public WSFEException(string message) : base(message ?? "Error en WebService FE de Comprobantes Electrónicos")
        {
            //
        }
    }
}
