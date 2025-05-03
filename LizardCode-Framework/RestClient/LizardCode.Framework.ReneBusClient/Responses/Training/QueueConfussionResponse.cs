namespace LizardCode.Framework.ReneBusClient.Responses.Training
{
    public class QueueConfussionResponse
    {
        public int Id { get; set; }
        public QueueConfussionItemResponse Row { get; set; }
        public QueueConfussionItemResponse Column { get; set; }
        public double Value { get; set; }
    }
}