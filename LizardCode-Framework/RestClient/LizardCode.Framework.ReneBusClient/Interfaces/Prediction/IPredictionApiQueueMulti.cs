using LizardCode.Framework.ReneBusClient.Requests.Prediction;
using LizardCode.Framework.ReneBusClient.Responses;
using LizardCode.Framework.ReneBusClient.Responses.Prediction;

namespace LizardCode.Framework.ReneBusClient.Interfaces.Prediction
{
    public interface IPredictionApiQueueMulti
    {
        Task<ReneBusResponse<QueueMultiEnqueueResponse>?> Enqueue(QueueMultiEnqueueRequest command, string? token);
        Task<ReneBusResponse<List<QueueMultiResultResponse>>?> GetResults(int idQueue, string? token);
    }
}