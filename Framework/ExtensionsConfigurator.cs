// Copyright (c) Code Solidi Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
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

namespace CoreXF.Framework
{
    public static class ExtensionsConfigurator
    {
        private static void AddApplicationParts(IMvcBuilder mvcBuilder, IExtensionsRegistry registry)
        {
            foreach (var extension in (registry as ExtensionsRegistry).Extensions)
            {
                var assembly = extension.GetType().Assembly;
                mvcBuilder.AddApplicationPart(assembly);
            }
        }

        public static IMvcBuilder AddCoreXF(this IMvcBuilder builder, IServiceCollection services, IConfiguration configuration)
        {
            // add services
            var actionDescriptorChangeProvider = new ExtensionsActionDescriptorChangeProvider();
            services.AddSingleton<IActionDescriptorChangeProvider>(actionDescriptorChangeProvider);

            var provider = services.BuildServiceProvider();

            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            //loggerFactory.AddConsole(configuration).AddDebug();

            var hostingEnvironment = provider.GetRequiredService<IHostingEnvironment>();

            var registry = ExtensionsLoader.DiscoverExtensions(loggerFactory);
            services.AddSingleton<IExtensionsRegistry>(registry);

            // make extensions available to MVC application
            ExtensionsConfigurator.AddApplicationParts(builder, registry);

            // replace controller feature provider
            var partManager = provider.GetRequiredService<ApplicationPartManager>();
            var controllerFeatureProvider = partManager.FeatureProviders.SingleOrDefault(p => p is ControllerFeatureProvider);
            partManager.FeatureProviders.Remove(controllerFeatureProvider);
            partManager.FeatureProviders.Add(new ExtensionsControllerFeatureProvider(loggerFactory));

            // replace view component feature provider
            var viewComponentFeatureProvider = partManager.FeatureProviders.SingleOrDefault(p => p is ViewComponentFeatureProvider);
            partManager.FeatureProviders.Remove(viewComponentFeatureProvider);
            partManager.FeatureProviders.Add(new ExtensionsViewComponentFeatureProvider(loggerFactory));

            // replace tag helper feature provider (PopulateFeature() does not get called!?)
            var tagHelperFeatureProvider = partManager.FeatureProviders.SingleOrDefault(p => p is TagHelperFeatureProvider);
            partManager.FeatureProviders.Remove(tagHelperFeatureProvider);
            partManager.FeatureProviders.Add(new ExtensionsTagHelperFeatureProvider(loggerFactory));

            // set file providers
            hostingEnvironment.WebRootFileProvider = new ExtensionsFileProviderAggregator(loggerFactory
                , (registry as ExtensionsRegistry)?.Extensions
                , hostingEnvironment.WebRootFileProvider).Composite;
            hostingEnvironment.ContentRootFileProvider = new ExtensionsFileProviderAggregator(loggerFactory
                , (registry as ExtensionsRegistry)?.Extensions
                , hostingEnvironment.ContentRootFileProvider).Composite;

            return builder;
        }

        public static IApplicationBuilder UseCoreXF(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExtensionsMiddleware>();
        }
    }
}