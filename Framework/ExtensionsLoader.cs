// Copyright (c) Code Solidi Ltd. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using CoreXF.Abstractions;

using Microsoft.Extensions.Logging;

namespace CoreXF.Framework
{
    internal class ExtensionsLoader
    {
        private readonly ILogger logger;
        private readonly IExtensionsRegistry registry;
        private readonly ILoggerFactory factory;

        private ExtensionsLoader(ILoggerFactory factory, IExtensionsRegistry registry)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.logger = factory.CreateLogger<ExtensionsLoader>();
            this.registry = registry;
        }

        public static IExtensionsRegistry DiscoverExtensions(ILoggerFactory factory)
        {
            var registry = new ExtensionsRegistry(factory);
            var excludes = new[] { Assembly.GetAssembly(typeof(ExtensionBase)).FullName };
            var location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _ = new ExtensionsLoader(factory, registry).Discover(location, excludes);
            return registry;
        }

        private ExtensionsLoader Discover(string location, string[] excludes)
        {
            foreach (var path in Directory.GetFiles(location, "*.dll"))
            {
                var assembly = this.LoadAssembly(path);
                if (excludes.Any(x => x != assembly?.FullName))
                {
                    var types = assembly?.GetTypes();
                    var type = types?.SingleOrDefault(x => typeof(IExtension).IsAssignableFrom(x));
                    if (type?.IsAbstract == false)
                    {
                        try
                        {
                            var instance = Activator.CreateInstance(type) as IExtension;
                            (this.registry as ExtensionsRegistry)?.Register(instance);
                        }
                        catch (Exception x)
                        {
                            this.logger.LogError(x.Message);
                        }
                    }
                }
            }

            return this;
        }

        private Assembly LoadAssembly(string assemblyPath)
        {
            try
            {
                // load dependent assemblies:
                // https://samcragg.wordpress.com/2017/06/30/resolving-assemblies-in-net-core/
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);  // WARNING: once loaded it's forever!
                return assembly;
            }
            catch (Exception x)
            {
                this.logger.Log(LogLevel.Error, x, $"Error loading '{assemblyPath}'.");
                return null;
            }
        }
    }
}