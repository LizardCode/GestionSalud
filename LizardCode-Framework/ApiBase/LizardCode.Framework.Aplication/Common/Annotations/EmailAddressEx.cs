using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LizardCode.Framework.Application.Common.Annotations
{
    public class EmailAddressExAttribute : RegularExpressionAttribute, IClientModelValidator
    {
        private const string RegexPattern = @"^(([^<>()\[\]\\.,;:\s@""]+(\.[^<>()\[\]\\.,;:\s@""]+)*)|("".+""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";
        private const string Message = "Formato incorrecto";


        public EmailAddressExAttribute() : base(RegexPattern)
        {
            ErrorMessage = Message;
        }


        public void AddValidation(ClientModelValidationContext context)
        {
            if (!context.Attributes.ContainsKey("data-val"))
                context.Attributes.Add("data-val", "true");

            if (!context.Attributes.ContainsKey("data-val-regex"))
                context.Attributes.Add("data-val-regex", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));

            if (!context.Attributes.ContainsKey("data-val-regex-pattern"))
                context.Attributes.Add("data-val-regex-pattern", RegexPattern);
        }

        public override bool IsValid(object value)
        {
            return Regex.IsMatch(value?.ToString() ?? string.Empty, RegexPattern);
        }
    }
}
