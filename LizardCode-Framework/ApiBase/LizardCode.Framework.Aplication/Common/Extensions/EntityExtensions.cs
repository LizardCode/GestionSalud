using Dapper.Contrib.Extensions;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace LizardCode.Framework.Application.Common.Extensions
{
    public static class EntityExtensions
    {
        public static SelectList ToDropDownList<T>(this IEnumerable<T> list,
            Expression<Func<T, object>> key,
            Expression<Func<T, object>> description,
            Expression<Func<T, object>> code = null,
            Expression<Func<T, object>> order = null,
            bool descriptionIncludesKey = false,
            string selectedKey = null)
        {

            if (order != null)
            {
                list = list
                    .OrderBy(order.Compile())
                    .ToList();
            }

            var piKey = key.GetPropertyInfo();
            var piDescription = description.GetPropertyInfo();
            var piCode = default(PropertyInfo);

            if (code != null)
            {
                piCode = code.GetPropertyInfo();
            }

            var digits = list.Count().ToString().Length;

            var resultList = list
                .Select(entity => new
                {
                    Key = piKey.GetValue(entity),
                    Description = piDescription.GetValue(entity).ToString(),
                    Code = piCode == null ? null : ((int)piCode.GetValue(entity)).ToString(new string('0', digits))
                })
                .Select(entity =>
                {
                    var text = entity.Description;

                    if (descriptionIncludesKey)
                    {
                        if (piKey.PropertyType.IsNumericType())
                        {
                            text = $"{entity.Code ?? ((int)entity.Key).ToString(new string('0', digits))}) {entity.Description}";
                        }

                        if (piKey.PropertyType.IsAssignableFrom(typeof(string)))
                        {
                            text = $"{entity.Code ?? entity.Key}) {entity.Description}";
                        }
                    }

                    return new
                    {
                        Value = entity.Key.ToString(),
                        Text = text,
                        Selected = selectedKey != null && entity.Key.ToString() == selectedKey
                    };
                })
                .ToList();

            return new SelectList(resultList, "Value", "Text");
        }

        public static SelectList ToDropDownList(this Dictionary<object, string> list,
            bool descriptionIncludesKey = true,
            string selectedKey = null)
        {
            var digits = list.Count.ToString().Length;
            var resultList = list
                .Select(item => new
                {
                    Key = item.Key,
                    Description = item.Value
                })
                .Select(item =>
                {
                    var text = item.Description;

                    if (descriptionIncludesKey)
                    {
                        if (item.Key.GetType().IsNumericType())
                        {
                            text = $"{((int)item.Key).ToString(new string('0', digits))}) {item.Description}";
                        }
                        else if (item.Key.GetType().IsAssignableFrom(typeof(string)))
                        {
                            text = $"{item.Key}) {item.Description}";
                        }
                    }

                    return new
                    {
                        Value = item.Key.ToString(),
                        Text = text,
                        Selected = selectedKey != null && item.Key.ToString() == selectedKey
                    };
                })
                .ToList();

            return new SelectList(resultList, "Value", "Text");
        }


        public static string Table(this Type entityType)
        {
            var attribute = entityType.GetCustomAttribute<TableAttribute>();

            if (attribute == null)
                return null;

            return attribute.Name;
        }
    }
}