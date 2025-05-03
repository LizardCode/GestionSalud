using System.Collections.Generic;

namespace LizardCode.Framework.Helpers.Utilities.Mail
{
    public class Message
    {
        public string From { get; set; }

        public List<string> To { get; set; }

        public string ReplayTo { get; set; }

        public string Subject { get; set; }

        public bool IsBodyHtml { get; set; }

        public string Body { get; set; }

        public bool Async { get; set; }

        public List<Attachment> Attachments { get; set; }

        public List<Resource> LinkedResources { get; set; }


        public Message()
        {
            //
        }
    }
}
