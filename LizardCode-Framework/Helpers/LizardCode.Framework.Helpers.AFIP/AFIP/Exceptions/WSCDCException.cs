using System;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class WSCDCException : Exception
    {
        public int Code => 1;

        public WSCDCException(string message) : base(message ?? "Error en WebService CDC de Consulta de Comprobantes")
        {
            //
        }
    }
}
