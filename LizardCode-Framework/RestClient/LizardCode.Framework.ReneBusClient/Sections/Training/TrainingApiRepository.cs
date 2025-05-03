using LizardCode.Framework.Helpers.Utilities;
using LizardCode.Framework.ReneBusClient.Interfaces.Training;
using LizardCode.Framework.ReneBusClient.Responses;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Net;
using TrainingResponses = LizardCode.Framework.ReneBusClient.Responses.Training;

namespace LizardCode.Framework.ReneBusClient.Sections.Training
{
    public class TrainingApiRepository : ITrainingApiRepository
    {
        private readonly ILogger<TrainingApiRepository> _logger;
        private readonly string _serviceUrl;


        public TrainingApiRepository(ILogger<TrainingApiRepository> logger)
        {
            _logger = logger;
            _serviceUrl = "ReneBus:TrainingUrl".FromAppSettings<string>(notFoundException: true);
        }


        public async Task<ReneBusResponse<TrainingResponses.RepositoryGetByRequirementResponse>?> GetByRequirement(int idRequirement, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"TrainingApiRepository.GetByRequirement({idRequirement})");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Repository/GetByRequirement/{idRequirement}", token: token);
            var response = await client.ExecuteAsync<ReneBusResponse<TrainingResponses.RepositoryGetByRequirementResponse>>(request, default);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                response.Data!.StatusCode = response.StatusCode;
                return response.Data;
            }

            return null;
        }

        public async Task<ReneBusResponse<List<TrainingResponses.RepositoryGetAnnotationByRepositoryResponse>>?> GetAnnotationsByRepository(int idRepository, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"TrainingApiRepository.GetAnnotationsByRepository({idRepository})");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Repository/GetAnnotationsByRepository/{idRepository}", token: token);
            var response = await client.ExecuteAsync<ReneBusResponse<List<TrainingResponses.RepositoryGetAnnotationByRepositoryResponse>>>(request, default);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                response.Data!.StatusCode = response.StatusCode;
                return response.Data;
            }

            return null;
        }

        public async Task<ReneBusResponse<List<TrainingResponses.RepositoryGetImagesByRepositoryResponse>>?> GetImagesByRepository(int idRepository, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"TrainingApiRepository.GetImagesByRepository({idRepository})");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Repository/GetImagesByRepository/{idRepository}", token: token);
            var response = await client.ExecuteAsync<ReneBusResponse<List<TrainingResponses.RepositoryGetImagesByRepositoryResponse>>>(request, default);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                response.Data!.StatusCode = response.StatusCode;
                return response.Data;
            }

            return null;
        }

        public async Task<(HttpStatusCode, string?, byte[]?)> GetImageBinary(int idRepository, int idImage, string? token)
        {
            if (token.IsNull())
                throw new ArgumentNullException(nameof(token));

            _logger.LogInformation($"TrainingApiRepository.GetImageBinary({idRepository},{idImage})");

            (var client, var request) = ReneBusClient.BuildClient(_serviceUrl, $"Repository/GetImageBinary/{idRepository}/{idImage}", token: token);
            var response = await client.ExecuteAsync(request);

            return (response.StatusCode, response.ContentType, response.RawBytes);
        }
    }
}
