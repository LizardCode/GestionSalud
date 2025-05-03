using LizardCode.Framework.Helpers.Utilities;
using LizardCode.Framework.ReneBusClient.Interfaces.Training;
using LizardCode.Framework.ReneBusClient.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using TrainingResponses = LizardCode.Framework.ReneBusClient.Responses.Training;

namespace LizardCode.Framework.ReneBusClient.Sections.Training
{
    public class TrainingApiQueue : ITrainingApiQueue
    {
        private readonly ILogger<TrainingApiQueue> _logger;
        private readonly string _serviceUrl;


        public TrainingApiQueue(ILogger<TrainingApiQueue> logger)
        {
            _logger = logger;
            _serviceUrl = "ReneBus:TrainingUrl".FromAppSettings<string>(notFoundException: true);
        }


        public async Task<ReneBusResponse<TrainingResponses.QueueGetOneQueueResponse>?> GetOneQueue(int idRequest, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"TrainingApiQueue.GetOneQueue({idRequest})");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Queue/GetOneQueue/{idRequest}", token: token);
            var response = await client.ExecuteAsync<ReneBusResponse<TrainingResponses.QueueGetOneQueueResponse>>(request, default);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                response.Data!.StatusCode = response.StatusCode;
                return response.Data;
            }

            return null;
        }

        public async Task<ReneBusResponse<TrainingResponses.QueueEnqueueResponse>?> Enqueue(int idWorkspace, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"TrainingApiQueue.Enqueue({idWorkspace})");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Queue/Enqueue", token, method: Method.Post);

            request.AddParameter(nameof(idWorkspace), idWorkspace);

            var response = await client.ExecuteAsync(request, default);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var reneBusResponse = JsonConvert.DeserializeObject<ReneBusResponse<TrainingResponses.QueueEnqueueResponse>>(response.Content!);
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

        public async Task<ReneBusResponse<TrainingResponses.QueueEnqueueResponse>?> CancelTraining(int idRequirement, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"TrainingApiQueue.CancelTraining({idRequirement})");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Queue/CancelTraining", token, method: Method.Post);

            request.AddParameter(nameof(idRequirement), idRequirement);

            var response = await client.ExecuteAsync(request, default);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var reneBusResponse = JsonConvert.DeserializeObject<ReneBusResponse<TrainingResponses.QueueEnqueueResponse>>(response.Content!);
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
