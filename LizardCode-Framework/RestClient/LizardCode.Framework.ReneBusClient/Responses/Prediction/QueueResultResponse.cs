namespace LizardCode.Framework.ReneBusClient.Responses.Prediction
{
    public class QueueResultResponse
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int IdState { get; set; }
        public string State { get; set; }
        public List<QueueResultIssueResponse> Issues { get; set; }
    }
}
