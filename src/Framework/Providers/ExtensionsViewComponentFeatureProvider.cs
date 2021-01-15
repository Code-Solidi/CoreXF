/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CoreXF.Abstractions.Attributes;
using CoreXF.Abstractions.Base;
using CoreXF.Framework.Settings;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Logging;

namespace CoreXF.Framework.Providers
{
    public class ExtensionsViewComponentFeatureProvider : IApplicationFeatureProvider<ViewComponentFeature>
    {
        private readonly ILogger<ExtensionsViewComponentFeatureProvider> logger;

        public ExtensionsViewComponentFeatureProvider(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<ExtensionsViewComponentFeatureProvider>();
        }

        public IDictionary<string, IList<TypeInfo>> Areas { get; } = new Dictionary<string, IList<TypeInfo>>();

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ViewComponentFeature feature)
        {
            _  = feature ?? throw new ArgumentNullException(nameof(feature));
            var appParts = parts?.OfType<IApplicationPartTypeProvider>() ?? throw new ArgumentNullException(nameof(parts));

            // inspect all but those parts coming from CoreXF.Framework (fails with an ReflectionTypeLoadException)
            foreach (var part in appParts.Where(x => (x as ApplicationPart)?.Name != Assembly.GetAssembly(typeof(Registry.ExtensionsLoader)).GetName().Name))
            {
                var types = part.Types.Where(t => ExtensionsHelper.IsViewComponent(t) && feature.ViewComponents.Contains(t) == false);
                foreach (var type in types)
                {
                    if (ExtensionsHelper.IsExtension(type.Assembly, this.logger))
                    {
                        // should be one or more, First/OrDefault() doesn't work instead
                        var extensionAttribute = type.GetCustomAttributes().SingleOrDefault(a => a is ExportAttribute);
                        if (extensionAttribute != null)
                        {
                            var area = type.Assembly.GetName().Name;
                            if (this.Areas.ContainsKey(area) == false)
                            {
                                this.Areas.Add(area, new List<TypeInfo>());
                            }

                            this.Areas[area].Add(type);
                            feature.ViewComponents.Add(type);
                            this.logger.LogInformation($"View Component '{type.AsType().FullName}' has been registered and is accessible.");
                        }
                        else
                        {
                            this.logger.LogWarning($"View Component '{type.AsType().FullName}' is inaccessible. Decorate it with '{nameof(ExportAttribute)}' if you want to access it.");
                        }
                    }
                    else
                    {
                        feature.ViewComponents.Add(type);
                    }
                }
            }
        }
    }
}