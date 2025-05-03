namespace LizardCode.Framework.ReneBusClient.Responses.Training
{
    public class QueueModelResponse
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public double Accuracy { get; set; }
        public double AccuracyTest { get; set; }
        public double LossTest { get; set; }
        public string Url { get; set; }
    }
}