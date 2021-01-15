/*
 * Copyright (c) 2016-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using CoreXF.Abstractions.Attributes;
using CoreXF.Abstractions.Registry;
using CoreXF.Framework.Settings;

using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreXF.Framework.Providers
{
    public class ExtensionsControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly ILogger<ExtensionsControllerFeatureProvider> logger;
        private readonly IExtensionsRegistry registry;

        public ExtensionsControllerFeatureProvider(IExtensionsRegistry registry, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<ExtensionsControllerFeatureProvider>();
            this.registry = registry;
        }

        public IDictionary<string, IList<TypeInfo>> Areas { get; } = new Dictionary<string, IList<TypeInfo>>();

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            _ = feature ?? throw new ArgumentNullException(nameof(feature));

            var appParts = parts?.OfType<IApplicationPartTypeProvider>() ?? throw new ArgumentNullException(nameof(parts));

            // inspect all but those parts coming from CoreXF.Framework (fails with an ReflectionTypeLoadException)
            foreach (var part in appParts.Where(x => (x as ApplicationPart)?.Name != Assembly.GetAssembly(typeof(Registry.ExtensionsLoader)).GetName().Name))
            {
                foreach (var type in part.Types.Where(t => ExtensionsHelper.IsController(t) && feature.Controllers.Contains(t) == false))
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

                            feature.Controllers.Add(type);

                            this.registry.GetExtension(((AssemblyPart)part).Assembly).AddController(type);

                            this.logger.LogInformation($"Controller '{type.AsType().FullName}' has been registered and is accessible.");
                        }
                        else
                        {
                            this.logger.LogWarning($"Controller '{type.AsType().FullName}' is inaccessible. Decorate it with '{nameof(ExportAttribute)}' if you want to access it.");
                        }
                    }
                    else
                    {
                        feature.Controllers.Add(type);
                    }
                }
            }
        }
    }
}