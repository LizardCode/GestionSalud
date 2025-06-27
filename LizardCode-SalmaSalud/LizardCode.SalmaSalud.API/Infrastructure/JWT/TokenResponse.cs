namespace LizardCode.SalmaSalud.API.Infrastructure.JWT
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
