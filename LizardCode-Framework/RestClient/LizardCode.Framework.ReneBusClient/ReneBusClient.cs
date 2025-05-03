using LizardCode.Framework.Helpers.Utilities;
using LizardCode.Framework.ReneBusClient.Interfaces;
using LizardCode.Framework.ReneBusClient.Oauth;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Authenticators;

namespace LizardCode.Framework.ReneBusClient
{
    public class ReneBusClient : IReneBusClient
    {
        private readonly ILogger _logger;
        private readonly string _authTokenUrl;
        private readonly string _authClientId;
        private readonly string _authSecret;
        private TokenResponse? _tokenResponse;


        internal ReneBusClient(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().ToString());
            _authTokenUrl = "ReneBus:Auth:TokenUrl".FromAppSettings<string>(notFoundException: true);
            _authClientId = "ReneBus:Auth:ClientId".FromAppSettings<string>(notFoundException: true);
            _authSecret = "ReneBus:Auth:ClientSecret".FromAppSettings<string>(notFoundException: true);
            _tokenResponse = null;
        }


        public async Task<TokenResponse> Authenticate()
        {
            _logger.LogInformation("ReneBusClient.Authenticate()");

            if (_tokenResponse != null && _tokenResponse.Expires.AddMinutes(-5) > DateTime.Now)
                return _tokenResponse;

            (var client, var request) = BuildClient(_authTokenUrl, null, null, _authClientId, _authSecret, Method.Post);

            request.AddParameter(OauthParameterNames.GrantType, OauthFlows.ClientCredentials);
            request.AddParameter(OauthParameterNames.Scope, "prediction training");

            var response = await client.ExecuteAsync<TokenResponse>(request);

            if (!response.IsSuccessful)
                throw new Exception("ReneBusClient Error");

            _tokenResponse = response.Data!;
            _tokenResponse.Issued = DateTime.Now;
            _tokenResponse.Expires = DateTime.Now.AddSeconds(_tokenResponse.Expires_In);

            return response.Data!;
        }


        public static (RestClient, RestRequest) BuildClient(string baseUrl, string? resource = null, string? token = null, string authClientId = null, string authSecret = null, Method method = Method.Get)
        {
            var options = new RestClientOptions(baseUrl);
            
            if (authClientId.IsNotNull() && authSecret.IsNotNull())
                options.Authenticator = new HttpBasicAuthenticator(authClientId, authSecret);

            var client = new RestClient(options);
            var request = new RestRequest(resource, method);

            if (token.IsNotNull())
                request.AddHeader(HeaderNames.Authorization, $"Bearer {token}");

            return (client, request);
        }
    }
}
