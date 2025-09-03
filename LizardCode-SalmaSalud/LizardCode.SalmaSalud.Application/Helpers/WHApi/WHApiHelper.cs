using LizardCode.Framework.Helpers.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Helpers.WHApi
{
    internal class WHApiHelper : IWHApiHelper
    {
        private readonly string _baseUrl;
        private readonly string _token;

        public WHApiHelper()
        {
            _baseUrl = "WHApi:Url".FromAppSettings<string>(notFoundException: true);
            _token = "WHApi:Token".FromAppSettings<string>(notFoundException: true);
        }

        public async Task<WHApiResponse> SendMessage(string to, string body, CancellationToken cancellationToken = default)
        {
            var rMessage = new WHApiResponse();
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
                        $"{{\"to\": \"549{to}\", \"body\": \"{body}\"}}",
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
                    rMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<WHApiResponse>(rString);
                }
            }

            return rMessage;
        }
    }
}
