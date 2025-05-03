using Microsoft.AspNetCore.Cors.Infrastructure;

namespace LizardCode.Framework.Helpers.DynamicCors.Accessors
{
    internal interface ICorsPolicyAccessor
    {
        CorsPolicy GetPolicy();
        CorsPolicy GetPolicy(string name);
    }
}