using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Helpers.WHApi
{
    public interface IWHApiHelper
    {
        Task<WHApiResponse> SendMessage(string phone, string message, CancellationToken cancellationToken = default);
    }

    public class WHApiResponse
    {
        public bool sent { get; set; }
        public Message message { get; set; }
    }

    public class Message
    {
        public string id { get; set; }
        public bool from_me { get; set; }
        public string type { get; set; }
        public string chat_id { get; set; }
        public int timestamp { get; set; }
        public string source { get; set; }
        public int device_id { get; set; }
        public string status { get; set; }
        public Text text { get; set; }
        public string from { get; set; }
    }

    public class Text
    {
        public string body { get; set; }
    }

}
