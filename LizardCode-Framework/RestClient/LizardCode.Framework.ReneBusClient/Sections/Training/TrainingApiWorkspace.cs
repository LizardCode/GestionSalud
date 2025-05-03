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
    public class TrainingApiWorkspace : ITrainingApiWorkspace
    {
        private readonly ILogger<TrainingApiWorkspace> _logger;
        private readonly string _serviceUrl;


        public TrainingApiWorkspace(
            ILogger<TrainingApiWorkspace> logger)
        {
            _logger = logger;
            _serviceUrl = "ReneBus:TrainingUrl".FromAppSettings<string>(notFoundException: true);
        }

        public async Task<ReneBusResponse<TrainingResponses.WorkspaceAddItemResponse>?> AddItem(string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"TrainingApiWorkspace.AddItem()");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Workspace/AddItem", token, method: Method.Post);
            var response = await client.ExecuteAsync(request, default);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var reneBusResponse = JsonConvert.DeserializeObject<ReneBusResponse<TrainingResponses.WorkspaceAddItemResponse>>(response.Content!);
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
