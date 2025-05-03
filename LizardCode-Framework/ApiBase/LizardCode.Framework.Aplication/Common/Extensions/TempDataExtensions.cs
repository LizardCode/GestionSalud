using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace LizardCode.Framework.Application.Common.Extensions
{
    public static class TempDataExtensions
    {
        public static void Add<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            if (tempData.TryGetValue(key, out var value))
                return JsonConvert.DeserializeObject<T>((string)value);

            return null;
        }
    }
}
