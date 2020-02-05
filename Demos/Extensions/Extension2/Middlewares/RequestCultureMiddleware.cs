//#define Demo
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Extension2.Middlewares
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-3.1
    /// </summary>
    public class RequestCultureMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public RequestCultureMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.logger = loggerFactory.CreateLogger<RequestCultureMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
#if Demo
            System.Console.WriteLine($"Before {this.GetType().Name}");
#endif
            this.logger.LogTrace($"Before {this.GetType().Name}");
            var cultureQuery = context.Request.Query["culture"];
            if (string.IsNullOrWhiteSpace(cultureQuery) == false)
            {
                var culture = new CultureInfo(cultureQuery);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

            }

            // Call the next delegate/middleware in the pipeline
            await this.next(context).ConfigureAwait(false);
#if Demo
            System.Console.WriteLine($"After {this.GetType().Name}");
#endif
            this.logger.LogTrace($"After {this.GetType().Name}");
        }
    }

    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCulture(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestCultureMiddleware>();
        }
    }
}
