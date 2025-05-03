using LizardCode.Framework.ReneBusClient.Responses;
using LizardCode.Framework.ReneBusClient.Responses.Prediction;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace LizardCode.Framework.ReneBusClient.Interfaces.Prediction
{
    public interface IPredictionApiService
    {
        Task<(HttpStatusCode, string?, byte[]?)> GetImage(int idImage, string token);
        Task<ReneBusResponse<ServiceUploadModelResponse>?> UploadModel(IFormFile binary, string? token);
    }
}