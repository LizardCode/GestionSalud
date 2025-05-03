namespace LizardCode.Framework.ReneBusClient.Responses.Prediction
{
    public class QueueMultiResultResponse
    {
        public int Id { get; set; }
        public int IdModel { get; set; }
        public string Model { get; set; }
        public DateTime Timestamp { get; set; }
        public int IdState { get; set; }
        public string State { get; set; }
        public List<QueueMultiResultIssueResponse> Issues { get; set; }
    }
}
