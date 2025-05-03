using LizardCode.Framework.Helpers.Utilities;
using LizardCode.Framework.ReneBusClient.Interfaces.Prediction;
using LizardCode.Framework.ReneBusClient.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using PredictionResponses = LizardCode.Framework.ReneBusClient.Responses.Prediction;

namespace LizardCode.Framework.ReneBusClient.Sections.Prediction
{
    public class PredictionApiService : IPredictionApiService
    {
        private readonly ILogger<PredictionApiService> _logger;
        private readonly string _serviceUrl;


        public PredictionApiService(ILogger<PredictionApiService> logger)
        {
            _logger = logger;
            _serviceUrl = "ReneBus:PredictionUrl".FromAppSettings<string>(notFoundException: true);
        }


        public async Task<(HttpStatusCode, string?, byte[]?)> GetImage(int idImage, string token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"PredictionApiService.GetPredictionImageBinary({idImage})");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Service/GetImage/{idImage}", token: token);
            var response = await client.ExecuteAsync(request);

            return (response.StatusCode, response.ContentType, response.RawBytes);
        }

        public async Task<ReneBusResponse<PredictionResponses.ServiceUploadModelResponse>?> UploadModel(IFormFile binary, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"PredictionApiService.UploadModel([AssetBinary])");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Service/UploadModel", token, method: Method.Post);

            request.AddFile(nameof(binary), binary.OpenReadStream, binary.FileName);

            var response = await client.ExecuteAsync(request, default);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var reneBusResponse = JsonConvert.DeserializeObject<ReneBusResponse<PredictionResponses.ServiceUploadModelResponse>>(response.Content!);
                reneBusResponse!.StatusCode = response.StatusCode;

                return reneBusResponse;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var casted = JsonConvert.DeserializeObject<ReneBusResponse<Dictionary<string, string[]>>>(response.Content!);

                return new()
                {
                    StatusCode = response.StatusCode,
                    Succeeded = false,
                    Error = new()
                    {
                        Code = 0,
                        Message = string.Join(";", casted!.Data!.Select(s => $"{s.Key}: [{string.Join(",", s.Value)}]"))
                    },
                    Data = null
                };
            }

            return null;
        }
    }
}
