/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Abstractions.Base;
using CoreXF.Abstractions.Builder;
using CoreXF.Abstractions.Registry;
using CoreXF.Framework.Builder;
using CoreXF.Framework.Providers;
using CoreXF.Framework.Registry;
using CoreXF.Framework.Settings;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreXF.Framework
{
    /// <summary>
    /// The startup extensions.
    /// </summary>
    public static class StartupExtensions
    {
        /// <summary>
        /// Adds the CoreXF to DI container.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        /// 
#if NETCOREAPP3_1
        [SuppressMessage("Design", "CC0021:Use nameof", Justification = "<Pending>")]
        [SuppressMessage("Info Code Smell", "S1135:Track uses of \"TODO\" tags", Justification = "<Pending>")]
        public static IMvcBuilder AddCoreXF(this IMvcBuilder builder, IServiceCollection services, IConfiguration configuration)
        {
#endif
#if NET6_0
        public static IMvcBuilder AddCoreXF(this IMvcBuilder builder, IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
#endif

            // add registry as a service
            static IExtensionsRegistry AddRegistry(IServiceCollection services, string location)
            {
                var provider = services.BuildServiceProvider();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var registry = ExtensionsLoader.DiscoverExtensions(services, loggerFactory, location);
                services.AddSingleton(registry);
                return registry;
            }

            // make extensions available to MVC application, configures extensions and configures extension services (SRP, split?)
            static void AddApplicationParts(IMvcBuilder mvcBuilder, IExtensionsRegistry registry, IServiceCollection services, ILoggerFactory loggerFactory)
            {
                foreach (var extension in (registry as ExtensionsRegistry)?.Extensions)
                {
                    var assembly = extension.GetType().Assembly;
                    mvcBuilder.AddApplicationPart(assembly);

#if NETCOREAPP3_1
                    if (extension is IMvcExtension)
                    {
                        // load compiled views
                        var directory = extension.Location;
                        var viewsAssemblyName = (extension as IMvcExtension).Views;
                        var logger = loggerFactory.CreateLogger(nameof(StartupExtensions));
                        var viewsAssembly = ExtensionsLoader.LoadAssembly(Path.Combine(directory, viewsAssemblyName), logger);
                        mvcBuilder.AddApplicationPart(viewsAssembly);
                    }
#endif
                    //extension.ConfigureServices(services); -- called during registration!! 
                }
            }

            static void ReplaceControllerFeatureProvider(IServiceCollection services, IExtensionsRegistry registry, ILoggerFactory loggerFactory)
            {
                var provider = services.BuildServiceProvider();
                var partManager = provider.GetRequiredService<ApplicationPartManager>();
                var controllerFeatureProvider = partManager.FeatureProviders.SingleOrDefault(p => p is ControllerFeatureProvider);
                partManager.FeatureProviders.Remove(controllerFeatureProvider);
                partManager.FeatureProviders.Add(new ExtensionsControllerFeatureProvider(registry, loggerFactory));
            }

            static void ReplaceViewComponentFeatureProvider(IServiceCollection services, ILoggerFactory loggerFactory)
            {
                var partManager = services.BuildServiceProvider().GetRequiredService<ApplicationPartManager>();
                var viewComponentFeatureProvider = partManager.FeatureProviders.SingleOrDefault(p => p is ViewComponentFeatureProvider);
                partManager.FeatureProviders.Remove(viewComponentFeatureProvider);
                partManager.FeatureProviders.Add(new ExtensionsViewComponentFeatureProvider(loggerFactory));
            }

            // todo: PopulateFeature() does not get called!? find out why!!
            static void ReplaceTagHelperFeatureProvider(IServiceCollection services, ILoggerFactory loggerFactory)
            {
                var partManager = services.BuildServiceProvider().GetRequiredService<ApplicationPartManager>();
                var tagHelperFeatureProvider = partManager.FeatureProviders.SingleOrDefault(p => p is TagHelperFeatureProvider);
                partManager.FeatureProviders.Remove(tagHelperFeatureProvider);
                partManager.FeatureProviders.Add(new ExtensionsTagHelperFeatureProvider(loggerFactory));
            }

            static void SetFileProvides(IServiceCollection services, IExtensionsRegistry registry, ILoggerFactory loggerFactory)
            {
                var hostingEnvironment = services.BuildServiceProvider().GetRequiredService<IHostingEnvironment>();
                hostingEnvironment.WebRootFileProvider = new ExtensionsFileProviderAggregator(loggerFactory
                    , (registry as ExtensionsRegistry)?.Extensions
                    , hostingEnvironment.WebRootFileProvider).Composite;
                hostingEnvironment.ContentRootFileProvider = new ExtensionsFileProviderAggregator(loggerFactory
                    , (registry as ExtensionsRegistry)?.Extensions
                    , hostingEnvironment.ContentRootFileProvider).Composite;
            }

            // here we go...
            services.AddOptions();
            services.Configure<CoreXfOptions>(configuration.GetSection("CoreXF"));

            var actionDescriptorChangeProvider = new ExtensionsActionDescriptorChangeProvider();
            services.AddSingleton<IActionDescriptorChangeProvider>(actionDescriptorChangeProvider);

            var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();

            services.AddSingleton<IExtensionsApplicationBuilderFactory, ExtensionsApplicationBuilderFactory>();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptionsMonitor<CoreXfOptions>>().CurrentValue;
            var registry = AddRegistry(services, options.Location);

            ReplaceControllerFeatureProvider(services, registry, loggerFactory);
            AddApplicationParts(builder, registry, services, loggerFactory);

            // NB: what options were to do doesn't work! No time to explore details.
            if (options.UseViewComponents) { ReplaceViewComponentFeatureProvider(services, loggerFactory); }

            if (options.UseTagHelpers) { ReplaceTagHelperFeatureProvider(services, loggerFactory); }

            if (options.UseFileProviders) { SetFileProvides(services, registry, loggerFactory); }

            return builder;
        }

        /// <summary>
        /// Register CoreXF middleware.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>An IApplicationBuilder.</returns>
        public static IApplicationBuilder UseCoreXF(this IApplicationBuilder builder)
        {
#if V20 // there was a chance to register middlewares in v2.0 -- removed!
            var services = builder.ApplicationServices;

            var registry = services.GetRequiredService<IExtensionsRegistry>();
            var app = services.GetRequiredService<IExtensionsApplicationBuilderFactory>().CreateBuilder(builder);

            foreach (var extension in (registry as ExtensionsRegistry)?.Extensions)
            {
                extension.ConfigureMiddleware(app);
            }

#else

#endif
            var options = builder.ApplicationServices.GetRequiredService<IOptionsMonitor<CoreXfOptions>>().CurrentValue;
            var registry = builder.ApplicationServices.GetRequiredService<IExtensionsRegistry>();
            foreach (var extension in registry.Extensions)
            {
                var path = extension.Location;

                // NB: we rely on wwwroot to locate static files; do not change it!
                var extensionStaticFilesPath = Path.Combine(extension.Location, "wwwroot");
                var hasStaticFiles =  Directory.Exists(extensionStaticFilesPath);
                if (hasStaticFiles)
                {
                    builder.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(extensionStaticFilesPath)
                    });
                }
            }

            return builder.UseMiddleware<CoreXFMiddleware>();
        }
    }

    /// <summary>
    /// The core XF middleware.
    /// </summary>
    public class CoreXFMiddleware
    {
        /// <summary>
        /// The next.
        /// </summary>
        private readonly RequestDelegate next;
        /// <summary>
        /// The logger.
        /// </summary>
        readonly ILogger logger;
        /// <summary>
        /// The extensions registry.
        /// </summary>
        readonly IExtensionsRegistry extensionsRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreXFMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="extensionsRegistry">The extensions registry.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public CoreXFMiddleware(RequestDelegate next, IExtensionsRegistry extensionsRegistry, ILoggerFactory loggerFactory)
        {
            this.extensionsRegistry = extensionsRegistry;
            this.logger = loggerFactory.CreateLogger(this.GetType());
            this.next = next;
        }

        /// <summary>
        /// Invokes the <see cref="Task"/>.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>A Task.</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            var endPointFeature = httpContext.Features.Get<IEndpointFeature>();
            var endPoint = endPointFeature?.Endpoint;
            var controllerActionDescriptor = endPoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            var processed = false;
            if (controllerActionDescriptor != null)
            {
                var suffix = controllerActionDescriptor?.ControllerName.EndsWith("Controller") ?? true ? string.Empty : "Controller";
                var controllerName = $"{controllerActionDescriptor?.ControllerName}{suffix}";
                this.logger?.LogDebug($"Controller: {controllerName}");
                foreach (var extension in this.extensionsRegistry?.Extensions.Where(x => x is IWebApiExtension))
                {
                    var controller = extension.GetType().Assembly.GetTypes()
                        .SingleOrDefault(x => x.Name.Equals(controllerName, StringComparison.OrdinalIgnoreCase));
                    if (controller != default && (extension as IWebApiExtension)?.Status == IWebApiExtension.ExtensionStatus.Stopped)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;  // or other?
                        await Task.CompletedTask;
                        processed = true;
                    }
                }
            }

            if (!processed)
            {
                // call the next middleware delegate in the pipeline 
                await this.next.Invoke(httpContext);
            }
        }
    }
}