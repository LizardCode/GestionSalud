using LizardCode.Framework.Application.Common.Enums;
using LizardCode.Framework.Application.Interfaces.Exceptions;
using LizardCode.Framework.Helpers.Utilities;

namespace LizardCode.Framework.Application.Common.Exceptions
{
    public class PermissionException : Exception, IBaseException
    {
        public int Code { get; private set; }
        public string Detail { get; private set; }


        public PermissionException(int code, string detail) : base(detail)
        {
            Code = code;
            Detail = detail;
        }

        public PermissionException(PermisoError Code) : base(Code.Description())
        {
            this.Code = (int)Code;
            Detail = Code.Description();
        }

        public PermissionException(PermisoError code, string internalDetail) : base($"{code.Description()} - {internalDetail}")
        {
            Code = (int)code;
            Detail = $"{code.Description()} - {internalDetail}";
        }
    }
}
