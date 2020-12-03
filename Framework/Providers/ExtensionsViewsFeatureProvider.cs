/*
 * Copyright (c) 2016-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using CoreXF.Abstractions.Attributes;
using CoreXF.Framework.Settings;

using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreXF.Framework.Providers
{
    public class ExtensionsViewsFeatureProvider : IApplicationFeatureProvider<ViewsFeature>
    {
        private readonly ILogger<ExtensionsViewsFeatureProvider> logger;

        public ExtensionsViewsFeatureProvider(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<ExtensionsViewsFeatureProvider>();
        }

        public IDictionary<string, IList<TypeInfo>> Areas { get; } = new Dictionary<string, IList<TypeInfo>>();

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ViewsFeature feature)
        {
            _ = feature ?? throw new ArgumentNullException(nameof(feature));

            var appParts = parts?.OfType<IApplicationPartTypeProvider>() ?? throw new ArgumentNullException(nameof(parts));

            // inspect all but those parts coming from CoreXF.Framework (fails with an ReflectionTypeLoadException)
            foreach (var part in appParts.Where(x => (x as ApplicationPart)?.Name != Assembly.GetAssembly(typeof(Registry.ExtensionsLoader)).GetName().Name))
            {
                var types = part.Types.Where(t => ExtensionsHelper.IsCompiledView(t) && feature.ViewDescriptors.Select(x => x.Type).Contains(t) == false);
                foreach (var viewDescriptor in feature.ViewDescriptors)
                {
                    if (ExtensionsHelper.IsExtension(viewDescriptor.Type.Assembly, this.logger))
                    {

                    }
                    else
                    {
                        feature.ViewDescriptors.Add(viewDescriptor);
                    }
                }

                foreach (var type in types)
                {
                    if (ExtensionsHelper.IsExtension(type.Assembly, this.logger))
                    {
                        // should be one or more, First/OrDefault() doesn't work instead
                        var extensionAttribute = type.GetCustomAttributes().SingleOrDefault(a => a is ExportAttribute);
                        if (extensionAttribute != null)
                        {
                            //var area = type.Assembly.GetName().Name;
                            //if (this.Areas.ContainsKey(area) == false)
                            //{
                            //    this.Areas.Add(area, new List<TypeInfo>());
                            //}

                            //this.Areas[area].Add(type);
                            var viewDescriptor = feature.ViewDescriptors.Single(x => x.Type == type);
                            feature.ViewDescriptors.Add(viewDescriptor);
                            this.logger.LogInformation($"View '{type.AsType().FullName}' has been registered and is accessible.");
                        }
                        else
                        {
                            this.logger.LogWarning($"View '{type.AsType().FullName}' is inaccessible. Decorate it with '{nameof(ExportAttribute)}' if you want to access it.");
                        }
                    }
                    else
                    {
                        feature.ViewDescriptors.Add(type);
                    }
                }
            }
        }
    }
}