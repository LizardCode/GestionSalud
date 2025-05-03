using LizardCode.Framework.ReneBusClient.Responses;
using LizardCode.Framework.ReneBusClient.Responses.Training;

namespace LizardCode.Framework.ReneBusClient.Interfaces.Training
{
    public interface ITrainingApiWorkspace
    {
        Task<ReneBusResponse<WorkspaceAddItemResponse>?> AddItem(string? token);
    }
}