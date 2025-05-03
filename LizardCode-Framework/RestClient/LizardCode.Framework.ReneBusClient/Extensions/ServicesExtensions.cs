using LizardCode.Framework.ReneBusClient.Interfaces.Prediction;
using LizardCode.Framework.ReneBusClient.Interfaces.Training;
using LizardCode.Framework.ReneBusClient.Sections.Prediction;
using LizardCode.Framework.ReneBusClient.Sections.Training;
using Microsoft.Extensions.DependencyInjection;

namespace LizardCode.Framework.ReneBusClient.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddReneBusClient(this IServiceCollection services)
        {
            services.AddTransient<IPredictionApiClient, PredictionApiClient>();
            services.AddTransient<IPredictionApiQueue, PredictionApiQueue>();
            services.AddTransient<IPredictionApiQueueMulti, PredictionApiQueueMulti>();
            services.AddTransient<IPredictionApiService, PredictionApiService>();

            services.AddTransient<ITrainingApiClient, TrainingApiClient>();
            services.AddTransient<ITrainingApiQueue, TrainingApiQueue>();
            services.AddTransient<ITrainingApiRepository, TrainingApiRepository>();
            services.AddTransient<ITrainingApiWorkspace, TrainingApiWorkspace>();
            services.AddTransient<ITrainingApiModel, TrainingApiModel>();

            return services;
        }
    }
}
