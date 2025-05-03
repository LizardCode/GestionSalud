using LizardCode.Framework.Helpers.Utilities;
using LizardCode.Framework.ReneBusClient.Interfaces.Prediction;
using LizardCode.Framework.ReneBusClient.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using PredictionRequests = LizardCode.Framework.ReneBusClient.Requests.Prediction;
using PredictionResponses = LizardCode.Framework.ReneBusClient.Responses.Prediction;

namespace LizardCode.Framework.ReneBusClient.Sections.Prediction
{
    public class PredictionApiQueueMulti : IPredictionApiQueueMulti
    {
        private readonly ILogger<PredictionApiQueue> _logger;
        private readonly string _serviceUrl;


        public PredictionApiQueueMulti(ILogger<PredictionApiQueue> logger)
        {
            _logger = logger;
            _serviceUrl = "ReneBus:PredictionUrl".FromAppSettings<string>(notFoundException: true);
        }

        public async Task<ReneBusResponse<List<PredictionResponses.QueueMultiResultResponse>>?> GetResults(int idQueue, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"PredictionApiQueueMulti.GetResults({idQueue})");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"QueueMulti/GetResults/{idQueue}", token: token);
            var response = await client.ExecuteAsync<ReneBusResponse<List<PredictionResponses.QueueMultiResultResponse>>>(request, default);

            if (response.StatusCode == HttpStatusCode.NoContent)
                return null;

            if (response.Data != null)
                response.Data!.StatusCode = response.StatusCode;

            return response.Data;
        }

        public async Task<ReneBusResponse<PredictionResponses.QueueMultiEnqueueResponse>?> Enqueue(PredictionRequests.QueueMultiEnqueueRequest command, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"PredictionApiQueueMulti.Enqueue([{string.Join(",", command.IdModels)}], [AssetBinary])");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"QueueMulti/Enqueue", token, method: Method.Post);

            request.AddFile(nameof(command.Asset), command.Asset.OpenReadStream, command.Asset.FileName);

            Array.ForEach(command.IdModels, id =>
                request.AddParameter(nameof(command.IdModels), id)
            );

            var response = await client.ExecuteAsync(request, default);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var reneBusResponse = JsonConvert.DeserializeObject<ReneBusResponse<PredictionResponses.QueueMultiEnqueueResponse>>(response.Content!);
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
