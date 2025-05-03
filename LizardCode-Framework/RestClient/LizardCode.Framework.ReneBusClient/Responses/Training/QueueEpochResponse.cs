namespace LizardCode.Framework.ReneBusClient.Responses.Training
{
    public class QueueEpochResponse
    {
        public int Number { get; set; }
        public double Duration { get; set; }
        public double Accuracy { get; set; }
        public double AccuracyValidation { get; set; }
        public double Loss { get; set; }
        public double LossValidation { get; set; }
    }
}