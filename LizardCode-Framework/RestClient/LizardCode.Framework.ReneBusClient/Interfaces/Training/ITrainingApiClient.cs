using LizardCode.Framework.ReneBusClient.Oauth;

namespace LizardCode.Framework.ReneBusClient.Interfaces.Training
{
    public interface ITrainingApiClient
    {
        ITrainingApiQueue Queue { get; }
        ITrainingApiRepository Repository { get; }
        ITrainingApiWorkspace Workspace { get; }

        Task<TokenResponse> Authenticate();
    }
}