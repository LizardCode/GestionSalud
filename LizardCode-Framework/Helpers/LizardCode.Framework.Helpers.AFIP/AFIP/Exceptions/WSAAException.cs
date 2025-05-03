using System;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class WSAAException : Exception
    {
        public int Code => 1;

        public WSAAException(string message) : base(message ?? "Error en Obtener el Token WSAA")
        {
            //
        }
    }
}
