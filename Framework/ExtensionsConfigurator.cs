// Copyright (c) Code Solidi Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Linq;

using CoreXF.Abstractions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoreXF.Framework
{
    public static class ExtensionsConfigurator
    {
        /// <summary>
        /// Adds the CoreXF to DI container.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static IMvcBuilder AddCoreXF(this IMvcBuilder builder, IServiceCollection services, IConfiguration configuration)
        {
            // add registry as a service
            static IExtensionsRegistry AddRegistry(IServiceCollection services)
            {
                var provider = services.BuildServiceProvider();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var registry = ExtensionsLoader.DiscoverExtensions(loggerFactory);
                services.AddSingleton(registry);
                return registry;
            }

            // make extensions available to MVC application, configures extensions and configures extension services (SRP, split?)
            static void AddApplicationParts(IMvcBuilder mvcBuilder, IExtensionsRegistry registry, IServiceCollection services, IConfiguration configuration)
            {
                foreach (var extension in (registry as ExtensionsRegistry)?.Extensions)
                {
                    var assembly = extension.GetType().Assembly;
                    mvcBuilder.AddApplicationPart(assembly);

                    var startUpName = $"{assembly.GetName().Name}.Startup";
                    var startup = assembly.GetType(startUpName);
                    if (startup != null)
                    {
                        dynamic instance = Activator.CreateInstance(startup, configuration);
                        instance.ConfigureServices(services);
                    }
                }
            }

            static void ReplaceControllerFeatureProvider(IServiceCollection services, ILoggerFactory loggerFactory)
            {
                var partManager = services.BuildServiceProvider().GetRequiredService<ApplicationPartManager>();
                var controllerFeatureProvider = partManager.FeatureProviders.SingleOrDefault(p => p is ControllerFeatureProvider);
                partManager.FeatureProviders.Remove(controllerFeatureProvider);
                partManager.FeatureProviders.Add(new ExtensionsControllerFeatureProvider(loggerFactory));
            }

            // replace view component feature provider
            static void ReplaceViewComponentFeatureProvider(IServiceCollection services, ILoggerFactory loggerFactory)
            {
                var partManager = services.BuildServiceProvider().GetRequiredService<ApplicationPartManager>();
                var viewComponentFeatureProvider = partManager.FeatureProviders.SingleOrDefault(p => p is ViewComponentFeatureProvider);
                partManager.FeatureProviders.Remove(viewComponentFeatureProvider);
                partManager.FeatureProviders.Add(new ExtensionsViewComponentFeatureProvider(loggerFactory));
            }

            // replace tag helper feature provider (PopulateFeature() does not get called!?)
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

            services.AddOptions();
            services.Configure<CoreXfOptions>(configuration.GetSection("CoreXF"));

            var actionDescriptorChangeProvider = new ExtensionsActionDescriptorChangeProvider();
            services.AddSingleton<IActionDescriptorChangeProvider>(actionDescriptorChangeProvider);

            var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();

            var registry = AddRegistry(services);

            AddApplicationParts(builder, registry, services, configuration);

            ReplaceControllerFeatureProvider(services, loggerFactory);

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptionsMonitor<CoreXfOptions>>().CurrentValue;

            // NB: what options were to do doesn't work! No time to explore details.
            if (options.UseViewComponents) { ReplaceViewComponentFeatureProvider(services, loggerFactory); }

            if (options.UseTagHelpers) { ReplaceTagHelperFeatureProvider(services, loggerFactory); }

            if (options.UseFileProviders) { SetFileProvides(services, registry, loggerFactory); }

            return builder;
        }

        public static IApplicationBuilder UseCoreXF(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExtensionsMiddleware>();
        }
    }
}