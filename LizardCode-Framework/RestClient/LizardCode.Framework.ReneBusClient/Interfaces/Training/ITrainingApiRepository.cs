using LizardCode.Framework.ReneBusClient.Responses;
using LizardCode.Framework.ReneBusClient.Responses.Training;
using System.Net;

namespace LizardCode.Framework.ReneBusClient.Interfaces.Training
{
    public interface ITrainingApiRepository
    {
        Task<ReneBusResponse<List<RepositoryGetAnnotationByRepositoryResponse>>?> GetAnnotationsByRepository(int idRepository, string? token);
        Task<ReneBusResponse<RepositoryGetByRequirementResponse>?> GetByRequirement(int idRequirement, string? token);
        Task<(HttpStatusCode, string?, byte[]?)> GetImageBinary(int idRepository, int idImage, string? token);
        Task<ReneBusResponse<List<RepositoryGetImagesByRepositoryResponse>>?> GetImagesByRepository(int idRepository, string? token);
    }
}