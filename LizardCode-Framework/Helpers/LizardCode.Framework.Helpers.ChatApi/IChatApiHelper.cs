using LizardCode.Framework.Helpers.ChatApi.Responses;

namespace LizardCode.Framework.Helpers.ChatApi
{
    public interface IChatApiHelper
    {
        Task<SendMessageResponse> SendMessage(string phone, string message, CancellationToken cancellationToken = default);
    }
}