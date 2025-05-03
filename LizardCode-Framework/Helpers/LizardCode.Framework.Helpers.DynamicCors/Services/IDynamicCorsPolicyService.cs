using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace LizardCode.Framework.Helpers.DynamicCors.Services
{
    public interface IDynamicCorsPolicyService
    {
        void ApplyResult(CorsResult result, HttpResponse response);

        Task<CorsResult> EvaluatePolicy(HttpContext context, CorsPolicy policy);
    }
}