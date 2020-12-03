/*
 * Copyright (c) 2016-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using CoreXF.Abstractions.Base;
using CoreXF.Abstractions.Registry;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreXF.Framework.Registry
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

        public static IExtensionsRegistry DiscoverExtensions(ILoggerFactory factory, string location)
        {
            var registry = new ExtensionsRegistry(factory);
            var excludes = new[]
            {
                Assembly.GetAssembly(typeof(ExtensionBase)).FullName,       // CoreXF.Abstractions
                Assembly.GetAssembly(typeof(ExtensionsLoader)).FullName     // CoreXF.Framework
            };

            //var location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _ = new ExtensionsLoader(factory, registry).Discover(location, excludes);
            return registry;
        }

        private ExtensionsLoader Discover(string location, string[] excludes)
        {
            var files = Array.Empty<string>();
            try
            {
                location = location.Trim(' ', '\t', '\\', '/');
                location = Path.Combine(Environment.CurrentDirectory, location);
                files = Directory.GetFiles(location, "*.dll", SearchOption.AllDirectories);
            }
            catch (Exception x)
            {
                this.logger.LogError(x.Message);
                throw;
            }

            foreach (var path in files)
            {
                var assembly = ExtensionsLoader.LoadAssembly(path, this.logger);
                this.logger.LogDebug($"Inspecting {assembly.Location}.");
                if (excludes.Any(x => x == assembly?.FullName) == false)
                {
                    try
                    {
                        var types = assembly?.GetTypes();
                        var type = types?.SingleOrDefault(x => typeof(IExtension).IsAssignableFrom(x));
                        if (type?.IsAbstract == false)
                        {
                            try
                            {
                                var instance = Activator.CreateInstance(type) as IExtension;
                                instance.Location = Path.GetDirectoryName(path);
                                (this.registry as ExtensionsRegistry)?.Register(instance);
                            }
                            catch (Exception x)
                            {
                                this.logger.LogError(x.InnerException?.Message ?? x.Message);
                            }
                        }
                    }
                    catch (ReflectionTypeLoadException x)
                    {
                        this.logger.LogError(x.InnerException?.Message ?? x.Message);
                    }
                }
            }

            return this;
        }

        internal static Assembly LoadAssembly(string assemblyPath, ILogger logger)
        {
            try
            {
                // load dependent assemblies:
                // https://samcragg.wordpress.com/2017/06/30/resolving-assemblies-in-net-core/
                return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);  // WARNING: once loaded it's forever!
            }
            catch (Exception x)
            {
                logger.Log(LogLevel.Error, x, $"Error loading '{assemblyPath}'.");
                return null;
            }
        }
    }
}