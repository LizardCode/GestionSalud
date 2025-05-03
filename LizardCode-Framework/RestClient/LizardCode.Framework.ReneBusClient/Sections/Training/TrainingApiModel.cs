using LizardCode.Framework.Helpers.Utilities;
using LizardCode.Framework.ReneBusClient.Interfaces.Training;
using LizardCode.Framework.ReneBusClient.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace LizardCode.Framework.ReneBusClient.Sections.Training
{
    public class TrainingApiModel : ITrainingApiModel
    {
        private readonly ILogger<TrainingApiModel> _logger;
        private readonly string _serviceUrl;


        public TrainingApiModel(ILogger<TrainingApiModel> logger)
        {
            _logger = logger;
            _serviceUrl = "ReneBus:TrainingUrl".FromAppSettings<string>(notFoundException: true);
        }


        public async Task<(HttpStatusCode, string?, byte[]?)> Download(int idWorkspace, int idModel, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"TrainingApiModel.Download({idWorkspace},{idModel})");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Model/Download/{idWorkspace}/{idModel}", token: token);
            var response = await client.ExecuteAsync(request);

            return (response.StatusCode, response.ContentType, response.RawBytes);
        }

        public async Task<ReneBusResponse<bool?>?> UploadModel(int idWorkspace, int idModel, IFormFile binary, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"TrainingApiModel.Upload({idWorkspace},{idModel},[AssetBinary])");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Model/Upload", token, method: Method.Post);

            request.AddParameter(nameof(idWorkspace), idWorkspace);
            request.AddParameter(nameof(idModel), idModel);
            request.AddFile(nameof(binary), binary.OpenReadStream, binary.FileName);

            var response = await client.ExecuteAsync(request, default);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var reneBusResponse = JsonConvert.DeserializeObject<ReneBusResponse<bool?>>(response.Content!);
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
