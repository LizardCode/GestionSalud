namespace LizardCode.Framework.Helpers.ChatApi.Responses
{
    public class SendMessageResponse
    {
        public bool Sent { get; set; }
        public string Id { get; set; }
        public string Status { get; set; }
        public string Messages { get; set; }
        public string Msg { get; set; } //Presente en una rta erronea de login
    }
}
