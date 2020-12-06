using CoreXF.Abstractions.Builder;
using CoreXF.WebApiHost.Controllers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace CoreXF.WebApiHost
{
    public static class HostExtensions
    {
        public static IExtensionsApplicationBuilder UseCoreXFHost(this IExtensionsApplicationBuilder app, IWebHostEnvironment env)
        {
            var currentProvider = env.WebRootFileProvider;
            var embeddedProvider = new EmbeddedFileProvider(typeof(HomeController).Assembly, $"{nameof(CoreXF)}.{nameof(WebApiHost)}.wwwroot");
            env.WebRootFileProvider = new CompositeFileProvider(embeddedProvider, currentProvider);
            return app;
        }
    }
}