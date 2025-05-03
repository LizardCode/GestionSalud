using Newtonsoft.Json;
using System;
using static LizardCode.Framework.Helpers.SendGrid.Enums;

namespace LizardCode.Framework.Helpers.SendGrid
{
    [JsonObject("messages")]
    public class Messages
    {
        [JsonProperty("from_email")]
        public string FromEmail { get; set; }

        [JsonProperty("msg_id")]
        public string MsgId { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("to_email")]
        public string ToEmail { get; set; }

        [JsonProperty("status")]
        public StatusEmailCode Status { get; set; }

        [JsonProperty("opens_count")]
        public string OpensCount { get; set; }

        [JsonProperty("clicks_count")]
        public int ClicksCount { get; set; }

        [JsonProperty("last_event_time")]
        public DateTime LastEventTime { get; set; }

    }
}
