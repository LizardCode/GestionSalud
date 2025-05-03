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
    public class PredictionApiQueue : IPredictionApiQueue
    {
        private readonly ILogger<PredictionApiQueue> _logger;
        private readonly string _serviceUrl;


        public PredictionApiQueue(ILogger<PredictionApiQueue> logger)
        {
            _logger = logger;
            _serviceUrl = "ReneBus:PredictionUrl".FromAppSettings<string>(notFoundException: true);
        }

        public async Task<ReneBusResponse<PredictionResponses.QueueResultResponse>?> GetResult(int idQueue, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"PredictionApiQueue.GetResult({idQueue})");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Queue/GetResult/{idQueue}", token: token);
            var response = await client.ExecuteAsync<ReneBusResponse<PredictionResponses.QueueResultResponse>>(request, default);

            if (response.StatusCode == HttpStatusCode.NoContent)
                return null;

            response.Data!.StatusCode = response.StatusCode;

            return response.Data;
        }

        public async Task<ReneBusResponse<PredictionResponses.QueueEnqueueResponse>?> Enqueue(PredictionRequests.QueueEnqueueRequest command, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"PredictionApiQueue.Enqueue({command.IdModel}, [AssetBinary])");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Queue/Enqueue", token, method: Method.Post);

            request.AddFile(nameof(command.Asset), command.Asset.OpenReadStream, command.Asset.FileName);
            request.AddParameter(nameof(command.IdModel), command.IdModel);

            var response = await client.ExecuteAsync(request, default);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var reneBusResponse = JsonConvert.DeserializeObject<ReneBusResponse<PredictionResponses.QueueEnqueueResponse>>(response.Content!);
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
