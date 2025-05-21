namespace LizardCode.SalmaSalud.API.Infrastructure
{
    public class TokenResponse
    {
        public string Token {  get; set; }
        public DateTime? Expiration { get; set; }
    }
}
