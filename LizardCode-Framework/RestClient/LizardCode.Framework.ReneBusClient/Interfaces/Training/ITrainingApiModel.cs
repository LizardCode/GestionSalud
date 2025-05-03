using LizardCode.Framework.ReneBusClient.Responses;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace LizardCode.Framework.ReneBusClient.Interfaces.Training
{
    public interface ITrainingApiModel
    {
        Task<(HttpStatusCode, string?, byte[]?)> Download(int idWorkspace, int idModel, string? token);
        Task<ReneBusResponse<bool?>?> UploadModel(int idWorkspace, int idModel, IFormFile binary, string? token);
    }
}