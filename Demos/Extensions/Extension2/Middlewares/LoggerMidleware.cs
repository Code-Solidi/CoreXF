//#define Demo
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Extension2.Middlewares
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public LoggerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.logger = loggerFactory.CreateLogger<LoggerMiddleware>();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
#if Demo
            System.Console.WriteLine($"Before {this.GetType().Name}");
#endif
            this.logger.LogTrace($"Before {this.GetType().Name}");
            using (var reader = new StreamReader(httpContext.Request.Body))
            {
                var requestBody = await reader.ReadToEndAsync().ConfigureAwait(false);
                this.logger.LogInformation(requestBody);
            }

            await this.next.Invoke(httpContext);
#if Demo
            System.Console.WriteLine($"After {this.GetType().Name}");
#endif
            this.logger.LogTrace($"After {this.GetType().Name}");
        }
    }
}