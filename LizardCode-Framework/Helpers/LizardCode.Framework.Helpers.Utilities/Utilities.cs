using LizardCode.Framework.Helpers.Utilities.Mail;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LizardCode.Framework.Helpers.Utilities
{
    public static class Utilities
    {
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

        public static T CastValue<T>(object value, T defaultValue)
        {
            T output;
            var stringValue = (value is string ? (string)value : null);

            var t = typeof(T);

            if (t.IsEnum)
            {
                try
                {
                    return (T)Enum.Parse(t, stringValue);
                }
                catch
                {
                    return defaultValue;
                }
            }
            else if (t == typeof(TimeSpan))
            {
                try
                {
                    var ts = TimeSpan.Parse(stringValue);
                    return (T)Convert.ChangeType(ts, t);
                }
                catch
                {
                    return defaultValue;
                }
            }

            var tc = Type.GetTypeCode(t);

            switch (tc)
            {
                case TypeCode.Boolean:

                    if (string.IsNullOrWhiteSpace(stringValue))
                    {
                        throw new NotSupportedException();
                    }

                    var val = 0;

                    if (stringValue.Trim().ToLower() == "true" ||
                        (int.TryParse(stringValue.Trim(), out val) && val >= 1))
                    {
                        output = (T)Convert.ChangeType(true, tc);
                    }
                    else
                    {
                        output = (T)Convert.ChangeType(false, tc);
                    }

                    return output;

                case TypeCode.Char:
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

                    if (t == typeof(Dictionary<string, string>))
                    {
                        if (string.IsNullOrWhiteSpace(stringValue))
                        {
                            throw new NotSupportedException();
                        }

                        var d = new Dictionary<string, string>();

                        var entries = stringValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        if (entries == null || entries.Length == 0)
                        {
                            return defaultValue;
                        }

                        foreach (var e in entries)
                        {
                            if (e.Contains(":"))
                            {
                                var entry = e.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                                if (entry == null || entry.Length != 2 ||
                                    string.IsNullOrWhiteSpace(entry[0]) || string.IsNullOrWhiteSpace(entry[1]))
                                {
                                    throw new FormatException();
                                }

                                d.Add(entry[0], entry[1]);
                            }
                            else
                            {
                                d.Add(e, "");
                            }
                        }

                        return (T)Convert.ChangeType(d, t);
                    }
                    else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        if (string.IsNullOrWhiteSpace(stringValue))
                        {
                            throw new NotSupportedException();
                        }

                        var entries = stringValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        if (entries == null || entries.Length == 0)
                        {
                            return defaultValue;
                        }

                        var argumentType = t.GetGenericArguments().First();
                        var convertedList = (System.Collections.IList)Activator.CreateInstance(t);

                        entries
                            .ToList()
                            .ForEach(f => convertedList.Add(Convert.ChangeType(f, argumentType)));

                        return (T)convertedList;
                    }
                    else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var ut = Nullable.GetUnderlyingType(t);

                        if (!ut.IsPrimitive)
                        {
                            throw new NotSupportedException();
                        }

                        if (string.IsNullOrWhiteSpace(stringValue))
                        {
                            return default;
                        }

                        return (T)Convert.ChangeType(stringValue, ut);
                    }
                    else if (t.IsClass &&
                        stringValue != null &&
                        stringValue.Trim('\r', '\n').StartsWith("{") &&
                        stringValue.Trim('\r', '\n').EndsWith("}"))
                    {
                        try
                        {
                            return JsonConvert.DeserializeObject<T>(stringValue);
                        }
                        catch (Exception)
                        {
                            return defaultValue;
                        }
                    }
                    else if (t.IsArray)
                    {
                        try
                        {
                            var elementType = t.GetElementType();
                            var arr = ((string[])value).Select(s => Convert.ChangeType(s, elementType)).ToArray();
                            var newArr = Array.CreateInstance(elementType, arr.Length);

                            Array.Copy(arr, newArr, arr.Length);

                            return (T)Convert.ChangeType(newArr, t);
                        }
                        catch (Exception)
                        {
                            throw new NotSupportedException();
                        }
                    }
                    else
                    {
                        try
                        {
                            return (T)Convert.ChangeType(value, t);
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

        public static string Encrypt(object p)
        {
            throw new NotImplementedException();
        }

        public static uint IPAddressToUInt(string ip)
        {
            var bytes = IPAddress.Parse(ip).GetAddressBytes();

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToUInt32(bytes, 0);
        }

        public static string GetResourceValue(string name, string key)
        {
            throw new NotImplementedException();

            //if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(key))
            //    return null;

            //try
            //{
            //    return HttpContext.GetGlobalResourceObject(name, key).ToString();
            //}
            //catch
            //{
            //    return null;
            //}
        }

        /// <summary>
        /// Busca el recurso en una jerarquia de archivos de recurso definida en la lista 'resourceFileNames'.
        /// </summary>
        public static string GetResourceValue(IEnumerable<string> resourceFileNames, string key)
        {
            throw new NotImplementedException();

            //if (resourceFileNames == null || resourceFileNames.Count() == 0)
            //    return null;

            //foreach (var resFN in resourceFileNames)
            //{
            //    try { return HttpContext.GetGlobalResourceObject(resFN, key).ToString(); }
            //    catch { }
            //}

            //return null;
        }

        public static string GetLocalizedHtml(string file)
        {
            return GetLocalizedHtml(file, new List<string>() { file });
            //return GetLocalizedHtml(file, new List<string>() { file, "common" });
        }

        public static string GetLocalizedHtml(string file, IEnumerable<string> resourceFileNames)
        {
            throw new NotImplementedException();

            //if (string.IsNullOrWhiteSpace(file))
            //    return null;

            //var path = HostingEnvironment.MapPath("~/html/" + file + ".html");

            //if (string.IsNullOrWhiteSpace(path))
            //    return null;

            //var html = File.ReadAllText(path);

            //if (string.IsNullOrWhiteSpace(html))
            //    return null;

            //var matches = Regex.Matches(html, @"\[@[a-zA-Z\s]*\]");

            //if (matches.Count == 0)
            //    return html;

            //foreach (Match m in matches)
            //{
            //    var key = Regex.Match(m.Value, @"^\[@([a-zA-Z\s]*)\]$").Groups[1].Value;
            //    var newValue = GetResourceValue(resourceFileNames, key.Replace(" ", ""));
            //    if (string.IsNullOrWhiteSpace(newValue))
            //        newValue = key;

            //    html = html.Replace(m.Value, newValue);
            //}

            //return html;
        }

        public static double MeassureDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            /* Conjuro oscuro Made by Tibu */
            var aPlana = 298.25722;
            var radioPolar = 6378137.00;

            var f4 = latitude1 / 180 * Math.PI;
            var f5 = latitude2 / 180 * Math.PI;
            var f6 = f4 + f5;

            var f8 = longitude1 / 180 * Math.PI;
            var f9 = longitude2 / 180 * Math.PI;

            var b12 = Math.Sin(f4);
            var b13 = Math.Sin(f5);
            var b15 = Math.Cos(f4);
            var b16 = Math.Cos(f5);
            var b21 = Math.Cos(f8);
            var b22 = Math.Cos(f9);
            var e21 = Math.Cos(f9 - f8);
            var e22 = Math.Sin(f9 - f8);
            var b24 = Math.Sin(f6 / 2);

            var b25 = Math.Pow(b24, 2);
            var b31 = radioPolar * (1 + (1 / aPlana) * b25);
            var b32 = Math.Pow((b16 * e21 - b15), 2);
            var b33 = Math.Pow((b16 * e22), 2);
            var jj = (1 - 2 * (1 / aPlana)) * (b13 - b12);
            var b34 = Math.Pow(jj, 2);
            var b35 = Math.Sqrt(b32 + b33 + b34);
            var b36 = b31 * b35;
            var b37 = Math.Pow(b36, 3);

            var kmsDist = (b36 + b37 / (9.77 * (Math.Pow(10, 14)))) / 1000;

            return Math.Round(kmsDist, 1);
        }

        public static byte[] GetFileBinaryContent(string filePath, string cacheTimeoutKey = "CacheFileTimeout")
        {
            throw new NotImplementedException();

            //if (string.IsNullOrWhiteSpace(filePath))
            //    throw new Exception();

            //if (string.IsNullOrWhiteSpace(cacheTimeoutKey))
            //    throw new Exception();

            //var path = (
            //    HttpContext.Current == null
            //        ? filePath
            //        : HttpContext.Current.Server.MapPath(filePath)
            //);

            //if (!File.Exists(path))
            //    throw new FileNotFoundException(path);

            //var ext = Path.GetExtension(path).Replace(".", string.Empty).ToUpper();
            //if (string.IsNullOrWhiteSpace(ext))
            //    ext = "FILE";

            //var cacheKey = string.Concat(ext, ".", Regex.Replace(filePath, "[^A-Z]+", ".", RegexOptions.IgnoreCase).ToUpper());

            //return cacheKey.FromCache(
            //    () => File.ReadAllBytes(path),
            //    cacheTimeoutKey,
            //    false
            //);
        }

        public static string GetFileTextContent(string filePath, string cacheTimeoutKey = "CacheFileTimeout", Encoding encoding = null)
        {
            throw new NotImplementedException();

            //if (string.IsNullOrWhiteSpace(filePath))
            //    throw new Exception();

            //if (string.IsNullOrWhiteSpace(cacheTimeoutKey))
            //    throw new Exception();

            //var path = (
            //    HttpContext.Current == null
            //        ? filePath
            //        : HttpContext.Current.Server.MapPath(filePath)
            //);

            //if (!File.Exists(path))
            //    throw new FileNotFoundException(path);

            //var ext = Path.GetExtension(path).Replace(".", string.Empty).ToUpper();
            //if (string.IsNullOrWhiteSpace(ext))
            //    ext = "FILE";

            //var cacheKey = string.Concat(ext, ".", Regex.Replace(filePath, "[^A-Z]+", ".", RegexOptions.IgnoreCase).ToUpper());

            //return cacheKey.FromCache(
            //    () =>
            //    {
            //        if (encoding == null)
            //            return File.ReadAllText(path);
            //        else
            //            return File.ReadAllText(path, encoding);
            //    },
            //    cacheTimeoutKey,
            //    false
            //);
        }

        public static string GetCurrentPath(string append = "")
        {
            throw new NotImplementedException();

            //if (HttpContext.Current == null)
            //    return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), append);
            //else
            //    return HttpContext.Current.Server.MapPath("." + (string.IsNullOrWhiteSpace(append) ? string.Empty : "\\" + append));
        }

        public static string ResolvePath(string path)
        {
            throw new NotImplementedException();

            //if (string.IsNullOrWhiteSpace(path))
            //    return string.Empty;

            //if (HttpContext.Current == null)
            //    return Path.Combine(
            //        AppDomain.CurrentDomain.BaseDirectory,
            //        Path.GetDirectoryName(path.Replace("~/", "")),
            //        Path.GetFileName(path)
            //    );
            //else
            //    return HttpContext.Current.Server.MapPath(path);
        }

        public static bool IsValidEmail(string strIn)
        {
            if (String.IsNullOrEmpty(strIn))
            {
                return false;
            }

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private static string DomainMapper(Match match)
        {
            var idn = new IdnMapping();
            var domainName = match.Groups[2].Value;
            domainName = idn.GetAscii(domainName);

            return match.Groups[1].Value + domainName;
        }

        public static string MakeRequest(string url)
        {
            try
            {
                var client = new HttpClient();
                var response = client.GetStringAsync(new Uri(url)).GetAwaiter().GetResult();

                return response;
            }
            catch (WebException ex)
            {
                var response = string.Empty;

                if (ex.Status != WebExceptionStatus.Success)
                {
                    response = "El WebService no respondió correctamente: Status = " + ex.Status.ToString() + "(" + (int)ex.Status + ")";
                }

                // Si hay una excepción de tipo WebException, me fijo si el código de estado http es 400 (BadRequest)

                if (ex.Response != null && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.BadRequest)
                {
                    // Si el código de estado es 400 entonces pido el stream de response y leo todo el contenido.
                    // En caso de error, la API REST devuelve un objeto JSON con la estructura { "code": "", "desc": "" } detallando el error de negocio.

                    using var stream = new StreamReader(ex.Response.GetResponseStream());
                    response = stream.ReadToEnd();
                }

                return response;
            }
        }

        public static void RetryWhenFail(Func<int, bool> action, Exception throwWhenOut = null, int retries = 3)
        {
            if (action == null)
            {
                return;
            }

            var @try = 1;

            //OMI: Corrección: Cuando en el ultimo intento era el valido, quedaba try == 3 en  y de todas formas ejecutaba el throw
            var @hasError = true;

            while ((@try <= retries) && @hasError)
            {
                if (action.Invoke(@try))
                {
                    hasError = false;
                }

                @try++;
            }

            if ((@hasError) && (throwWhenOut != null))
            {
                throw throwWhenOut;
            }
        }

        public static bool IsBase64String(string value)
        {
            //
            // http://stackoverflow.com/questions/3355407/validate-string-is-base64-format-using-regex
            //

            var base64Characters = new HashSet<char>() {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
                'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
                'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
                'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/',
                '='
            };

            value = value
                .Replace("\r", String.Empty)
                .Replace("\n", String.Empty);

            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            else if (value.Length == 0 || value.Length % 4 != 0)
            {
                return false;
            }
            else if (value.Any(c => !base64Characters.Contains(c)))
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static async Task SendMail(Message mail)
        {
            #region Validations

            if (mail == null)
            {
                throw new ArgumentNullException(nameof(mail));
            }

            if (string.IsNullOrWhiteSpace(mail.From))
            {
                throw new ArgumentNullException(nameof(mail));
            }

            if (!Utilities.IsValidEmail(mail.From))
            {
                throw new EmailFormatException("Email From");
            }

            if (mail.To == null)
            {
                throw new ArgumentNullException(nameof(mail));
            }

            if (mail.To.Count == 0)
            {
                throw new ArgumentNullException(nameof(mail));
            }

            if (string.IsNullOrWhiteSpace(mail.To.First()))
            {
                throw new ArgumentNullException(nameof(mail));
            }

            var to = mail.To
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .ToList();

            if (to.Any(email => !Utilities.IsValidEmail(email)))
            {
                throw new EmailFormatException("Email To");
            }

            if (!string.IsNullOrWhiteSpace(mail.ReplayTo) && !Utilities.IsValidEmail(mail.ReplayTo))
            {
                throw new EmailFormatException("Email ReplyTo");
            }

            #endregion


            var message = new MailMessage
            {
                From = new MailAddress(mail.From),
                Subject = mail.Subject,
                Body = mail.Body,
                IsBodyHtml = mail.IsBodyHtml
            };

            message.To.Add(string.Join(", ", to));

            if (!string.IsNullOrWhiteSpace(mail.ReplayTo))
            {
                message.ReplyToList.Add(mail.ReplayTo);
            }


            #region Adding linked resources

            var mediaType = (mail.IsBodyHtml ? "text/html" : "text/plain");

            if (mail.LinkedResources != null && mail.LinkedResources.Count > 0)
            {
                var view = AlternateView.CreateAlternateViewFromString(mail.Body, null, mediaType);

                mail.LinkedResources.ForEach(r =>
                {
                    var lr = new LinkedResource(r.Stream) { ContentId = r.ContentId };
                    view.LinkedResources.Add(lr);
                });

                message.AlternateViews.Add(view);
            }

            #endregion


            #region Adding attachments

            if (mail.Attachments != null && mail.Attachments.Count > 0)
            {
                mail.Attachments.ForEach(attach =>
                    message.Attachments.Add(new System.Net.Mail.Attachment(attach.Stream, attach.Name))
                );
            }

            #endregion


            #region SMTP Sending

            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, policyErrors) => true; ;

            using var client = new SmtpClient
            {
                Host = "SMTPHost".FromAppSettings(string.Empty, true),
                Port = "SMTPPort".FromAppSettings(-1, true),
                Credentials = new NetworkCredential(
                    "SMTPUser".FromAppSettings(string.Empty, true),
                    "SMTPPassword".FromAppSettings(string.Empty, true)
                ),
                EnableSsl = "SMTPEnableSSL".FromAppSettings(false),
                Timeout = "SMTPTimeout".FromAppSettings(30000)
            };

            if (mail.Async)
            {
                await client.SendMailAsync(message);
            }
            else
            {
                client.Send(message);
            }

            #endregion
        }

        public static bool IsNullableType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static string GetAssemblyVersion(string dllName = null, string format = "v[@Major].[@Minor] (Build [@Build]:[@Revision])")
        {
            string version = format
                .Replace("[@Major]", "x")
                .Replace("[@Minor]", "x")
                .Replace("[@Build]", "x")
                .Replace("[@Revision]", "x");

            string path = null;

            AssemblyName assembly = null;
            FileVersionInfo fvi = null;

            if (string.IsNullOrWhiteSpace(dllName))
            {
                assembly = Assembly.GetExecutingAssembly()?.GetName();
            }
            else
            {
                //Puede que haya que usar GetCallingAssembly() si da error en el path
                path = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath);

                if (!dllName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                {
                    dllName += ".dll";
                }

                Assembly assm = Assembly.LoadFrom(Path.Combine(path, dllName));

                if (assm != null)
                {
                    assembly = assm.GetName();
                    fvi = FileVersionInfo.GetVersionInfo(assm.Location);
                }
            }

            if (assembly != null)
            {
                version = format
                    .Replace("[@Major]", assembly.Version.Major.ToString())
                    .Replace("[@Minor]", assembly.Version.Minor.ToString())
                    .Replace("[@Build]", fvi.FileBuildPart.ToString())
                    .Replace("[@Revision]", fvi.FilePrivatePart.ToString());
            }

            return version;
        }

        public static IEnumerable<T> GetEnumValues<T>()
        {
            if (typeof(T).BaseType != typeof(Enum))
            {
                throw new ArgumentException("T Debe ser de tipo Enum");
            }

            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Permite copiar las propiedades de dos clases (Tipos primitivos)
        /// </summary>
        /// <typeparam name="D">Tipo de la Clase Destino</typeparam>
        /// <typeparam name="T">Tipo de la Clase Origen</typeparam>
        /// <param name="from">Objeto a copiar</param>
        /// <returns></returns>
        public static D Copy<D, T>(T from)
            where T : class
            where D : class, new()
        {
            D to = new();

            PropertyInfo[] toProps = to.GetType().GetProperties();
            PropertyInfo[] fromProps = from.GetType().GetProperties();

            foreach (PropertyInfo pInfo in toProps)
            {
                PropertyInfo pI = fromProps.FirstOrDefault(f => f.Name == pInfo.Name);
                if (pI != null)
                {
                    pInfo.SetValue(to, pI.GetValue(from));
                }
            }

            return to;
        }

        /// <summary>
        /// Permite copiar las propiedades de dos clases y pasa a Mayuscula
        /// </summary>
        /// <typeparam name="D">Tipo de la Clase Destino</typeparam>
        /// <typeparam name="T">Tipo de la Clase Origen</typeparam>
        /// <param name="from">Objeto a copiar</param>
        /// <returns></returns>
        public static D Copy<D, T>(T from, bool upperCaseText = false)
            where T : class
            where D : class, new()
        {
            D to = new();

            PropertyInfo[] toProps = to.GetType().GetProperties();
            PropertyInfo[] fromProps = from.GetType().GetProperties();

            foreach (PropertyInfo pInfo in toProps)
            {
                PropertyInfo pI = fromProps.FirstOrDefault(f => f.Name == pInfo.Name);

                if (pI != null)
                {

                    object value = pI.GetValue(from, null);

                    if (pI.PropertyType == typeof(string))
                    {


                        if (upperCaseText && value != null)
                        {
                            value = ((string)value).ToUpper();
                        }
                    }

                    pInfo.SetValue(to, value);

                }

            }

            return to;
        }

        /// <summary>
        /// Clonado de objetos, soporta nulleables
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static D Clone<D, T>(T entity)
            where T : class
            where D : class, new()
        {
            D retEnt = new();

            //Solo las propiedades de retorno que difieren de las originales
            PropertyInfo[] retProps = typeof(D).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (PropertyInfo propiedad in entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                try
                {
                    object value = propiedad.GetValue(entity, null);

                    PropertyInfo retProp = retProps.FirstOrDefault(f => f.Name == propiedad.Name);

                    if (retProp != null && retProp.CanWrite) //Existe en la clase heredada
                    {
                        var targetType = retProp.PropertyType.IsNullableType() ? Nullable.GetUnderlyingType(retProp.PropertyType) : retProp.PropertyType;
                        var convertedValue = Convert.ChangeType(value, targetType);
                        retProp.SetValue(retEnt, convertedValue, null);
                    }
                    else
                    {
                        if (propiedad.CanWrite)
                        {
                            propiedad.SetValue(retEnt, propiedad.GetValue(entity, null), null);
                        }
                    }
                }
                catch { }
            }
            return retEnt;
        }

        public static string UrlCombine(string urlBase, params string[] queryString)
        {
            if (urlBase == null)
            {
                throw new ArgumentNullException(nameof(urlBase));
            }

            if (queryString == null || queryString.Length == 0)
            {
                return urlBase;
            }

            var protocol = Regex.Match(urlBase, @"[a-zA-Z].*:\/\/").Value;

            if (!protocol.IsNull())
            {
                urlBase = urlBase.Replace(protocol, string.Empty);
                protocol = protocol.Replace("/", "\\");
            }

            urlBase = Regex.Replace(urlBase, @"(\/)\1{0,}", @"\");

            if (!urlBase.Equals(@"\"))
            {
                urlBase = Regex.Replace(urlBase, @"^\\|\\$", string.Empty);
            }

            var query = string.Empty;
            var aux = string.Empty;

            foreach (var qs in queryString)
            {
                aux = Regex.Replace(qs, @"(\/)\1{0,}", @"\");
                aux = Regex.Replace(aux, @"^\\|\\$", string.Empty);
                query = Path.Combine(query, aux);
            }

            var url = Path.Combine(protocol, Path.Combine(urlBase, query));

            return url.Replace("\\", "/");
        }

        public static List<T> AddEx<T>(List<T> input, T value)
        {
            input.Add(value);
            return input;
        }

        public static string CleanCRLF(string input)
        {
            return input
            .Replace("\r\n", "[@NewLine@]")
            .Replace("\r", "[@NewLine@]")
            .Replace("\n", "[@NewLine@]")
            .Replace("\\", "[@NewLine@]")
            .Replace("\"", "[@NewLine@]");
        }

        public static bool EsDesarrollo()
        {
            var stage = "stage".FromAppSettings(string.Empty);
            return stage.IsNull() || stage.Equals("desarrollo", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool EsTesting()
        {
            return "stage".FromAppSettings(string.Empty).Equals("testing", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool EsProduccion()
        {
            return "stage".FromAppSettings(string.Empty).Equals("produccion", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool NoEsProduccion()
        {
            var stage = "stage".FromAppSettings(string.Empty);
            return !stage.IsNull() && !stage.Equals("produccion", StringComparison.InvariantCultureIgnoreCase);
        }

        public static string Ambiente()
        {
            var stage = "stage".FromAppSettings(string.Empty).ToLower();

            switch (stage)
            {
                case "produccion":
                    return "Producción";

                case "testing":
                    return "Testing";

                default:
                    return "Desarrollo";
            }
        }

        public static string GeneradorUnicoID(string valor)
        {
            return string.Format("{0}_{1:N}", valor, Guid.NewGuid());
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

        public static string ResolveMultiplePath(params string[] paths)
        {
            string finalDir = string.Empty;

            if (paths.Length == 0)
            {
                return finalDir;
            }

            if (paths.Length == 1)
            {
                finalDir = ResolvePath(paths[0]);
            }
            else
            {
                finalDir = ResolvePath(paths[0]);
                for (int i = 1; i < paths.Length; i++)
                {
                    finalDir = Path.Combine(finalDir, paths[i]);
                }
            }

            if (!Directory.Exists(finalDir))
            {
                Directory.CreateDirectory(finalDir);
            }

            return finalDir;
        }

        public static Dictionary<object, string> EnumToDictionary<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>()
                .ToDictionary(k => (object)Convert.ToInt32(k), v => v.Description());
        }

        /// <summary>
        /// Get the Description from the DescriptionAttribute.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumValue)
        {
            return enumValue.GetType()
                       .GetMember(enumValue.ToString())
                       .First()
                       .GetCustomAttribute<DescriptionAttribute>()?
                       .Description ?? string.Empty;
        }

        public static string NumeroALetras(double value)
        {
            string num2Text; value = Math.Truncate(value);
            if (value == 0) num2Text = "CERO";
            else if (value == 1) num2Text = "UNO";
            else if (value == 2) num2Text = "DOS";
            else if (value == 3) num2Text = "TRES";
            else if (value == 4) num2Text = "CUATRO";
            else if (value == 5) num2Text = "CINCO";
            else if (value == 6) num2Text = "SEIS";
            else if (value == 7) num2Text = "SIETE";
            else if (value == 8) num2Text = "OCHO";
            else if (value == 9) num2Text = "NUEVE";
            else if (value == 10) num2Text = "DIEZ";
            else if (value == 11) num2Text = "ONCE";
            else if (value == 12) num2Text = "DOCE";
            else if (value == 13) num2Text = "TRECE";
            else if (value == 14) num2Text = "CATORCE";
            else if (value == 15) num2Text = "QUINCE";
            else if (value < 20) num2Text = "DIECI" + NumeroALetras(value - 10);
            else if (value == 20) num2Text = "VEINTE";
            else if (value < 30) num2Text = "VEINTI" + NumeroALetras(value - 20);
            else if (value == 30) num2Text = "TREINTA";
            else if (value == 40) num2Text = "CUARENTA";
            else if (value == 50) num2Text = "CINCUENTA";
            else if (value == 60) num2Text = "SESENTA";
            else if (value == 70) num2Text = "SETENTA";
            else if (value == 80) num2Text = "OCHENTA";
            else if (value == 90) num2Text = "NOVENTA";
            else if (value < 100) num2Text = NumeroALetras(Math.Truncate(value / 10) * 10) + " Y " + NumeroALetras(value % 10);
            else if (value == 100) num2Text = "CIEN";
            else if (value < 200) num2Text = "CIENTO " + NumeroALetras(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) num2Text = NumeroALetras(Math.Truncate(value / 100)) + "CIENTOS";
            else if (value == 500) num2Text = "QUINIENTOS";
            else if (value == 700) num2Text = "SETECIENTOS";
            else if (value == 900) num2Text = "NOVECIENTOS";
            else if (value < 1000) num2Text = NumeroALetras(Math.Truncate(value / 100) * 100) + " " + NumeroALetras(value % 100);
            else if (value == 1000) num2Text = "MIL";
            else if (value < 2000) num2Text = "MIL " + NumeroALetras(value % 1000);
            else if (value < 1000000)
            {
                num2Text = NumeroALetras(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0)
                {
                    num2Text = num2Text + " " + NumeroALetras(value % 1000);
                }
            }
            else if (value == 1000000)
            {
                num2Text = "UN MILLON";
            }
            else if (value < 2000000)
            {
                num2Text = "UN MILLON " + NumeroALetras(value % 1000000);
            }
            else if (value < 1000000000000)
            {
                num2Text = NumeroALetras(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0)
                {
                    num2Text = num2Text + " " + NumeroALetras(value - Math.Truncate(value / 1000000) * 1000000);
                }
            }
            else if (value == 1000000000000) num2Text = "UN BILLON";
            else if (value < 2000000000000) num2Text = "UN BILLON " + NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            else
            {
                num2Text = NumeroALetras(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0)
                {
                    num2Text = num2Text + " " + NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
                }
            }
            return num2Text;
        }
    }
}
