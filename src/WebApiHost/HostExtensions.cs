using CoreXF.Abstractions.Base;
using CoreXF.Abstractions.Builder;
using CoreXF.Abstractions.Registry;
using CoreXF.WebApiHost.Controllers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.FileProviders;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CoreXF.WebApiHost
{
    /// <summary>
    /// Filters out requests to "stopped" extensions.
    /// </summary>
    public static class HostExtensions
    {
        public static IExtensionsApplicationBuilder UseCoreXFHost(this IExtensionsApplicationBuilder app, IWebHostEnvironment env)
        {
            var currentProvider = env.WebRootFileProvider;
            var embeddedProvider = new EmbeddedFileProvider(typeof(HomeController).Assembly, $"{nameof(CoreXF)}.{nameof(WebApiHost)}.wwwroot");
            env.WebRootFileProvider = new CompositeFileProvider(embeddedProvider, currentProvider);
            app.UseMiddleware<RequestMiddleware>();
            return app;
        }
    }

    /// <summary>
    /// NB: Should be called right before dispatching to any of the controllers/actions
    /// </summary>
    public class RequestMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IExtensionsRegistry registry;
        private readonly Dictionary<string, IExtension> routes;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1125:Boolean literals should not be redundant", Justification = "<Pending>")]
        public RequestMiddleware(RequestDelegate next, IExtensionsRegistry registry, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            this.next = next;
            this.registry = registry;

            this.routes = new Dictionary<string, IExtension>();
            var extAssemblyNames = this.registry.Extensions.Select(t => Path.GetFileName(t.Location));
            /*var extensionControllers = */
            _ = actionDescriptorCollectionProvider.ActionDescriptors.Items.Where(x =>
            {
                var parts = x.DisplayName.Split('.');
                var extensionAssemblyFileName = parts.Length > 0 ? parts[0] : string.Empty;
                if (extAssemblyNames.Any(t => t == extensionAssemblyFileName))
                {
                    var route = $"/{x.AttributeRouteInfo?.Template}";
                    var extension = this.registry.Extensions.SingleOrDefault(x => Path.GetFileName(x.Location) == extensionAssemblyFileName);

                    /*
                     * There might be duplicates because of same roots but different verbs (GET, POST, PUT, DELETE)
                     */
                    if (this.routes.ContainsKey(route) == false)
                    {
                        this.routes.Add(route, extension);
                    }
                    else
                    {
                        var existingExtension = this.routes[route];
                        if (existingExtension != extension)
                        {
                            throw new InvalidOperationException($"Route '{route}' is handled by more than one extension.");
                        }
                    }
                }

                return true;
            });
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (routes.ContainsKey(path))
            {
                var extension = routes[path];
                if (extension.Status == IExtension.ExtensionStatus.Stopped)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await context.Response.WriteAsync("Stopped");
                    return;
                }
            }

            await this.next(context);
        }
    }
}