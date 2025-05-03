using LizardCode.Framework.ReneBusClient.Responses;
using LizardCode.Framework.ReneBusClient.Responses.Training;

namespace LizardCode.Framework.ReneBusClient.Interfaces.Training
{
    public interface ITrainingApiQueue
    {
        Task<ReneBusResponse<QueueEnqueueResponse>?> CancelTraining(int idRequirement, string? token);
        Task<ReneBusResponse<QueueEnqueueResponse>?> Enqueue(int idWorkspace, string? token);
        Task<ReneBusResponse<QueueGetOneQueueResponse>?> GetOneQueue(int idRequest, string? token);
    }
}