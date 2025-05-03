using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Globalization;

namespace LizardCode.Framework.Application.Binders
{
    public class DoubleBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(double))
            {
                return new BinderTypeModelBinder(typeof(DoubleBinder));
            }

            return null;
        }
    }

	public class DoubleBinder : IModelBinder
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
					values.FirstValue.IsNull()
						? default
						: Convert.ToDouble(values.FirstValue, CultureInfo.InvariantCulture);

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
