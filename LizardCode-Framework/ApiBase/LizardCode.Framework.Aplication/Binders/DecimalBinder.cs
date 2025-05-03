using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Globalization;

namespace LizardCode.Framework.Application.Binders
{
    public class DecimalBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(decimal))
            {
                return new BinderTypeModelBinder(typeof(DecimalBinder));
            }

            return null;
        }
    }

    public class DecimalBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var values = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (values == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, values);

            try
            {
                var actualValue =
                    values.FirstValue == null
                        ? default(decimal)
                        : Convert.ToDecimal(values.FirstValue, CultureInfo.InvariantCulture);

                bindingContext.Result = ModelBindingResult.Success(actualValue);
            }
            catch (FormatException e)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, e, null);
            }

            return Task.CompletedTask;
        }
    }
}