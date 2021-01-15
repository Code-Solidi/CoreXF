/*
 * Copyright (c) 2016-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using CoreXF.Abstractions.Base;
using CoreXF.Abstractions.Registry;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreXF.Framework.Registry
{
    internal class ExtensionsRegistry : IExtensionsRegistry
    {
        private readonly List<IExtension> extensions = new();
        private readonly ILogger logger;

        public IEnumerable<IExtension> Extensions => this.extensions;

        public ExtensionsRegistry(ILoggerFactory factory)
        {
            this.logger = factory.CreateLogger<ExtensionsRegistry>();
        }

        public T GetExtension<T>() where T : IExtension
        {
            return (T)this.extensions.SingleOrDefault(x => x is T); ;
        }

        public void Register(IExtension extension)
        {
            var found = this.extensions.SingleOrDefault(x => x.Name == extension.Name);
            if (found != null)
            {
                this.logger.LogError($"Extension {extension.Name}, v{extension.Version} has already been registered.");
            }
            else
            {
                this.extensions.Add(extension);
            }
        }

        public IExtension GetExtension(string name) 
            => this.extensions.SingleOrDefault(x => x.Name == name);

        public IExtension GetExtension(Assembly assembly)
            => this.Extensions.SingleOrDefault(x => x.GetType().Assembly.GetName().FullName == assembly.GetName().FullName);
    }
}