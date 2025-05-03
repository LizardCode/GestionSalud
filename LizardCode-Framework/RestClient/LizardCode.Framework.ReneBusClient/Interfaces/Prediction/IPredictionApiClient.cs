using LizardCode.Framework.ReneBusClient.Oauth;

namespace LizardCode.Framework.ReneBusClient.Interfaces.Prediction
{
    public interface IPredictionApiClient
    {
        IPredictionApiQueue Queue { get; }
        IPredictionApiQueueMulti QueueMulti { get; }
        IPredictionApiService Service { get; }

        Task<TokenResponse> Authenticate();
    }
}