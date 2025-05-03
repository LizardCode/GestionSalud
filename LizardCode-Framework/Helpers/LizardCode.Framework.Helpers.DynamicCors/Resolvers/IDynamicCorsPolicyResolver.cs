using Microsoft.AspNetCore.Http;

namespace LizardCode.Framework.Helpers.DynamicCors.Resolvers
{
    public interface IDynamicCorsPolicyResolver
    {
        Task<bool> ResolveForOrigin(HttpContext context, string origin);
    }
}