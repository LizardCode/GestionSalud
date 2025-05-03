using Microsoft.AspNetCore.Http;

namespace LizardCode.Framework.ReneBusClient.Requests.Prediction
{
    public class QueueMultiEnqueueRequest
    {
        public IFormFile Asset { get; set; }
        public int[] IdModels { get; set; }
    }
}
