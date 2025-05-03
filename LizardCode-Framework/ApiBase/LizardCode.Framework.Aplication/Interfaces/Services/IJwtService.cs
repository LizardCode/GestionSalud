namespace LizardCode.Framework.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string CreateJwtSecurityToken(string user, out DateTime? expiration);
    }
}