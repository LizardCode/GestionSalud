using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Globalization;

namespace LizardCode.Framework.Application.Binders
{
    public class NullableDateTimeBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(DateTime?))
            {
                return new BinderTypeModelBinder(typeof(NullableDateTimeBinder));
            }

            return null;
        }
    }

    public class NullableDateTimeBinder : IModelBinder
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
                        ? default(DateTime?)
                        : DateTime.Parse(values.FirstValue, new CultureInfo("Es-ar"), DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces);

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