using System.ComponentModel.DataAnnotations;

namespace LizardCode.Framework.Application.Common.Annotations
{
    public class StringLengthExAttribute : StringLengthAttribute
    {
        private const string Message = "Excede los ({0}) caracteres";

        public StringLengthExAttribute(int maximumLength) : base(maximumLength)
        {
            base.ErrorMessage = string.Format(Message, maximumLength);
        }
    }
}
