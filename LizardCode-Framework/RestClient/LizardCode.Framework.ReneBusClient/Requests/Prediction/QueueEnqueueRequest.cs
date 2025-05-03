using Microsoft.AspNetCore.Http;

namespace LizardCode.Framework.ReneBusClient.Requests.Prediction
{
    public class QueueEnqueueRequest
    {
        public IFormFile Asset { get; set; }
        public int IdModel { get; set; }
    }
}
