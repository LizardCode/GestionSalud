using LizardCode.Framework.Application.Interfaces.Exceptions;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class EmpresaNotFoundException : Exception, IBaseException
    {
        public int Code => 0;

        public EmpresaNotFoundException() : base("La Empresa No Existe en el Maestro de Empresas.")
        {
            //
        }
    }
}
