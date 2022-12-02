/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Abstractions.Base;
using CoreXF.Abstractions.Registry;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace CoreXF.Framework.Registry
{
    internal sealed class ExtensionsLoader
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

        public static IExtensionsRegistry DiscoverExtensions(IServiceCollection services, ILoggerFactory factory, string location)
        {
            var registry = new ExtensionsRegistry(factory);
            var excludes = new[]
            {
                Assembly.GetAssembly(typeof(ExtensionBase)).FullName,       // CoreXF.Abstractions
                Assembly.GetAssembly(typeof(ExtensionsLoader)).FullName     // CoreXF.Framework
            };

            new ExtensionsLoader(factory, registry).Discover(services, location, excludes);

            return registry;
        }

        private void Discover(IServiceCollection services, string location, string[] excludes)
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
                return;
            }

            foreach (var assembly in this.LoadAssemblies(files))
            {
                this.logger.LogDebug($"Inspecting {assembly.Location}.");
                if (!excludes.Any(x => x == assembly?.FullName))
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
                                instance.ConfigureServices(services);
                                instance.Location = Path.GetDirectoryName(assembly.Location);
                                (this.registry as ExtensionsRegistry)?.Register(instance);
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
        }

        private List<Assembly> LoadAssemblies(string[] files)
        {
            this.logger.LogDebug("Loading assemblies...");
            var assemblyList = new List<Assembly>();
            foreach (var path in files)
            {
                this.logger.LogDebug($"Loading assembly: '{path}'.");
                var assembly = ExtensionsLoader.LoadAssembly(path, this.logger);
                if (assembly != default)
                {
                    assemblyList.Add(assembly);
                }
            }

            return assemblyList;
        }

        internal static Assembly LoadAssembly(string assemblyPath, ILogger logger)
        {
            //if (assemblyPath.Contains("Extensions\\Identity"))
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            try
            {
                // load dependent assemblies:
                // https://samcragg.wordpress.com/2017/06/30/resolving-assemblies-in-net-core/
                return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);  // WARNING: once loaded it's forever!
                //return new AssemblyResolver(assemblyPath).Assembly;
            }
            catch (FileLoadException x)
            {
                logger.Log(LogLevel.Error, x, $"Error loading '{assemblyPath}'.");
            }
            catch (FileNotFoundException x)
            {
                logger.Log(LogLevel.Error, x, $"'{assemblyPath}' not found.");
            }
            catch (BadImageFormatException x)
            {
                logger.Log(LogLevel.Error, x, $"Bad image format in '{assemblyPath}'.");
            }
            catch (Exception x)
            {
                logger.Log(LogLevel.Error, x, $"Generic exception thrown: '{assemblyPath}'.");
            }

            return null;
        }

        //private sealed class AssemblyResolver : IDisposable
        //{
        //    private readonly ICompilationAssemblyResolver assemblyResolver;
        //    private readonly DependencyContext dependencyContext;
        //    private readonly AssemblyLoadContext loadContext;

        //    public AssemblyResolver(string path)
        //    {
        //        this.Assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        //        this.dependencyContext = DependencyContext.Load(this.Assembly);

        //        this.assemblyResolver = new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[]
        //        {
        //            new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(path)),
        //            new ReferenceAssemblyPathResolver(),
        //            new PackageCompilationAssemblyResolver()
        //        });

        //        this.loadContext = AssemblyLoadContext.GetLoadContext(this.Assembly);
        //        this.loadContext.Resolving += OnResolving;
        //    }

        //    public Assembly Assembly { get; }

        //    public void Dispose()
        //    {
        //        this.loadContext.Resolving -= this.OnResolving;
        //    }

        //    private Assembly OnResolving(AssemblyLoadContext context, AssemblyName name)
        //    {
        //        bool NamesMatch(RuntimeLibrary runtime)
        //        {
        //            return string.Equals(runtime.Name, name.Name, StringComparison.OrdinalIgnoreCase);
        //        }

        //        var library = this.dependencyContext.RuntimeLibraries.FirstOrDefault(NamesMatch);
        //        if (library != null)
        //        {
        //            var wrapper = new CompilationLibrary(library.Type
        //                , library.Name
        //                , library.Version
        //                , library.Hash
        //                , library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths)
        //                , library.Dependencies
        //                , library.Serviceable);

        //            var assemblies = new List<string>();
        //            this.assemblyResolver.TryResolveAssemblyPaths(wrapper, assemblies);
        //            if (assemblies.Count > 0)
        //            {
        //                return this.loadContext.LoadFromAssemblyPath(assemblies[0]);
        //            }
        //        }

        //        return null;
        //    }
        //}
    }
}