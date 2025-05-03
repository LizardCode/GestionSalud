using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.Framework.Application.Middlewares
{
    public class AuditMiddleware
    {
        private ILogger<AuditMiddleware> _logger;
        private readonly RequestDelegate _next;


        public AuditMiddleware(ILogger<AuditMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }


        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;

            request.EnableBuffering();

            var sr = new StreamReader(request.Body, leaveOpen: true);
            var body = await sr.ReadToEndAsync();
            
            request.Body.Seek(0, SeekOrigin.Begin);

            var url = $"AUDIT >> {request.Method} {request.Protocol}://{request.Host}{request.Path}";
            _logger.LogDebug(url);

            if (body.IsNotNull())
                _logger.LogDebug($"AUDIT >> BODY {body}");

            await _next(context);
        }
    }
}
