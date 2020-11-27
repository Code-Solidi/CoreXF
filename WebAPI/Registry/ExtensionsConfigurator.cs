/*
 * Copyright (c) 2017-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using CoreXF.Abstractions.Builder;
using CoreXF.Abstractions.Registry;
using CoreXF.Framework.Builder;
using CoreXF.Framework.Providers;
using CoreXF.Framework.Settings;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Linq;

namespace CoreXF.Framework.Registry
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
            static IExtensionsRegistry AddRegistry(IServiceCollection services, string location)
            {
                var provider = services.BuildServiceProvider();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var registry = ExtensionsLoader.DiscoverExtensions(loggerFactory, location);
                services.AddSingleton(registry);
                return registry;
            }

            // make extensions available to MVC application, configures extensions and configures extension services (SRP, split?)
            static void AddApplicationParts(IMvcBuilder mvcBuilder, IExtensionsRegistry registry, IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory)
            {
                foreach (var extension in (registry as ExtensionsRegistry)?.Extensions)
                {
                    var assembly = extension.GetType().Assembly;
                    mvcBuilder.AddApplicationPart(assembly);
                    extension.ConfigureServices(services, configuration);
                }
            }

            static void ReplaceControllerFeatureProvider(IServiceCollection services, ILoggerFactory loggerFactory)
            {
                var partManager = services.BuildServiceProvider().GetRequiredService<ApplicationPartManager>();
                var controllerFeatureProvider = partManager.FeatureProviders.SingleOrDefault(p => p is ControllerFeatureProvider);
                partManager.FeatureProviders.Remove(controllerFeatureProvider);
                partManager.FeatureProviders.Add(new ExtensionsControllerFeatureProvider(loggerFactory));
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
            var options2 = configuration.GetSection("CoreXF");
            services.Configure<CoreXfOptions>(options2);

            var actionDescriptorChangeProvider = new ExtensionsActionDescriptorChangeProvider();
            services.AddSingleton<IActionDescriptorChangeProvider>(actionDescriptorChangeProvider);

            services.AddSingleton<IExtensionsApplicationBuilderFactory, ExtensionsApplicationBuilderFactory>();
            var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
            ReplaceControllerFeatureProvider(services, loggerFactory);

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptionsMonitor<CoreXfOptions>>().CurrentValue;
            var registry = AddRegistry(services, options.Location);
            AddApplicationParts(builder, registry, services, configuration, loggerFactory);

            if (options.UseFileProviders) { SetFileProvides(services, registry, loggerFactory); }

            return builder;
        }

        public static IApplicationBuilder UseCoreXF(this IApplicationBuilder builder)
        {
            var services = builder.ApplicationServices;

            var registry = services.GetRequiredService<IExtensionsRegistry>();
            var app = services.GetRequiredService<IExtensionsApplicationBuilderFactory>().CreateBuilder(builder);

            foreach (var extension in (registry as ExtensionsRegistry)?.Extensions)
            {
                extension.ConfigureMiddleware(app);
            }

            return builder;
        }
    }
}