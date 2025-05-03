using LizardCode.Framework.ReneBusClient.Interfaces.Training;
using Microsoft.Extensions.Logging;

namespace LizardCode.Framework.ReneBusClient
{
    public class TrainingApiClient : ReneBusClient, ITrainingApiClient
    {
        public ITrainingApiQueue Queue { get; }
        public ITrainingApiRepository Repository { get; }
        public ITrainingApiWorkspace Workspace { get; }
        public ITrainingApiModel Model { get; }


        public TrainingApiClient(
            ILoggerFactory loggerFactory,
            ITrainingApiQueue queue,
            ITrainingApiRepository repository,
            ITrainingApiWorkspace workspace,
            ITrainingApiModel model) : base(loggerFactory)
        {
            Queue = queue;
            Repository = repository;
            Workspace = workspace;
            Model = model;
        }
    }
}
