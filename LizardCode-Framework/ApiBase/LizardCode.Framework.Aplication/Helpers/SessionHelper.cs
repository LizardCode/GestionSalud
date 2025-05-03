using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Text;

namespace LizardCode.Framework.Application.Helpers
{
    public static class SessionHelper
    {
        public enum SessionKey
        {
            [Description("DO_CAPTCHA")]
            DO_CAPTCHA
        }

        public static void SetObjectAsString(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static bool DoCaptcha(this ISession session)
        {
            var value = session.GetString(SessionKey.DO_CAPTCHA.Description());

            return !string.IsNullOrEmpty(value) && value != "\"\"";
        }
    }
}
