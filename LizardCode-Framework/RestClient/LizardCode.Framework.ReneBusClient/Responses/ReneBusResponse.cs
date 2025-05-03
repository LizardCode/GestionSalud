using System.Net;

namespace LizardCode.Framework.ReneBusClient.Responses
{
    public class ReneBusResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool Succeeded { get; set; }
        public ReneBusErrorResponse? Error { get; set; }
        public T? Data { get; set; }
    }
}
