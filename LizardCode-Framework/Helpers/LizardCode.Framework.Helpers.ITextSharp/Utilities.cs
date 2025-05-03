using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LizardCode.Framework.Helpers.ITextSharp
{
    public static class Extensions
    {
        public static ITFont Size(this ITFont input, float size)
        {
            input.Size = size;
            return input;
        }

        public static ITFont Bold(this ITFont input, bool bold)
        {
            input.Bold = bold;
            return input;
        }

        public static Font Font(this ITFont input)
        {
            return new Font(input.BaseFont, input.Size);
        }

        public static PdfContentByte SetFont(this PdfContentByte canvas, ITFont font)
        {
            canvas.SetFontAndSize(font.BaseFont, font.Size);
            return canvas;
        }

        public static string Escape(this string input)
        {
            return input
                .Replace("\r\n", "[@NewLine@]")
                .Replace("\r", "[@NewLine@]")
                .Replace("\n", "[@NewLine@]")
                .Replace("[@NewLine@]", "\r\n")
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }

        public static string ReplaceLabel(this string input, string label, string value, bool escapeSymbols = true)
        {
            if (escapeSymbols)
                value = value.Escape();

            var match = Regex.Match(input, @"\B\" + label + @"\B((?:\[\d+\])?)");

            if (string.IsNullOrEmpty(match.Groups[1].Value))
                return input.Replace(label, value);
            else
            {
                var token = match.Groups[0].Value;
                var width = Convert.ToInt32(match.Groups[1].Value.Replace("[", "").Replace("]", ""));

                return input.Replace(token.PadRight(width), value.PadRight(width).Substring(0, width));
            }
        }

        public static JsonDocument ReplaceLabel(this JsonDocument document, string label, string value)
        {
            if (string.IsNullOrWhiteSpace(label))
                return document;

            var copy = document.Clone();

            if (copy.Elements != null)
                foreach (var element in copy.Elements)
                    element.Text = element
                        .Text
                        .ReplaceLabel(label, value);

            if (copy.NewPageElements != null)
                foreach (var element in copy.NewPageElements)
                    element.Text = element
                        .Text
                        .ReplaceLabel(label, value);

            return copy;
        }

        public static JsonDocument ReplaceBinary(this JsonDocument document, string name, byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return document;

            var copy = document.Clone();

            if (copy.Elements != null)
                foreach (var element in copy.Elements.Where(w => w.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                    element.Bytes = buffer;

            if (copy.NewPageElements != null)
                foreach (var element in copy.NewPageElements.Where(w => w.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                    element.Bytes = buffer;

            return copy;
        }
    }

    public static class Utils
    {
        public static T CastValue<T>(object value, T defaultValue)
        {
            T output;
            var stringValue = (value is string ? (string)value : null);

            var tc = Type.GetTypeCode(typeof(T));

            switch (tc)
            {
                case TypeCode.Boolean:

                    if (string.IsNullOrWhiteSpace(stringValue))
                        throw new NotSupportedException();

                    var val = 0;

                    if (stringValue.Trim().ToLower() == "true" ||
                        (int.TryParse(stringValue.Trim(), out val) && val >= 1))
                        output = (T)Convert.ChangeType(true, tc);
                    else
                        output = (T)Convert.ChangeType(false, tc);

                    return output;

                case TypeCode.Byte:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.String:

                    try
                    {
                        output = (T)Convert.ChangeType(value, tc);
                    }
                    catch
                    {
                        output = defaultValue;
                    }

                    return output;

                case TypeCode.Object:

                    if (typeof(T) == typeof(Dictionary<string, string>))
                    {
                        if (string.IsNullOrWhiteSpace(stringValue))
                            throw new NotSupportedException();

                        var d = new Dictionary<string, string>();

                        var entries = stringValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        if (entries == null || entries.Length == 0)
                            return defaultValue;

                        foreach (var e in entries)
                        {
                            if (e.Contains(":"))
                            {
                                var entry = e.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                                if (entry == null || entry.Length != 2 ||
                                    string.IsNullOrWhiteSpace(entry[0]) || string.IsNullOrWhiteSpace(entry[1]))
                                    throw new FormatException();

                                d.Add(entry[0], entry[1]);
                            }
                            else
                                d.Add(e, "");
                        }

                        return (T)Convert.ChangeType(d, typeof(T));
                    }
                    else if (typeof(T) == typeof(List<string>))
                    {
                        if (string.IsNullOrWhiteSpace(stringValue))
                            throw new NotSupportedException();

                        var entries = stringValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        if (entries == null || entries.Length == 0)
                            return defaultValue;

                        return (T)Convert.ChangeType(new List<string>(entries), typeof(T));
                    }
                    else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var ut = Nullable.GetUnderlyingType(typeof(T));

                        if (!ut.IsPrimitive)
                            throw new NotSupportedException();

                        if (string.IsNullOrWhiteSpace(stringValue))
                            return default(T);

                        return (T)Convert.ChangeType(stringValue, ut);
                    }
                    else
                    {
                        try
                        {
                            return (T)Convert.ChangeType(value, typeof(T));
                        }
                        catch (Exception)
                        {
                            throw new NotSupportedException();
                        }
                    }

                default:
                    throw new NotSupportedException();
            }
        }

        public static string ResolvePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            return Path.GetFullPath(path.Replace("~", "."));
        }

        public static byte[] GetPDFContent(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new Exception();

            if (!Path.IsPathRooted(path))
                path = ResolvePath(path);

            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            return File.ReadAllBytes(path);
        }

        public static string GetJsonContent(string path, Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new Exception();

            if (!Path.IsPathRooted(path))
                path = ResolvePath(path);

            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            if (encoding != null)
                return File.ReadAllText(path, encoding);
            else
                return File.ReadAllText(path);
        }
    }
}
