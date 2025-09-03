using LizardCode.Framework.Helpers.Utilities;
using RestSharp;
using System;
using System.Threading;
using System.Threading.Tasks;
using RestSharp.Authenticators.OAuth2;
using MySqlX.XDevAPI;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using NPOI.XWPF.UserModel;

namespace LizardCode.SalmaSalud.Application.Helpers
{
    public class WAppApiHelper : IWAppApiHelper
    {
        private readonly string _baseUrl;
        private readonly string _token;

        public WAppApiHelper()
        {
            _baseUrl = "WAppApi:Url".FromAppSettings<string>(notFoundException: true);
            _token = "WAppApi:Token".FromAppSettings<string>(notFoundException: true);
        }

        public async Task<WAppApiResponse> SendMessage(string chatId, string message, CancellationToken cancellationToken = default)
        {
            var rMessage = new WAppApiResponse();
            using (var httpClient = new HttpClient())
            {
                // Configurar la solicitud
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_baseUrl),
                    Headers =
                    {
                        { "accept", "application/json" },
                        { "authorization", "Bearer " + _token },
                    },
                    Content = new StringContent(
                        $"{{\"chatId\": \"549{chatId}@c.us\", \"message\": \"{message}\"}}",
                        Encoding.UTF8,
                        "application/json")
                };

                // Enviar la solicitud y obtener la respuesta
                using (var response = await httpClient.SendAsync(request))
                {
                    // Asegurar que la solicitud fue exitosa
                    response.EnsureSuccessStatusCode();

                    // Leer y retornar el contenido de la respuesta
                    var rString = await response.Content.ReadAsStringAsync();
                    rMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<WAppApiResponse>(rString);
                }
            }

            return rMessage;
        }
    }
}
