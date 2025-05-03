using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.Framework.Application.Interfaces.Models;
using System;
using System.Linq;
using System.Text;

namespace LizardCode.SalmaSalud.Application.Common.Extensions
{
    public static class ModelsExtensions
    {
        public static string ToStringAFIP(this ISicoreCiti input)
        {
            var properties = input.GetType()
                .GetProperties()
                .Where(w => w.GetCustomAttributes(typeof(SubstringAttribute), false).Any())
                .Select(s => new
                {
                    s.Name,
                    Value = s.GetValue(input),
                    Attribute = s.GetCustomAttributes(typeof(SubstringAttribute), false)
                        .Cast<SubstringAttribute>()
                        .SingleOrDefault()
                })
                .OrderBy(o => o.Attribute.Nro)
                .ToList();

            var result = new StringBuilder();
            properties.ForEach(p =>
            {
                switch (p.Attribute.AttrType)
                {
                    case SubstringAttributeType.DateTime:

                        result.Append(DateTime.Parse(p.Value.ToString()).ToString(p.Attribute.Format.IsNull() ? "dd/MM/yyyy" : p.Attribute.Format));
                        break;

                    case SubstringAttributeType.Integer:
                        result.Append(p.Value?.ToString().PadLeft(p.Attribute.Len, '0') ?? "0".PadLeft(p.Attribute.Len, '0'));
                        break;

                    case SubstringAttributeType.String:
                        if (p.Attribute.PadLeft.IsNull())
                        {
                            if (p.Value?.ToString().Length > p.Attribute.Len)
                                result.Append(p.Value?.ToString()[..p.Attribute.Len].PadRight(p.Attribute.Len, ' ') ?? string.Empty.PadRight(p.Attribute.Len, ' '));
                            else
                                result.Append(p.Value?.ToString().PadRight(p.Attribute.Len, ' ') ?? string.Empty.PadRight(p.Attribute.Len, ' '));
                        }
                        else
                        {
                            if (p.Value?.ToString().Length > p.Attribute.Len)
                                result.Append(p.Value?.ToString()[..p.Attribute.Len].PadLeft(p.Attribute.Len, char.Parse(p.Attribute.PadLeft)) ?? string.Empty.PadRight(p.Attribute.Len, char.Parse(p.Attribute.PadLeft)));
                            else
                                result.Append(p.Value?.ToString().PadLeft(p.Attribute.Len, char.Parse(p.Attribute.PadLeft)) ?? string.Empty.PadRight(p.Attribute.Len, char.Parse(p.Attribute.PadLeft)));
                        }
                        break;

                    case SubstringAttributeType.Double:
                        var value = string.Empty;
                        if (p.Attribute.PadLeft.IsNull())
                        {
                            value = double.Parse(p.Value.ToString()).ToString(p.Attribute.Format.IsNull() ? "#.00" : p.Attribute.Format).PadLeft(p.Attribute.Len, '0');
                            if (p.Attribute.SinPuntoDecimal)
                                value = $"0{double.Parse(p.Value.ToString()).ToString(p.Attribute.Format.IsNull() ? "#.00" : p.Attribute.Format).PadLeft(p.Attribute.Len, '0').Replace(",", string.Empty).Replace(".", string.Empty)}";
                        }
                        else
                        {
                            value = double.Parse(p.Value.ToString()).ToString(p.Attribute.Format.IsNull() ? "#.00" : p.Attribute.Format).PadLeft(p.Attribute.Len, char.Parse(p.Attribute.PadLeft));
                            if (p.Attribute.SinPuntoDecimal)
                                value = $"0{double.Parse(p.Value.ToString()).ToString(p.Attribute.Format.IsNull() ? "#.00" : p.Attribute.Format).PadLeft(p.Attribute.Len, char.Parse(p.Attribute.PadLeft)).Replace(",", string.Empty).Replace(".", string.Empty)}";
                        }
                        result.Append(value);
                        break;
                }
            });

            return result.ToString();
        }
    }
}
