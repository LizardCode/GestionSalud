using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;

namespace LizardCode.SalmaSalud.API.Infrastructure.ApiKey
{
    public class ApiKeySchemeOptions : AuthenticationSchemeOptions
    {
        public const string Scheme = "ApiKeyScheme";

        /// <summary>
        /// Nombre del Header donde se buscará la API Key
        /// Default: Authorization
        /// </summary>
        /// <value></value>
        public string HeaderName { get; set; } = HeaderNames.Authorization;
    }
}
