﻿/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Abstractions.Base;
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
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Diagnostics.CodeAnalysis;
using System.IO;
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
                        var logger = loggerFactory.CreateLogger(nameof(ExtensionsConfigurator));
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

        public static IApplicationBuilder UseCoreXF(this IApplicationBuilder builder)
        {
#if V20
            var services = builder.ApplicationServices;

            var registry = services.GetRequiredService<IExtensionsRegistry>();
            var app = services.GetRequiredService<IExtensionsApplicationBuilderFactory>().CreateBuilder(builder);

            foreach (var extension in (registry as ExtensionsRegistry)?.Extensions)
            {
                extension.ConfigureMiddleware(app);
            }

#else

#endif
            return builder;
        }
    }
}