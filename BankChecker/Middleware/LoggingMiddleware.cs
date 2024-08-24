using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BankChecker.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate requestDelegate, ILogger<LoggingMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation($"Incoming request :{context.Request.Method} {context.Request.Path}");

            await _requestDelegate(context);

            stopwatch.Stop();
            _logger.LogInformation($"Incoming request :{context.Request.Method} {context.Request.Path} in{stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
