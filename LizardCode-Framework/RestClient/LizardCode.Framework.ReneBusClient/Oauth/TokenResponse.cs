namespace LizardCode.Framework.ReneBusClient.Oauth
{
    public class TokenResponse
    {
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
        public int Expires_In { get; set; } // Seconds
        public DateTime Issued { get; set; }
        public DateTime Expires { get; set; }
    }
}
