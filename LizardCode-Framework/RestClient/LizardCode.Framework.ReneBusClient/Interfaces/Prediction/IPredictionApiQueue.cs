using LizardCode.Framework.ReneBusClient.Requests.Prediction;
using LizardCode.Framework.ReneBusClient.Responses;
using LizardCode.Framework.ReneBusClient.Responses.Prediction;

namespace LizardCode.Framework.ReneBusClient.Interfaces.Prediction
{
    public interface IPredictionApiQueue
    {
        Task<ReneBusResponse<QueueEnqueueResponse>?> Enqueue(QueueEnqueueRequest command, string? token);
        Task<ReneBusResponse<QueueResultResponse>?> GetResult(int idQueue, string? token);
    }
}