/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Abstractions.Base;
using CoreXF.Abstractions.Registry;

using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

//using static System.Net.WebRequestMethods;

namespace CoreXF.Framework.Registry
{
    /// <summary>
    /// The extensions loader.
    /// </summary>
    internal sealed class ExtensionsLoader
    {
        private readonly ILogger logger;
        private readonly IExtensionsRegistry registry;
        private readonly ILoggerFactory factory;

        /// <summary>
        /// Prevents a default instance of the <see cref="ExtensionsLoader"/> class from being created.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="registry">The registry.</param>
        private ExtensionsLoader(ILoggerFactory factory, IExtensionsRegistry registry)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.logger = factory.CreateLogger<ExtensionsLoader>();
            this.registry = registry;
        }

        /// <summary>
        /// Discover extensions.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="location">The location.</param>
        /// <returns>An IExtensionsRegistry.</returns>
        public static IExtensionsRegistry DiscoverExtensions(IServiceCollection services, ILoggerFactory factory, string location)
        {
            var registry = new ExtensionsRegistry(factory);
            var excludes = new[]
            {
                Assembly.GetAssembly(typeof(AbstractExtension)).FullName,       // CoreXF.Abstractions
                Assembly.GetAssembly(typeof(ExtensionsLoader)).FullName     // CoreXF.Framework
            };

            //new ExtensionsLoader(factory, registry).Discover(services, location, excludes);
            new ExtensionsLoader(factory, registry).Discover(services, location);

            return registry;
        }

        private Assembly LoadExtension(string folder)
        {
            // 1. locate any of <name>.deps.json or <name>.runtimeconfig.json
            var supplementary = Directory.GetFiles(folder, "*.deps.json").SingleOrDefault() ??
                Directory.GetFiles(folder, "*.runtimeconfig.json").SingleOrDefault();

            // 2. determine extension name
            var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(supplementary));

            // 3. load the assembly
            var libraries = DependencyContext.Default.CompileLibraries.Cast<Library>().Union(DependencyContext.Default.RuntimeLibraries).Distinct();
            var extensionPath = Path.ChangeExtension(Path.Combine(folder, name) + ".", ".dll");
            var assembly = ExtensionsLoader.LoadAssembly(extensionPath, libraries, this.logger);

            // 4. load dependencies
            var files = Directory.GetFiles(folder, "*.dll", SearchOption.AllDirectories);
            var dependencyContext = DependencyContext.Load(assembly);
            foreach (var dependency in dependencyContext.RuntimeLibraries.Where(x => x.Name != name).Select(x => x.Name))
            {
                var dependencyPath = Path.ChangeExtension(Path.Combine(folder, dependency) + ".", ".dll");
                if (File.Exists(dependencyPath))
                {
                    _ = ExtensionsLoader.LoadAssembly(dependencyPath, libraries, this.logger);
                }
            }

            return assembly;
        }

        /// <summary>
        /// Discover extensions in 'location'.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="location">The location.</param>
        private void Discover(IServiceCollection services, string location)
        {
            var files = Array.Empty<string>();
            try
            {
                location = Path.Combine(Environment.CurrentDirectory, location.Trim(' ', '\t', '\\', '/'));
                //files = Directory.GetFiles(location, "*.dll", SearchOption.AllDirectories);

                foreach (var folder in Directory.EnumerateDirectories(location))
                {
                    var assembly = this.LoadExtension(folder);
                    try
                    {
                        var types = assembly?.GetTypes();
                        var type = types?.SingleOrDefault(x => typeof(IExtension).IsAssignableFrom(x));
                        if (type?.IsAbstract == false)
                        {
                            try
                            {
                                var instance = Activator.CreateInstance(type);
                                var extension = instance as IExtension;
                                if (instance != default)
                                {
                                    extension.ConfigureServices(services);
                                    extension.Configure(assembly);
                                    (this.registry as ExtensionsRegistry)?.Register(extension);
                                }
                            }
                            catch (Exception x)
                            {
                                this.logger.LogDebug(x.InnerException?.Message ?? x.Message);
                            }
                        }
                    }
                    catch (ReflectionTypeLoadException x)
                    {
                        this.logger.LogDebug(x.InnerException?.Message ?? x.Message);
                    }
                }
            }
            catch (Exception x)
            {
                this.logger.LogError(x.Message);
                return;
            }
        }

        /// <summary>
        /// Load assembly.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="libraries">The libraries.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>An Assembly.</returns>
        internal static Assembly LoadAssembly(string assemblyPath, IEnumerable<Library> libraries, ILogger logger)
        {
            //if (assemblyPath.Contains("Extensions\\Identity"))
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            libraries ??= DependencyContext.Default.CompileLibraries.Cast<Library>().Union(DependencyContext.Default.RuntimeLibraries).Distinct();
            try
            {
                return libraries.Any(x => x.Name.Equals(Path.GetFileNameWithoutExtension(assemblyPath), StringComparison.OrdinalIgnoreCase))
                    ? default//Assembly.Load(new AssemblyName(fileNameWithOutExtension))
                    // https://samcragg.wordpress.com/2017/06/30/resolving-assemblies-in-net-core/
                    : AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);  // WARNING: once loaded it's forever!
            }
            catch (FileLoadException x)
            {
                logger.LogError(x, $"Cannot load '{assemblyPath}'");
            }
            catch (BadImageFormatException x)
            {
                logger.Log(LogLevel.Error, x, $"Bad image format in '{assemblyPath}'.");
            }
            catch (Exception x)
            {
                logger.Log(LogLevel.Error, x, $"Exception thrown: '{assemblyPath}'.");
            }

            return default;
        }

        /// <summary>
        /// Source: https://www.michael-whelan.net/replacing-appdomain-in-dotnet-core/
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        private static IEnumerable<Assembly> GetReferencingAssemblies(string assemblyName, DependencyContext dependencyContext)
        {
            var assemblies = new List<Assembly>();
            foreach (var library in dependencyContext.RuntimeLibraries)
            {
                if (ExtensionsLoader.IsCandidateLibrary(library, assemblyName))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
            }
            return assemblies;
        }

        /// <summary>
        /// Source: https://www.michael-whelan.net/replacing-appdomain-in-dotnet-core/
        /// </summary>
        /// <param name="library"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private static bool IsCandidateLibrary(RuntimeLibrary library, string assemblyName)
            => library.Name == assemblyName || library.Dependencies.Any(d => d.Name.StartsWith(assemblyName));
    }
}