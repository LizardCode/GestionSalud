using LizardCode.Framework.Helpers.Utilities;
using RestSharp;
using RestSharp.Authenticators;

namespace LizardCode.Framework.Helpers.ChatApi
{
    public class ChatApiHelper : IChatApiHelper
    {
        private readonly string _user;
        private readonly string _password;
        private readonly string _baseUrl;


        public ChatApiHelper()
        {
            _user = "ChatApi:User".FromAppSettings<string>(notFoundException: true);
            _password = "ChatApi:Password".FromAppSettings<string>(notFoundException: true);
            _baseUrl = "ChatApi:Url".FromAppSettings<string>(notFoundException: true);
        }


        public async Task<Responses.SendMessageResponse> SendMessage(string phone, string message, CancellationToken cancellationToken = default)
        {
            var client = BuildClient();
            var parameters = new
            {
                message,
                phone
            };

            return await client.PostJsonAsync<object, Responses.SendMessageResponse>("sendMessage", parameters, cancellationToken);
        }


        private RestClient BuildClient()
        {
            var options = new RestClientOptions
            {
                BaseUrl = new Uri(_baseUrl),
                Authenticator = new HttpBasicAuthenticator(_user, _password)
            };

            return new RestClient(options);
        }
    }
}