using LizardCode.Framework.Application.Interfaces.Exceptions;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class EmpresaNotDefinedException : Exception, IBaseException
    {
        public int Code => 11;

        public EmpresaNotDefinedException() : base("No se ha definido a EMPRESA para el usuario Administrador.")
        {
            //
        }
    }
}
