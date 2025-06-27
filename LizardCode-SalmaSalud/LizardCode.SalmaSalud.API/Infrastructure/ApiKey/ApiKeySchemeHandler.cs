using LizardCode.Framework.Application.Interfaces.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace LizardCode.SalmaSalud.API.Infrastructure.ApiKey
{
    public class ApiKeySchemeHandler : AuthenticationHandler<ApiKeySchemeOptions>
    {
        private readonly IConfiguration _configuration;

        public ApiKeySchemeHandler(IOptionsMonitor<ApiKeySchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IConfiguration configuration) : base(options, logger, encoder)
        {
            _configuration = configuration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(Options.HeaderName))
            {
                return AuthenticateResult.Fail("Header Not Found.");
            }

            var headerValue = Request.Headers[Options.HeaderName];

            var apiKey = _configuration.GetValue<string>("ApiKey");                        
            if (apiKey is null)
            {
                return AuthenticateResult.Fail("Api Key not configuratted.");
            }

            if (apiKey != headerValue)
            {
                return AuthenticateResult.Fail("Wrong Api Key.");
            }

            var claims = new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "LizardCode"),
            new Claim(ClaimTypes.Name, "LizardCode")
            };

            var identiy = new ClaimsIdentity(claims, nameof(ApiKeySchemeHandler));
            var principal = new ClaimsPrincipal(identiy);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
