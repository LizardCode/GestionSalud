using LizardCode.Framework.ReneBusClient.Interfaces.Prediction;
using Microsoft.Extensions.Logging;

namespace LizardCode.Framework.ReneBusClient
{
    public class PredictionApiClient : ReneBusClient, IPredictionApiClient
    {
        public IPredictionApiQueue Queue { get; }
        public IPredictionApiQueueMulti QueueMulti { get; }
        public IPredictionApiService Service { get; }


        public PredictionApiClient(
            ILoggerFactory loggerFactory,
            IPredictionApiQueue queue,
            IPredictionApiQueueMulti queueMulti,
            IPredictionApiService service) : base(loggerFactory)
        {
            Queue = queue;
            QueueMulti = queueMulti;
            Service = service;
        }
    }
}
