using System.ComponentModel.DataAnnotations;

namespace LizardCode.Framework.Application.Common.Annotations
{
    public class RequiredExAttribute : RequiredAttribute
    {
        private const string Message = "Obligatorio";

        public RequiredExAttribute()
        {
            base.ErrorMessage = Message;
        }
    }
}
