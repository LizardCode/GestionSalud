namespace LizardCode.Framework.ReneBusClient.Responses.Training
{
    public class QueueStatisticsResponse
    {
        public List<QueueEpochResponse> Epochs { get; set; }
        public List<QueueConfussionResponse> Confussion { get; set; }
        public List<QueueAnnotationResponse> Annotations { get; set; }
    }
}