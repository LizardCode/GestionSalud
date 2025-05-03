using LizardCode.Framework.Application.Models.ReCaptcha;
using LizardCode.Framework.Helpers.Utilities;

namespace LizardCode.Framework.Application.Helpers
{
    public static class ReCaptchaHelper
    {
        public static async Task<bool> ValidarCaptchaV2(string captcha)
        {
            var privateKey = "Captcha:PrivateKey".FromAppSettings<string>(notFoundException: true);

            var endpoint = "Captcha:VerifyUrl".FromAppSettings<string>(notFoundException: true);
            endpoint = endpoint.Replace("[SECRET_KEY]", privateKey).Replace("[RESPONSE]", captcha);

            var result = await Get<GenericResponse>(endpoint);

            return result.success;
        }

        private static async Task<T> Get<T>(string endpoint)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(endpoint),
                    Method = new HttpMethod("Get")
                };

                var response = await httpClient.SendAsync(request);

                var jsonString = string.Empty;
                using (HttpContent content = response.Content)
                    jsonString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    T data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);

                    return data;
                }
                else
                {
                    GenericResponse data = Newtonsoft.Json.JsonConvert.DeserializeObject<GenericResponse>(jsonString);
                    throw new Exception(response.StatusCode.ToString());
                }
            }
        }
    }
}