namespace LizardCode.Framework.ReneBusClient.Responses.Training
{
    public class QueueGetOneQueueResponse
    {
        public int IdRequest { get; set; }
        public DateTime Date { get; set; }
        public int IdStatus { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime StartDateEta { get; set; }
        public TimeSpan? ProcessingTime { get; set; }
        public TimeSpan ProcessingTimeEta { get; set; }
        public DateTime? EndDate { get; set; }

        public int? IdResult { get; set; }

        public QueueModelResponse Model { get; set; }
        public QueueStatisticsResponse Statistics { get; set; }
    }
}
