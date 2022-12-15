using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;

using System;
using System.Threading.Tasks;

namespace MiddlewareClassLibrary
{
    public class CoreXFMiddlwware
    {
        private readonly RequestDelegate next;

        public CoreXFMiddlwware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var endPointFeature = httpContext.Features.Get<IEndpointFeature>();
            var endPoint = endPointFeature?.Endpoint;
            var controllerActionDescriptor = endPoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            Console.WriteLine($"Controller: {controllerActionDescriptor?.ControllerName ?? "n/a"}, Action: {controllerActionDescriptor?.ActionName ?? "n/a"}");

            // Call the next middleware delegate in the pipeline 
            await this.next.Invoke(httpContext);
        }
    }

    public static class CoreXFMiddlwwareExtensions
    {
        public static IApplicationBuilder UseSOAPEndpoint(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CoreXFMiddlwware>();
        }
    }
}
