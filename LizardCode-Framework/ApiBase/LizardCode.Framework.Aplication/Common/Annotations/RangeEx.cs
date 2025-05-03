using System.ComponentModel.DataAnnotations;

namespace LizardCode.Framework.Application.Common.Annotations
{
    public class RangeExAttribute : RangeAttribute
    {
        private const string Message = "Valor incorrecto";

        public RangeExAttribute(double minimum, double maximum) : base(minimum, maximum)
        {
            base.ErrorMessage = Message;
        }
    }
}
