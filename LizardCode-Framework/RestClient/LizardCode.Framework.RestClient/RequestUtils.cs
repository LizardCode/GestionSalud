
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LizardCode.Framework.RestClient
{
    public static class RequestUtils
    {
        public static T ValidateHeader<T>(string hdrName, HttpRequestHeaders headers, bool required = true)
        {
            IEnumerable<string> values;
            if (headers.TryGetValues(hdrName, out values))
                return RequestUtils.GetHeader<T>(hdrName, headers);
            if (required)
                throw new HttpRequestException(string.Format("El header {0} es requerido.", (object)hdrName));
            return default(T);
        }

        public static T GetHeader<T>(string hdrName, HttpRequestHeaders headers)
        {
            IEnumerable<string> values;
            headers.TryGetValues(hdrName, out values);
            try
            {
                return (T)Convert.ChangeType((object)values.First<string>(), typeof(T));
            }
            catch (InvalidCastException ex)
            {
                throw new HttpRequestException(string.Format("Formato de header {0} incorrecto. Valor: {1}.", (object)hdrName, (object)values), (Exception)ex);
            }
        }

        public static string GetQueryString(HttpRequestMessage request)
        {
            return request.RequestUri.Query;
        }

        public static string GetHeaders(HttpRequestMessage request)
        {
            IEnumerable<string> source = request.Headers.Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>)(kv => kv.Key + ": " + kv.Value.FirstOrDefault<string>()));
            if (source.Count<string>() > 0)
                return source.Aggregate<string>((Func<string, string, string>)((i, j) => i + ", " + j));
            return "";
        }

        public static string GetUri(HttpRequestMessage request)
        {
            return request.RequestUri.AbsoluteUri;
        }
        public static string GetActivityIDKey()
        {
            return "ActivityID";
        }
    }
}
