using LizardCode.Framework.ReneBusClient.Oauth;

namespace LizardCode.Framework.ReneBusClient.Interfaces
{
    public interface IReneBusClient
    {
        Task<TokenResponse> Authenticate();
    }
}