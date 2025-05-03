using LizardCode.Framework.Helpers.Utilities.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace LizardCode.Framework.Helpers.Utilities
{
    public static class Extensions
    {
        private static IConfiguration _configuration;
        private static IHttpContextAccessor _httpContextAccessor;
        private static IMemoryCache _memoryCache;

        public static void Configure(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }


        public static string FromConnections(this string key, bool notFoundException = true)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var value = _configuration.GetConnectionString(key);

            if (value == null && notFoundException)
            {
                throw new KeyNotFoundException("'" + key.Trim() + "' not found in ConnectionStrings.");
            }

            return value;
        }

        public static T FromAppSettings<T>(this string key, T defaultValue = default, bool notFoundException = false) =>
            FromAppSettings(key, "AppSettings", defaultValue, notFoundException);

        public static T FromAppSettings<T>(this string key, string section, T defaultValue = default, bool notFoundException = false)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }

            var configurationSection = _configuration.GetSection(section).GetSection(key);
            object value;

            if (typeof(T).IsArray)
            {
                value = configurationSection?.AsEnumerable()
                    .Skip(1)
                    .Select(item => item.Value)
                    .Reverse()
                    .ToArray();
            }
            else
            {
                value = configurationSection?.Value;
            }

            if (value == null)
            {
                if (notFoundException)
                    throw new KeyNotFoundException("'" + key.Trim() + "' not found in AppSettings.");

                return defaultValue;
            }

            if (value is string && string.IsNullOrWhiteSpace((string)value))
                return defaultValue;

            return Utilities.CastValue(value, defaultValue);
        }

        public static T FromQueryString<T>(this string key, T defaultValue = default, bool notFoundException = false)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null || context.Request == null)
            {
                throw new InvalidOperationException();
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }

            var value = default(string);

            if (context.Request.Query.TryGetValue(key, out StringValues queryValue))
            {
                value = queryValue.ToString();
            }
            else if (context.Request.Form.TryGetValue(key, out StringValues formValue))
            {
                value = formValue.ToString();
            }

            if (value == null && notFoundException)
            {
                throw new KeyNotFoundException("'" + key.Trim() + "' not found in QueryString.");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return Utilities.CastValue(value, defaultValue);
        }

        public static T FromSession<T>(this string key, T defaultValue = default, bool notFoundException = false)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                throw new InvalidOperationException();
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }

            if (context.Session == null || !context.Session.Keys.Any())
            {
                return defaultValue;
            }

            if (!context.Session.Keys.Any(a => a.Equals(key)) && notFoundException)
            {
                throw new KeyNotFoundException("'" + key.Trim() + "' not found in Session.");
            }

            var obj = context.Session.GetString(key);

            if (obj == null)
            {
                return defaultValue;
            }

            return Utilities.CastValue(obj, defaultValue);
        }


        public static string Description(this Enum value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            try
            {
                var type = value.GetType();
                var field = type.GetField(value.ToString());
                var attr = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                var desc = ((DescriptionAttribute)attr[0]).Description;

                return desc;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string Code(this Enum value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            try
            {
                var type = value.GetType();
                var field = type.GetField(value.ToString());
                var attr = field.GetCustomAttributes(typeof(CodeAttribute), false);

                var result = ((CodeAttribute)attr[0]).Value;

                return result;
            }
            catch
            {
                return string.Empty;
            }
        }


        public static SqlCommand Query(this SqlCommand cmd, string query)
        {
            if (cmd == null || string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            var q = query; // Fortify. No remover esta línea.
            cmd.CommandText = q;

            return cmd;
        }

        public static SqlCommand AddParam(this SqlCommand cmd, string name, object value)
        {
            if (cmd == null || string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            cmd.Parameters.AddWithValue(name, value);

            return cmd;
        }

        public static SqlCommand ResetParams(this SqlCommand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            cmd.Parameters.Clear();

            return cmd;
        }

        public static string GetIP(this HttpRequest request, bool CheckForward = false)
        {
            var ip = string.Empty;

            // Se utiliza en desarrollo local para evitar que devuelva IPV6 que no nos sirve.
            if (request.Host.Value.Contains("localhost"))
            {
                // En caso de localhost no hay FORWARD...
                ip = GetIPV4(request);
            }
            else
            {
                ip = string.Empty;

                if (CheckForward)
                {
                    if (request.Headers.ContainsKey("HTTP_X_FORWARDED_FOR"))
                    {
                        ip = request.Headers["HTTP_X_FORWARDED_FOR"];
                    }
                }

                if (string.IsNullOrEmpty(ip))
                {
                    ip = request.HttpContext.Connection.RemoteIpAddress.ToString();
                }
                else
                { // Using X-Forwarded-For last address
                    ip = ip.Split(',').Last().Trim();
                }
            }

            return ip;
        }

        private static string GetIPV4(this HttpRequest request)
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(request.HttpContext.Connection.LocalIpAddress.ToString()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            if (IP4Address != String.Empty)
            {
                return IP4Address;
            }

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }

        public static T NewInstanceOf<T>(T input) where T : class
        {
            if (input == null)
            {
                return null;
            }

            var s = new XmlSerializer(input.GetType());
            var w = new StringWriter();
            T o = null;

            s.Serialize(w, input);
            var r = new StringReader(w.ToString());
            o = (T)s.Deserialize(r);

            w.Close();
            r.Close();

            return o;
        }

        public static string RegexReplace(this string input, string match, string replacement)
        {
            return Regex.Replace(input, match, replacement, RegexOptions.IgnoreCase);
        }

        public static string SubstituteSpecial(this string input)
        {
            if (input.IsNull())
            {
                return null;
            }

            var chars = new Dictionary<string, string>
            {
                { "á", "a" }, { "Á", "Á" },
                { "é", "e" }, { "É", "E" },
                { "í", "i" }, { "Í", "I" },
                { "ó", "o" }, { "Ó", "O" },
                { "ú", "u" }, { "Ú", "U" },
                { "ñ", "ñ" }, { "Ñ", "N" }
            };

            var result = input;

            foreach (var pair in chars)
                if (result.Contains(pair.Key))
                    result = result.Replace(pair.Key, pair.Value);

            return result;

        }

        public static string Extract(this string input, int lenght = 30)
        {
            if (input.IsNull())
                return null;

            if (input.Length < lenght)
                return input;

            return input.Substring(0, lenght).Trim();
        }

        public static string ToHexString(this byte[] input)
        {
            var sb = new StringBuilder();

            foreach (var b in input)
            {
                sb.Append(string.Format("{0:x2}", b));
            }

            return sb.ToString();
        }

        public static T GetValue<T>(this DataRow row, string column, T defaultValue = default, bool notFoundException = true)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            if (string.IsNullOrWhiteSpace(column))
            {
                throw new ArgumentNullException(nameof(column));
            }

            try
            {
                var value = row[column];

                if (value == DBNull.Value)
                {
                    return defaultValue;
                }
                else if (typeof(T).IsArray)
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                else
                {
                    var tc = Type.GetTypeCode(typeof(T));
                    return (T)Convert.ChangeType(value, tc);
                }
            }
            catch (ArgumentException)
            {
                return defaultValue;
            }
        }

        public static DateTime FromUnixTime(this int units, bool inMilliseconds = false)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            if (inMilliseconds)
            {
                return epoch.AddMilliseconds(units);
            }
            else
            {
                return epoch.AddSeconds(units);
            }
        }

        public static void RetryWhenFail(Func<int, bool> action, bool throwWhenOut = false, int retries = 3)
        {
            if (action == null)
            {
                return;
            }

            var @try = 1;
            var @continue = true;

            while ((@try < retries) && @continue)
            {
                if (action.Invoke(@try))
                {
                    @continue = false;
                }

                @try++;
            }

            if (throwWhenOut && (@try > retries))
            {
                throw new IndexOutOfRangeException();
            }
        }

        public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(this Dictionary<TKey, TValue> input, Dictionary<TKey, TValue> range)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (range == null || range.Count == 0)
            {
                return input;
            }

            foreach (var v in range)
            {
                if (input.ContainsKey(v.Key))
                {
                    throw new Exception("La clave ya existe '" + v.Key + "'");
                }
                else
                {
                    input.Add(v.Key, v.Value);
                }
            }

            return input;
        }

        public static string UTF8ToBase64(this string utf8String)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(utf8String);
            var text = Convert.ToBase64String(bytes);
            return text;
        }

        public static string Base64ToUTF8(this string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            var text = System.Text.Encoding.UTF8.GetString(bytes);
            return text;
        }

        public static string ToShortDate(this string input, string format = "yyyyMMdd")
        {
            if (input.IsNull())
            {
                return null;
            }

            return DateTime.ParseExact(input, format, null).ToString("dd/MM/yyyy");
        }

        public static DateTime? ToDate(this string input, string format = "yyyyMMdd")
        {
            if (input.IsNull())
            {
                return null;
            }


            if (DateTime.TryParseExact(input, format, null, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            else
            {
                return null;
            }
        }

        public static string ToDateString(this string input, string inputFormat = "yyyyMMdd", string outputFormat = "dd/MM/yyyy")
        {
            var date = input.ToDate(inputFormat);

            if (date.HasValue)
            {
                return date.Value.ToString(outputFormat);
            }
            else
            {
                return null;
            }
        }

        public static T FromCache<T>(this string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_memoryCache == null)
            {
                throw new NullReferenceException(nameof(_memoryCache));
            }

            return (T)_memoryCache.Get(key);
        }

        public static T FromCache<T>(this string key, Func<T> getItemMethod)
        {
            return FromCache(key, getItemMethod, "CacheItemTimeout", null, true);
        }

        public static T FromCache<T>(this string key, Func<T> getItemMethod, int? timeout)
        {
            return FromCache(key, getItemMethod, string.Empty, timeout, true);
        }

        public static T FromCache<T>(this string key, Func<T> getItemMethod, bool infiniteUpdate)
        {
            return FromCache(key, getItemMethod, "CacheItemTimeout", null, infiniteUpdate);
        }

        public static T FromCache<T>(this string key, Func<T> getItemMethod, int? timeout, bool infiniteUpdate)
        {
            return FromCache(key, getItemMethod, string.Empty, timeout, infiniteUpdate);
        }

        public static T FromCache<T>(this string key, Func<T> getItemMethod, string cacheTimeoutKey)
        {
            return FromCache(key, getItemMethod, cacheTimeoutKey, null, true);
        }

        public static T FromCache<T>(this string key, Func<T> getItemMethod, string cacheTimeoutKey, bool infiniteUpdate)
        {
            return FromCache(key, getItemMethod, cacheTimeoutKey, null, infiniteUpdate);
        }

        public static T FromCache<T>(this string key, Func<T> getItemMethod, string cacheTimeoutKey = "CacheItemTimeout", int? timeout = null, bool infiniteUpdate = true, bool noExpiration = false)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (getItemMethod == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrWhiteSpace(cacheTimeoutKey) && !timeout.HasValue)
            {
                throw new ArgumentException("Timeout no definido", nameof(cacheTimeoutKey));
            }

            var cacheTimeout = timeout ?? cacheTimeoutKey.FromAppSettings(5.00);

            if (_memoryCache == null || cacheTimeout <= 0.00)
            {
                var item = getItemMethod.Invoke();

                if (item == null)
                {
                    return default;
                }

                return item;
            }

            if (_memoryCache.Get(key) == null)
            {
                var item = getItemMethod.Invoke();

                if (item == null)
                {
                    return default;
                }

                if (noExpiration)
                {
                    _memoryCache.Set(key, item);
                }
                else
                {
                    throw new NotImplementedException();
                }
                //      MemoryCache.Set(
                //            key,
                //            item,
                //            null,
                //            DateTime.UtcNow.AddMinutes(cacheTimeout),
                //            Cache.NoSlidingExpiration,
                //            (string k, CacheItemUpdateReason r, out object o, out CacheDependency d, out DateTime e, out TimeSpan s) =>
                //            {
                //                d = null;
                //                e = (infiniteUpdate ? DateTime.Now.AddMinutes(cacheTimeout) : DateTime.Now);
                //                s = Cache.NoSlidingExpiration;

                //                o = (infiniteUpdate ? getItemMethod.Invoke() : default);
                //            }
                //        );

                return item;
            }
            else
            {
                return (T)_memoryCache.Get(key);
            }
        }

        public static string ToCurrency(this double input, bool withSymbol = true, bool withDecimals = true)
        {
            input = (withDecimals ? input : Math.Truncate(input));

            return input
                .ToString((withDecimals ? "C2" : "C0"), System.Globalization.CultureInfo.CreateSpecificCulture("es-AR"))
                .Replace("$", (withSymbol ? "$ " : string.Empty));
        }

        public static string ToPercent(this double input)
        {
            return (input / 100)
                .ToString("P1", System.Globalization.CultureInfo.CreateSpecificCulture("es-AR"));
        }

        public static bool IsNull(this string input)
        {
            return string.IsNullOrWhiteSpace(input);
        }

        public static bool IsNotNull(this string input)
        {
            return !input.IsNull();
        }

        public static bool IsDefault<T>(this T input)
        {
            return input.Equals(default(T));
        }

        public static bool IsNumericType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static string ValueOrEmpty(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            return input;
        }

        /// <summary>
        /// Evalua si la lista es nulla o vacia
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="exceptionMessage">Si no es vacío, se lanza excepción con el mensaje</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this List<T> list, string exceptionMessage = null) where T : class
        {
            bool result = false;

            if (list == null || list.Count == 0)
            {
                result = true;
            }

            if (result && !string.IsNullOrWhiteSpace(exceptionMessage))
            {
                throw new NullReferenceException(exceptionMessage);
            }

            return result;
        }

        public static string ToDb(this bool value)
        {
            if (value)
            {
                return "S";
            }

            return "N";
        }
        /// <summary>
        /// Diccionario 
        /// </summary>
        /// <param name="obj">Objeto JToken</param>
        /// <param name="name">Nombre del la propiedad</param>
        /// <returns></returns>
        public static Dictionary<string, object> Bagify(this JToken obj, string name = null)
        {
            name = name ?? "obj";
            if (obj is JObject)
            {
                IEnumerable<KeyValuePair<string, object>> asBag = from prop in (obj as JObject).Properties()
                                                                  let propName = prop.Name
                                                                  let propValue =
                                                                  prop.Value is JValue
                                                                  ? new Dictionary<string, object>()
                                                                  {
                                                                        {
                                                                        prop
                                                                        .Name,
                                                                        prop
                                                                        .Value
                                                                        }
                                                                  }
                                                                  : prop.Value.Bagify(prop.Name)
                                                                  select
                                                                  new KeyValuePair<string, object>(
                                                                  propName,
                                                                  propValue);
                return asBag.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            if (obj is JArray)
            {
                IJEnumerable<JToken> vals = (obj as JArray).Values();
                object[] alldicts = vals.SelectMany(val => val.Bagify(name)).Select(x => x.Value).ToArray();
                return new Dictionary<string, object>() { { name, alldicts } };
            }

            if (obj is JValue)
            {
                return new Dictionary<string, object>() { { name, (obj as JValue) } };
            }

            return new Dictionary<string, object>() { { name, null } };
        }


        public static IDictionary<string, object> ToDictionaryLog<TEntity>(this IList<TEntity> lista)
        {
            Dictionary<string, object> dict = new();
            if (lista != null)
            {
                for (var i = 0; i <= lista.Count - 1; i++)
                {
                    dict.Add(i.ToString(), lista[i].ToDictionaryLog());
                }
            }

            return dict;
        }
        public static IDictionary<string, object> ToDictionaryLog(this object obj)
        {
            IDictionary<string, object> ret = null;

            try
            {
                if (obj == null)
                {
                    ret = null;
                }
                else
                {
                    ret = obj.ToDictionary();
                }
            }
            catch (Exception)
            {
                ret = new Dictionary<string, object>
                {
                    { "ERROR INTERNO", "Error al convertir a diccionario." }
                };
            }

            return ret;
        }

        /// <summary>Convierte una instancia de clase en su representacion
        ///     como IDictionary&lt;string, object&gt;.</summary>
        /// <param name="source">Objeto a convertir a diccionario.</param>
        /// <returns>El objeto convertido a diccionario del tipo clave (string), valor (object).</returns>
        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return ToDictionary(source, false);
        }

        /// <summary>Convierte una instancia de clase en su representacion
        ///     como IDictionary&lt;string, object&gt;.</summary>
        /// <param name="source">Objeto a convetir.</param>
        /// <param name="skipNull">No agrega los valores nulos al diccionario.</param>
        /// <returns>El objeto convertido a diccionario del tipo clave (string), valor (object).</returns>
        public static IDictionary<string, object> ToDictionary(this object source, bool skipNull)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Type type = source.GetType();
            const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.Instance;

            return
            type.GetProperties(BindingAttr)
            .Where(propInfo => !skipNull || (propInfo.GetValue(source) != null))
            .ToDictionary(propInfo => propInfo.Name, propInfo => propInfo.GetValue(source));
        }

        /// <summary>Devuelve un diccionario con los pares de clave : valor recibidos como parametros.</summary>
        /// <typeparam name="TKey">Tipo de dato de la clave.</typeparam>
        /// <typeparam name="TValue">Tipo de dato de los valores.</typeparam>
        /// <param name="source">Objeto origen.</param>
        /// <param name="list">Lista de valores con los que generar el dicc.</param>
        /// <returns>Un diccionario generado seg n los valores.</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this object source, params object[] list)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (list.Length % 2 != 0)
            {
                throw new ArgumentException(
                "La cantidad de parametros recibidos no corresponde a una cantidad validad de parametros.");
            }

            Dictionary<TKey, TValue> result = new(list.Length);
            for (int i = 0; i < list.Length; i += 2)
            {
                result.Add((TKey)list[i], (TValue)list[i + 1]);
            }

            return result;
        }

        /// <summary>
        /// To the yes no text Spanish.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        public static string ToYesNoText(this bool? bText)
        {
            if (bText.HasValue)
            {
                if (bText.Value)
                {
                    return "Si";
                }

                return "No";
            }

            return "-";
        }

        /// <summary>
        /// To the date string.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="outputFormat">The output format.</param>
        /// <returns></returns>
        public static string ToDateString(this DateTime? date, string outputFormat = "dd/MM/yyyy")
        {
            if (date.HasValue)
            {
                return date.Value.ToString(outputFormat);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// To the date string.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="outputFormat">The output format.</param>
        /// <returns></returns>
        public static string ToDateString(this DateTime date, string outputFormat = "dd/MM/yyyy")
        {

            return date.ToString(outputFormat);

        }

        /// <summary>
        /// Dada una edad de nacimiento calcula su edad actual
        /// </summary>
        /// <param name="nacimiento">Fecha de nacimiento</param>
        /// <returns>Años, meses y días</returns>
        public static int[] Edad(this DateTime nacimiento)
        {
            var ahora = DateTime.Now;
            int años, meses, dias;
            int DaysInBdayMonth = DateTime.DaysInMonth(nacimiento.Year, nacimiento.Month);
            int DaysRemain = ahora.Day + (DaysInBdayMonth - nacimiento.Day);

            if (ahora.Month > nacimiento.Month)
            {
                años = ahora.Year - nacimiento.Year;
                meses = ahora.Month - (nacimiento.Month + 1) + Math.Abs(DaysRemain / DaysInBdayMonth);
                dias = (DaysRemain % DaysInBdayMonth + DaysInBdayMonth) % DaysInBdayMonth;
            }
            else if (ahora.Month == nacimiento.Month)
            {
                if (ahora.Day >= nacimiento.Day)
                {
                    años = ahora.Year - nacimiento.Year;
                    meses = 0;
                    dias = ahora.Day - nacimiento.Day;
                }
                else
                {
                    años = (ahora.Year - 1) - nacimiento.Year;
                    meses = 11;
                    dias = DateTime.DaysInMonth(nacimiento.Year, nacimiento.Month) - (nacimiento.Day - ahora.Day);
                }
            }
            else
            {
                años = (ahora.Year - 1) - nacimiento.Year;
                meses = ahora.Month + (11 - nacimiento.Month) + Math.Abs(DaysRemain / DaysInBdayMonth);
                dias = (DaysRemain % DaysInBdayMonth + DaysInBdayMonth) % DaysInBdayMonth;
            }

            return new[] { años, meses, dias };
        }


        /// <summary>
        /// To the yes no text Spanish.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        public static string ToSiNoTexto(this bool? bText)
        {
            if (bText.HasValue)
            {
                if (bText.Value)
                {
                    return "Si";
                }

                return "No";
            }

            return "-";
        }

        /// <summary>
        /// To the si no text.
        /// </summary>
        /// <param name="texto">The texto.</param>
        /// <returns></returns>
        public static string ToSiNoText(this string texto)
        {
            if (!string.IsNullOrEmpty(texto))
            {
                switch (texto.ToUpper())
                {
                    case "SI":
                    case "S":
                    case "Y":
                    case "1":
                        return "Si";
                    case "NO":
                    case "N":
                    case "0":
                        return "No";
                    default:

                        break;
                }

            }

            return string.Empty;
        }


        /// <summary>
        /// To the texto empty.
        /// </summary>
        /// <param name="texto">The texto.</param>
        /// <returns></returns>
        public static string ToTexto(this string texto)
        {
            return string.IsNullOrEmpty(texto) ? "-" : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto.ToLower());
        }

        /// <summary>
        /// To the texto.
        /// </summary>
        /// <param name="texto">The texto.</param>
        /// <returns></returns>
        public static string ToTextoEmpty(this string texto)
        {
            return string.IsNullOrEmpty(texto) ? string.Empty : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto.ToLower());
        }

        /// <summary>
        /// Parses to nullable int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int? ParseToNullableInt(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                //OMI: Temporalmente cambié ya que no tengo el VS2017 ni el espacio en disco para instalarlo
                return int.TryParse(value, out int iValue) ? iValue : null;
            }

            return null;
        }

        /// <summary>
        /// Parses to nullable time span.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static TimeSpan? ParseToNullableTimeSpan(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                //OMI: Temporalmente cambié ya que no tengo el VS2017 ni el espacio en disco para instalarlo
                return TimeSpan.TryParse(value, out TimeSpan iValue) ? iValue : null;
            }

            return null;
        }

        /// <summary>
        /// To the time string.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="outputFormat">The output format.</param>
        /// <returns></returns>
        public static string ToTimeString(this TimeSpan? time, string outputFormat = @"hh\:mm")
        {
            if (time.HasValue)
            {
                return time.Value.ToString(outputFormat);
            }
            else
            {
                return null;
            }
        }

        public static PropertyInfo GetPropertyInfo<T>(this Expression<Func<T, object>> property)
        {
            if (property == null)
            {
                return null;
            }

            var me = (
                property.Body is UnaryExpression ue
                    ? ue.Operand as MemberExpression
                    : property.Body as MemberExpression
            );

            if (me == null)
            {
                throw new InvalidExpressionException();
            }

            var pi = me.Member as PropertyInfo;

            if (pi == null)
            {
                throw new InvalidExpressionException();
            }

            return pi;
        }

        public static Dictionary<string, object> Merge(this Dictionary<string, object> input, string key, string value)
        {
            if (!input.ContainsKey(key))
            {
                input.Add(key, value);
                return input;
            }

            if (input[key].GetType().IsAssignableFrom(typeof(string)))
                input[key] = $"{input[key]} {value}";

            return input;
        }

        public static Dictionary<string, object> Append(this Dictionary<string, object> input, string key, string value)
        {
            if (!input.ContainsKey(key))
                input.Add(key, value);

            return input;
        }

        public static byte[] ToByteArray(this Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Provide a integer conversion extension to a string 
        /// </summary>
        /// <param name="s">string to convert to an Int</param>
        /// <returns>int value represented in the string or 0</returns>
        public static int ToInt(this string s)
        {

            if (int.TryParse(s, out int o) == false)
            {
                o = 0;
            }

            return o;
        }

        /// <summary>
        /// Provide a decimal conversion extension to a string 
        /// </summary>
        /// <param name="s">string to convert to an decimal</param>
        /// <returns>decimal value represented in the string or 0</returns>
        public static decimal ToDecimal(this string s)
        {

            if (decimal.TryParse(s, out decimal o) == false)
            {
                o = 0.00M;
            }

            return o;
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

        public static bool ValidarCUIT(this string CUIT)
        {
            if (CUIT.IsNull())
            {
                return false;
            }

            CUIT = CUIT.Replace("-", string.Empty).Replace(" ", string.Empty).Replace("_", string.Empty);

            if (CUIT.Length != 11)
            {
                return false;
            }
            else
            {
                int[] mult = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
                char[] nums = CUIT.ToCharArray();
                int total = 0;

                for (int i = 0; i < mult.Length; i++)
                {
                    total += int.Parse(nums[i].ToString()) * mult[i];
                }

                var resto = total % 11;
                int calculado = resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;
                int digito = int.Parse(CUIT.Substring(10));

                return calculado == digito;
            }
        }

        public static string NumeroALetras(this decimal numberAsString)
        {
            string dec;

            var entero = Convert.ToInt64(Math.Truncate(numberAsString));
            var decimales = Convert.ToInt32(Math.Round((numberAsString - entero) * 100, 2));
            if (decimales > 0)
            {
                //dec = " PESOS CON " + decimales.ToString() + "/100";
                dec = $" PESOS {decimales:0,0} /100";
            }
            //Código agregado por mí
            else
            {
                //dec = " PESOS CON " + decimales.ToString() + "/100";
                dec = $" PESOS {decimales:0,0} /100";
            }
            var res = Utilities.NumeroALetras(Convert.ToDouble(entero)) + dec;
            return res;
        }

        public static string AbsoluteUrl(this IHttpContextAccessor httpContextAccessor, string relativeUrl, object parameters = null)
        {
            //
            // https://stackoverflow.com/a/54094034/1812392
            //

            var request = httpContextAccessor.HttpContext.Request;

            var url = new Uri(new Uri($"{request.Scheme}://{request.Host.Value}"), relativeUrl).ToString();

            if (parameters != null)
            {
                var serialized = JsonConvert.SerializeObject(parameters);
                var deserialized = JsonConvert.DeserializeObject<Dictionary<string, string>>(serialized);
                url = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(url, deserialized);
            }

            return url;
        }
    }
}
